using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using server.Models;
using System;
using server.Dto;
using server.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace server.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(DataContext context, IUserService userService,IConfiguration configuration)
        {
            _context = context;
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost]
        public void Save([FromBody]UserDto userDto)
        {
            _context.AllUsers.Add(new User
            {
                Id = Guid.NewGuid(),
                Dni = userDto.Dni,
                Usuario = userDto.Usuario,
                Password = userDto.Password,
                Token = "2321321"
            });
            _context.SaveChanges();
        }

        [HttpPost("Auth")]
        public ActionResult<User> Authentication([FromBody]LoginDto p_LoginDto)
        {
            var u = _userService.Authenticate(p_LoginDto.Usuario, p_LoginDto.Password);
            
            if (u != null)
            {
                // authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();

                //Settings
                string set = _configuration.GetSection("AppSettings").GetSection("Secret").Value; 
                var key = Encoding.ASCII.GetBytes(set);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, u.Id.ToString(), ClaimTypes.SerialNumber, u.Dni.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                u.Token = tokenHandler.WriteToken(token);
            }
            u.Password = "";
            return u;

            //
        }

        [HttpGet("getall")]
        public ActionResult<List<User>> GetAll()
        {
            return _context.AllUsers.ToList();
        }

        [HttpGet("getbyid/{id}/")]
        public ActionResult<User> GetById(long id)
        {
            var item = _context.AllUsers.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            return item;
        }
    }
}