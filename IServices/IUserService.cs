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
    }
    
}
