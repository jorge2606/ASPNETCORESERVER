using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Models;
using server.IServices;

namespace server.Services
{
    public class UserService : IUserService
    {
        private DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }


        public User Authenticate(string username, string password)
        {
            return _context.AllUsers.SingleOrDefault(x => x.Password.Equals(password) && x.Usuario == username);
        }
    }
}
