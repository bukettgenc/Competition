using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace competition.Models
{
    public class ProfilePost
    {
        public int Id { get; set; }
        public string ProfilePosts { get; set; }
        public string VideoPosts { get; set; }
        public int ProfileNu { get; set; }
        public int CategoryId { get; set; }
        public int NumberofLikes { get; set; }
        public bool ForTheCompetition { get; set; }
        public  DateTime Date { get; set; }
    }
}