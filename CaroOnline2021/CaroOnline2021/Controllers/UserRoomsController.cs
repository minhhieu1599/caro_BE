using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CaroOnline2021;
using CaroOnline2021.Models;
using CaroOnline2021.ViewModels;
using Microsoft.AspNetCore.SignalR;
using CaroOnline2021.HubConfig;

namespace CaroOnline2021.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoomsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IHubContext<CaroRealtimeHub> _hub;
        public UserRoomsController(AppDbContext context, IHubContext<CaroRealtimeHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        // GET: api/UserRooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRoom>>> GetUserRooms()
        {
            return await _context.UserRooms.ToListAsync();
        }

        // GET: api/UserRooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserRoom>> GetUserRoom(Guid id)
        {
            var userRoom = await _context.UserRooms.FindAsync(id);

            if (userRoom == null)
            {
                return NotFound();
            }

            return userRoom;
        }

        // PUT: api/UserRooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserRoom(Guid id, UserRoom userRoom)
        {
            if (id != userRoom.Id)
            {
                return BadRequest();
            }

            _context.Entry(userRoom).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserRooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserRoom>> PostUserRoom(UserRoom userRoom)
        {
            if (string.IsNullOrEmpty(userRoom.UserId.ToString()) || string.IsNullOrEmpty(userRoom.RoomId.ToString()))
                return BadRequest("Không thể truy cập được phòng");

            var user = _context.Users.FirstOrDefault(x => x.Id == userRoom.UserId);
            var room = _context.Rooms.FirstOrDefault(x => x.Status != 0 && x.Id == userRoom.RoomId);

            if (null == user || null == room)
                return BadRequest("Không thể truy cập được phòng");


            var currentUserRoom = _context.UserRooms.FirstOrDefault(x => x.RoomId == userRoom.RoomId && x.UserId == userRoom.UserId);

            if (null != currentUserRoom)
                return CreatedAtAction("GetUserRoom", new { id = currentUserRoom.Id }, currentUserRoom);

            userRoom.Id = Guid.NewGuid();
            userRoom.CreatedDate = DateTime.Now;
            _context.UserRooms.Add(userRoom);

            if (room.Status == 1)
                room.Status = 2;

            await _context.SaveChangesAsync();

            var usersInRoom = _context.UserRooms.Where(x => x.RoomId == userRoom.RoomId).ToList();

            if (usersInRoom.Count == 2)
            {
                var newMatch = new Match();
                newMatch.Id = Guid.NewGuid();
                newMatch.CreatedDate = DateTime.Now;
                newMatch.SecondUserId = userRoom.UserId;
                var firstUser = _context.UserRooms.FirstOrDefault(x => x.RoomId == userRoom.RoomId && x.UserId != userRoom.UserId);
                newMatch.FirstUserId = firstUser.UserId;

                int scoreFirstUser = _context.Users.FirstOrDefault(x => x.Id == firstUser.UserId).Score;
                int scoreSecondUser = _context.Users.FirstOrDefault(x => x.Id == userRoom.UserId).Score;

                newMatch.FirstScore = scoreFirstUser;
                newMatch.SecondScore = scoreSecondUser;
                newMatch.RoomId = userRoom.RoomId;

                _context.Matches.Add(newMatch);
                _context.SaveChanges();

                // Gửi tín hiệu báo đấm đc rồi

                var matchDual = new MatchDualRequest();
                matchDual.RoomId = userRoom.RoomId.ToString();
                matchDual.FirstUserId = firstUser.UserId.ToString();
                matchDual.SecondUserId = userRoom.UserId.ToString();


                //Gửi model cho signalr để thông báo các user trong phòng thực hiện đấm nhau

                var userInRoom = _context.UserRooms.Where(x => x.RoomId.ToString() == userRoom.RoomId.ToString());

                var users = new List<User>();

                foreach (var item in userInRoom)
                {
                    var currentUser = _context.Users.FirstOrDefault(x => x.Id == item.UserId);
                    users.Add(currentUser);

                }

                matchDual.Users = users;



             
                await _hub.Clients.All.SendAsync("match-dual", matchDual);

            }

            // Gửi kèm danh sách nước đánh 
            if(usersInRoom.Count >= 3)
            {
           

                // Gửi tín hiệu báo đấm đc rồi
                var firstUser = _context.UserRooms.FirstOrDefault(x => x.RoomId == userRoom.RoomId && x.UserId != userRoom.UserId);

                var matchDual = new MatchDualRequest();
                matchDual.RoomId = userRoom.RoomId.ToString();
                matchDual.FirstUserId = firstUser.UserId.ToString();
                matchDual.SecondUserId = userRoom.UserId.ToString();


                //Gửi model cho signalr để thông báo các user trong phòng thực hiện đấm nhau

                var userInRoom = _context.UserRooms.Where(x => x.RoomId.ToString() == userRoom.RoomId.ToString());

                var users = new List<User>();

                foreach (var item in userInRoom)
                {
                    var currentUser = _context.Users.FirstOrDefault(x => x.Id == item.UserId);
                    users.Add(currentUser);

                }

                matchDual.Users = users;
                var match = _context.Matches.FirstOrDefault(x => x.RoomId == userRoom.RoomId);
                var matchDetails = _context.MatchDetails.Where(x => x.MatchId == match.Id).ToList();

                matchDual.MatchDetails = matchDetails;

                await _hub.Clients.All.SendAsync("match-dual", matchDual);
            }
            

            return CreatedAtAction("GetUserRoom", new { id = userRoom.Id }, userRoom);
        }

        // DELETE: api/UserRooms/userId/roomId
        [HttpDelete("{userId}/{roomId}")]
        public async Task<IActionResult> DeleteUserRoom(string userId, string roomId)
        {
            var userRoom = await _context.UserRooms.FirstOrDefaultAsync(ur => ur.UserId.ToString() == userId && ur.RoomId.ToString() == roomId);
            if (null == userRoom)
                return BadRequest("Đối tượng chưa vào phòng!");

            _context.UserRooms.Remove(userRoom);

            await _context.SaveChangesAsync();

            var userRooms = _context.UserRooms.Where(x => x.RoomId.ToString() == roomId);

            if (userRooms.Count() == 0)
            {
                var room = _context.Rooms.FirstOrDefault(x => x.Id.ToString() == roomId);
                room.Status = 1;

                _context.Rooms.Update(room);
                await _context.SaveChangesAsync();
            }

            // trong khi trận đấu chưa kết 1 trong 2 th player thoát ra thì trận đấu được coi như kết
            // set cái thằng còn lại thành winner 

            // trường hợp không là người chơi

            var match = _context.Matches.FirstOrDefault(x => x.RoomId.ToString() == roomId);

            if(null == match)
            {

                return Ok("Xóa thành công!");
            }

            if (match.FirstUserId.ToString() == userId || match.SecondUserId.ToString() == userId)
            {
                // kết thúc trận đấu
                if (match.FirstUserId.ToString() == userId)
                {
                    match.WinnerId = match.SecondUserId;
                }
                else
                {
                    match.WinnerId = match.FirstUserId;
                }

                var winner = _context.Users.FirstOrDefault(x => x.Id == match.WinnerId);

                var matchDetails = _context.MatchDetails.Where(x => x.MatchId == match.Id);

                _context.MatchDetails.RemoveRange(matchDetails);
                _context.SaveChanges();

                _context.Matches.Remove(match);
                _context.SaveChanges();

                await _hub.Clients.All.SendAsync("winner-notify", winner);
            }

            return Ok("Xóa thành công!");
        }

        private bool UserRoomExists(Guid id)
        {
            return _context.UserRooms.Any(e => e.Id == id);
        }


        [HttpGet("get-users-by-room/{roomId}")]
        public object GetUsersByRoom(string roomId)
        {

            var userInRoom = _context.UserRooms.Where(x => x.RoomId.ToString() == roomId);

            var users = new List<User>();

            foreach (var item in userInRoom)
            {
                var currentUser = _context.Users.FirstOrDefault(x => x.Id == item.UserId);
                users.Add(currentUser);
            }

            return Ok(users);
        }

        // get thứ tự vào phòng
        [HttpGet("get-order-by-user/{roomId}/{userId}")]
        public int GetOrderByUser(string roomId, string userId)
        {
            var room = _context.Rooms.FirstOrDefault(x => x.Id.ToString() == roomId);

            if (room == null)
                return -1;

            var user = _context.Users.FirstOrDefault(x => x.Id.ToString() == userId);

            if (user == null)
                return -1;

            var users = _context.UserRooms.Where(x => x.RoomId.ToString() == roomId).OrderBy(x => x.CreatedDate).ToList();

            for (int i = 0; i < users.Count(); i++)
            {
                if (userId == users[i].UserId.ToString())
                    return i + 1;
            }

            return -1;
        }



    }
}
