using Microsoft.AspNetCore.Identity;
using System;

namespace server.Models
{
    public class Role : IdentityRole<Guid>
    {
    }

    public class RoleDto
    {

        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
    }
}