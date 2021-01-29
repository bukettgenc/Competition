using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace competition.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string ProfileImage { get; set; }
        public int HomepageNu { get; set; }
        public int ProfileNu { get; set; }
        public bool IsOnline { get; set; }
    }
}