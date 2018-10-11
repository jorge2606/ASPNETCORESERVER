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
        public async Task<IActionResult> Save([FromBody]createUserDto createUser)
        {
            var newUser = await _userService.CreateAsync(createUser);

            if (!newUser.IsSuccess)
            {
                return BadRequest(newUser);
            }
            return Ok(newUser);
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
        [Authorize]
        public ActionResult<ModifyUserDto> GetById(Guid id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return null;
            }
            ModifyUserDto modifyUserDto = new ModifyUserDto
            {
                Dni = user.Dni,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
            };

            var RolesUser = _context.UserRoles.ToList();
            var AllRoles = _context.Roles.ToList();

            List<RoleWhenModifyUser> ListRolesBelongsToUser = new List<RoleWhenModifyUser>();
            foreach (var role in AllRoles)
            {
                if (RolesUser.Exists(x => x.UserId == user.Id && x.RoleId == role.Id))
                {
                    RoleWhenModifyUser roleWhenModifyUser = new RoleWhenModifyUser
                    {
                        Id = role.Id,
                        Name = role.Name,
                        RolBelongUser = true
                    };

                    ListRolesBelongsToUser.Add(roleWhenModifyUser);
                }
                else
                {
                    RoleWhenModifyUser roleWhenModifyUser = new RoleWhenModifyUser
                    {
                        Id = role.Id,
                        Name = role.Name,
                        RolBelongUser = false
                    };

                    ListRolesBelongsToUser.Add(roleWhenModifyUser);

                }
            }
            modifyUserDto.RolesUser = ListRolesBelongsToUser;
            return modifyUserDto;
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody]MofidyUserCommingFromClientDto userDto)
        {
            await _userService.UpdateAsync(userDto);
            return Ok();
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
            var usersPaginator = _context.Users.OrderBy(x => x.UserName);
            return usersPaginator;
        }

        [HttpGet("page/{page}")]
        public PagedResult<User> userPagination(int? page)
        {
            const int pageSize = 10;
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