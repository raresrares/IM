using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int isLoggedIn { get; internal set; }
    }
}
