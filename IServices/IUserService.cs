using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using server.Dto;
using server.Models;
using server.ServiceResult;

namespace server.IServices
{

    public interface IUserService
    {
        UserDto Authenticate(string username, string password);
        Task UpdateAsync(MofidyUserCommingFromClientDto user);
        Task<ServiceResult<string>> CreateAsync(createUserDto user);
        void Delete(Guid id);
        Task<ServiceResult<string>> Register(SaveUserDto model);
        Task UpdateUserRole(Guid idUser, Guid id);
    }
    
}
