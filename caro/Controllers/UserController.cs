using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using caro.HubConfig;
using caro.Models;
using caro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace caro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private  IHubContext<CcaroRealtimeHub> _hub;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

            public UserController(AppDbContext context, IConfiguration config, IHubContext<CcaroRealtimeHub> hub)
            {
            _context = context;
            _config = config;
            _hub = hub;
            }
        [HttpPost("register")]
        [AllowAnonymous]
        public object Register(RegisterUser request)
        {
            if(!ModelState.IsValid)
            return BadRequest(ModelState);

            var userDB = _context.Users.FirstOrDefault(x => x.UserName == request.UserName);

            if (userDB != null)
                return BadRequest("Username này đã được đăng ký!");

            if (request.Password != request.ConfirmPassword)
                return BadRequest("Mật khẩu xác nhận không đúng!");
            // gán cho bên RegisterUser tương tác với nhau
            var user = new User();
            user.Id = Guid.NewGuid();
            user.IsAdmin = request.IsAdmin;
            user.Name = request.Name;
            user.UserName = request.UserName;

            user.Password = request.Password;
            user.Score = 100;

            _context.Users.Add(user);// lưu vào ram
            _context.SaveChanges(); // lưu vào cơ sở dữ liệu

            return Ok();
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public object Login([FromForm]LoginUser request)
        {

            var user = _context.Users.FirstOrDefault(x => x.UserName == request.UserName);

            if (user == null)
            {
                return BadRequest("Tài khoản này chưa được đăng ký!");
            }
            if (request.Password != request.Password)
                return BadRequest("Sai mật khẩu !");

            // ở đây thực hiện xử lý cap nhat trường status true 

            user.Status = true;

            _context.Users.Update(user);
            _context.SaveChanges();

            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),

                new Claim("userName", user.UserName),
                new Claim("password", user.Password),
                new Claim("idadmin", user.IsAdmin.ToString()),
                                new Claim("name", user.Name),
                new Claim("score", user.Score.ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);// app dụng thuật toán sha256

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(1),// thời gian hết hạn của token
                signingCredentials:creds);


            var users = _context.Users.ToList();
            // gửi danh sách user đi
            _hub.Clients.All.SendAsync("user-online", users);

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public object Logout(LoginUser request)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == request.UserName);
            if (user == null)
                return BadRequest("Tài khoản này chưa được đăng ký");

            if (user.Password != request.Password)
                return BadRequest("sai mật khẩu");

            //thực hiện cập nhật status true
            user.Status = false;

            _context.Users.Update(user);
            _context.SaveChanges();

            var users = _context.Users.ToList();

            // gửi danh sách user đi
            _hub.Clients.All.SendAsync("user-online", users);

            return Ok("Đăng xuất thành công");
        }
     }
}
