using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace competition.Models
{
    public class Competition
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
    }
}