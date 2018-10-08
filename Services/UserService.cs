using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Models;
using server.IServices;
using server.Helpers.WebApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using server.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using server.Dto;
using server.ServiceResult;

namespace server.Services
{
    public class UserService : IUserService
    {
        private DataContext _context;
        private UserManager _userManager;
        private readonly RoleManager _roleManager;
        

        private readonly SignInManager _signInManager;

        public UserService(DataContext context,
            UserManager userManager, 
            RoleManager roleManager, 
            IConfiguration configuration,
            SignInManager signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        private IConfiguration _configuration { get; }

        public UserDto Authenticate(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.UserName == username);
            UserDto userDto = new UserDto();
            if (user != null)
            {
                var token = GenerateJwtTokenHandler(user.Email, user);

                userDto.Id = user.Id;
                userDto.Dni = user.Dni;
                userDto.UserName = user.UserName;
                userDto.PhoneNumber = user.PhoneNumber;
                userDto.Token = token;
            }
                return userDto;
        }

        public void Delete(Guid id)
        {
            var user = _context.Users.Find(id);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        //public async Task UpdateUserRole(Guid idUser, Guid roleId)
        //{
        //    var user = await _userManager.FindByIdAsync(idUser)
        //    //_userRoles.RoleId = roleId;
        //    //_userRoles.UserId = idUser;
        //    _userManager.AddToRoleAsync()
        //}

        public void Update(SaveUserDto userParam)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.UserName != user.UserName)
            {
                // username has changed so check if the new username is already taken
                if (_context.Users.Any(x => x.UserName == userParam.UserName))
                    throw new AppException("El usuario " + userParam.UserName + " ya existe en la db.");
            }

            // update user properties
            user.Dni = userParam.Dni;
            user.UserName = userParam.UserName;
            user.Email = userParam.UserName;
            user.PhoneNumber = userParam.PhoneNumber;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(userParam.Password))
            {
               user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userParam.Password);
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        //Create JWToken
        private string GenerateJwtTokenHandler(string email, User user)
        {
            var Token = "";
            if (user != null)
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
                    new Claim(ClaimTypes.Name, user.Id.ToString(), ClaimTypes.SerialNumber, user.Dni.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                Token = tokenHandler.WriteToken(token);
            }

            return Token;
        }
        
        public async Task<ServiceResult<string>> Register(SaveUserDto model)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Dni = model.Dni,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user,model.Password);

            if (!result.Succeeded)
            {
                return result.ToServiceResult<string>(null);
            }

            await _signInManager.SignInAsync(user, false);

            var token = GenerateJwtTokenHandler(model.Email, user);
             
            return new ServiceResult<string>(token);

        }

        public async Task UpdateUserRole(Guid userId, Guid roleId)
        {
            
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleId);

            if (user != null && role != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);

                foreach (var currentRole in currentRoles)
                {
                    await _userManager.RemoveFromRoleAsync(user, currentRole);
                }

                var result = await _userManager.AddToRoleAsync(user, role.Name);

                if (!result.Succeeded)
                {

                }
            }
        }
    }
}
