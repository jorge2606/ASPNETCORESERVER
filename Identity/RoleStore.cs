using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Identity
{
    public class RoleStore : RoleStore<Role, DataContext, Guid>
    {
        public RoleStore(DataContext context) : base(context)
        {
        }
    }

    public class RoleUserDto
    {
        public Guid RolId { get; set; }
        public Guid UserId { get; set; }
    }
}
