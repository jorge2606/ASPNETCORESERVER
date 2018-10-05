using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;

namespace server.Models
{
    public class User : IdentityUser<Guid>
    {
        public int Dni { set; get; }

    }
}