using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace competition.Models
{
    public class Top3
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int RepeatCount { get; set; }
    }
}