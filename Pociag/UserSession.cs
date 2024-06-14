using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pociag
{
    class UserSession
    {
        public static string Username { get; set; } = "Guest";
        public static bool IsLoggedIn { get; set; } = false;
        public static void Login(string username)
        {
            Username = username;
            IsLoggedIn = true;
        }
        public static void Logout()
        {
            Username = "Guest";
            IsLoggedIn=false;
        }
    }
}
