using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace competition.Models
{
    public class Follower
    {
        public int Id { get; set; }
        public int ProfileNu { get; set; }
        public int FriendProfileNu { get; set; }
    }
}