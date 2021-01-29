using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace competition.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ProfilePost> ProfilePosts { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<Blocked> Blockeds { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<PostLikes> PostLikeses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Top3> Top3s { get; set; }
        public DbSet<MessagesBox> MessagesBoxes { get; set; }

    }
}