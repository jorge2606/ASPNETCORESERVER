using System;

namespace server.Models {
    public class User{
        public Guid Id { get; set; } 
        public int Dni { set; get; }
        public string Usuario { set; get; }
        public string Password { set; get; }
        public string Token { set; get; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }

}