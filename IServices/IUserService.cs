using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Models;

namespace server.IServices
{

    public interface IUserService
    {
        User Authenticate(string username, string password);
        void Update(User user);
        void Delete(Guid id);
        User Create(User user, string password);
    }
    
}
