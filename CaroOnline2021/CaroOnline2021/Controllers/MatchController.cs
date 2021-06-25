using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaroOnline2021.HubConfig;
using CaroOnline2021.Models;
using CaroOnline2021.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CaroOnline2021.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IHubContext<CaroRealtimeHub> _hub;
        public MatchController(AppDbContext context, IHubContext<CaroRealtimeHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        [HttpGet("get-match-by-room/{roomId}")]
        public object GetMatchByRoom(string roomId)
        {
            var room = _context.Rooms.FirstOrDefault(x => x.Id.ToString() == roomId);

            if (room == null)
                return null;

            var match = _context.Matches.FirstOrDefault(x => x.RoomId.ToString() == roomId && x.WinnerId == null);

            if (match == null)
                return null;

            return match;
        }


        [HttpPost("play")]
        public async Task<object> PlayChess(MatchDetail detail)
        {
            var details = _context.MatchDetails.Where(x => x.MatchId == detail.MatchId);

            int order = details.Count() + 1;
            detail.Id = Guid.NewGuid();
            detail.Order = order;
            detail.SortNumber = order;

            _context.MatchDetails.Add(detail);
            _context.SaveChanges();


            var match = _context.Matches.FirstOrDefault(x => x.Id == detail.MatchId);

            // Gửi tín hiệu báo đấm đc rồi
            var firstUser = _context.UserRooms.FirstOrDefault(x => x.Id == match.FirstUserId);

            var matchDual = new MatchDualRequest();
            matchDual.RoomId = match.RoomId.ToString();
            matchDual.FirstUserId = match.FirstUserId.ToString();
            matchDual.SecondUserId = match.SecondUserId.ToString();


            //Gửi model cho signalr để thông báo các user trong phòng thực hiện đấm nhau

            var userInRoom = _context.UserRooms.Where(x => x.RoomId.ToString() == match.RoomId.ToString());

            var users = new List<User>();

            foreach (var item in userInRoom)
            {
                var currentUser = _context.Users.FirstOrDefault(x => x.Id == item.UserId);
                users.Add(currentUser);

            }

            matchDual.Users = users;
            var matchDetails = _context.MatchDetails.Where(x => x.MatchId == match.Id).ToList();

            matchDual.MatchDetails = matchDetails;

            await _hub.Clients.All.SendAsync("match-dual", matchDual);

            return Ok(detail);
        }


        [HttpGet("turn-of-user/{matchId}")]
        public object GetTurnOfUserID(string matchId)
        {
            var match = _context.Matches.FirstOrDefault(x => x.Id.ToString() == matchId);

            if (null == match)
                return BadRequest("Không tìm thấy trận đấy");

            var matchDetails = _context.MatchDetails.Where(x => x.MatchId == match.Id);

            if (matchDetails.Count() % 2 == 0)
            {
                var user = _context.Users.FirstOrDefault(x => x.Id == match.FirstUserId);

                return Ok(user);
            }
            else
            {
                var user = _context.Users.FirstOrDefault(x => x.Id == match.SecondUserId);

                return Ok(user);
            }
        }

        [HttpGet("get-match-details/{matchId}")]
        public object GetMatchDetailsByMatchId(string matchId)
        {
            var match = _context.Matches.FirstOrDefault(x => x.Id.ToString() == matchId);

            if (null == match)
                return BadRequest("Không tìm thấy trận đấy");

            var matchDetails = _context.MatchDetails.Where(x => x.MatchId == match.Id);

            return Ok(matchDetails);
        }
    }
}
