using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Identity
{
    public class UserStore : UserStore<User, Role, DataContext, Guid>
    {
        public UserStore(DataContext context) : base(context)
        {
        }
    }

}
