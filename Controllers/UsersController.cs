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
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using server.Identity;

namespace server.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private IUserService _userService;
        private readonly IConfiguration _configuration;
        private IMapper _mapper;

        public UserController(DataContext context, IUserService userService, IConfiguration configuration,
            IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpPost("SaveRolUser")]
        [AllowAnonymous]
        public async Task<ActionResult> SaveRolUser([FromBody]RoleUserDto rolUser)
        {
            await _userService.UpdateUserRole(rolUser.UserId, rolUser.RolId);
            return Ok(rolUser);
        }

        [HttpPost]
        public void Save([FromBody]SaveUserDto userDto)
        {
            _context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Dni = userDto.Dni
            });
            _context.SaveChanges();
        }

        [HttpPost("Auth")]
        public ActionResult<UserDto> Authentication([FromBody]LoginDto p_LoginDto)
        {
            var u = _userService.Authenticate(p_LoginDto.Usuario, p_LoginDto.Password);

            return u;

            //
        }

        [HttpGet("getall")]
        public ActionResult<List<User>> GetAll()
        {
            return _context.Users.ToList();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]SaveUserDto userDto)
        {
            var result = await _userService.Register(userDto);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            UserAuthenticationDto ObjToken = new UserAuthenticationDto
            {
                Token = result.Response
            };

            return Ok(ObjToken);
        }

        [HttpGet("getbyid/{id}")]
        //[Route]
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
                // _userService.Update(user);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        /*     [HttpDelete("{id}")]
             [Authorize]
             public IActionResult Delete(Guid id)
             {
                 _userService.Delete(id);
                 return Ok();
             }
             */
             public IQueryable<User> queryableUser()
             {
                 var usersPaginator = _context.Users.OrderBy(x => x.UserName);
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