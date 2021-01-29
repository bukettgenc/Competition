using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace competition.Models
{
    public class PostLikes
    {
        public int Id { get; set; }
        public int PostNu { get; set; }
        public int LikeProfileNu { get; set; }
    }
}