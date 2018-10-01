using Microsoft.AspNetCore.Identity;
using System;

namespace server.Models
{
    public class User : IdentityUser<Guid>
    {
        public int Dni { set; get; }
        public string Usuario { set; get; }
        public string Password { set; get; }
        public string Token { set; get; }
        public new byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}