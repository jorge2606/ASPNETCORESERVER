using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Dto
{
    public class LoginDto
    {
        public string Usuario { set; get; }
        public string Password { set; get; }
        public string Token { set; get; }
    }
}
