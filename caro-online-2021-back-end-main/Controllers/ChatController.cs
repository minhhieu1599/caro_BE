using CaroOnline2021.HubConfig;
using CaroOnline2021.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaroOnline2021.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IHubContext<CaroRealtimeHub> _hub;
        private readonly AppDbContext _context;
        public ChatController(AppDbContext context, IHubContext<CaroRealtimeHub> hub)
        {
            _context = context;

            _hub = hub;
        }
        [HttpPost("message-user")]
        public object MessageUser([FromForm] MessagerUser messagerUser)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == messagerUser.UserId);
            if (null == user)
                return BadRequest(" Không tìm thấy");

            // quết toàn bộ datanase xem có tk nào rỗng
            var room = _context.Rooms.FirstOrDefault(x => x.Id == messagerUser.RoomId);
            if(null == room)
                return BadRequest("không tìm thấy phòng chat");

            // gửi thông điệp tới all những ai kết nối phòng chát
  

            _hub.Clients.All.SendAsync("chat-online", messagerUser);
            return Ok();// 
        }    
    }
}
