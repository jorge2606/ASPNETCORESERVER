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
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using server.Helpers.WebApi.Helpers;
using server.Helpers;

namespace server.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private IUserService _userService;
        private readonly IConfiguration _configuration;
        private IMapper _mapper;

        public UserController(DataContext context, IUserService userService,IConfiguration configuration,
            IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpPost]
        public void Save([FromBody]SaveUserDto userDto)
        {
            _context.Users.Add(new User
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
                u.Password = "";
            }
            
            return u;

            //
        }

        [HttpGet("getall")]
        public ActionResult<List<User>> GetAll()
        {
            return _context.Users.ToList();
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]
        public ActionResult<SaveUserDto> GetById(Guid id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return null;
            }

            return _mapper.Map<SaveUserDto>(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]SaveUserDto userDto)
        {
            // map dto to entity
            var user = _mapper.Map<User>(userDto);

            try
            {
                // save 
                _userService.Create(user, userDto.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Authorize]
        public IActionResult Update([FromBody]SaveUserDto userDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<User>(userDto);
            user.Id = userDto.Id;

            try
            {
                // save 
                _userService.Update(user);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(Guid id)
        {
            _userService.Delete(id);
            return Ok();
        }

        public IQueryable<User> queryableUser()
        {
            var usersPaginator = _context.Users.OrderBy(x => x.Usuario);
            return usersPaginator;
        }

        [HttpGet("page/{page}")]
        public PagedResult<User> userPagination(int? page)
        {
            const int pageSize = 2;
            var queryPaginator = queryableUser();

            var result = queryPaginator.Skip((page ?? 0) * pageSize)
                                          .Take(pageSize)
                                          .ToList();
            return new PagedResult<User>
            {
                List = result,
                TotalRecords = queryPaginator.Count()
            };
        }
    }
}