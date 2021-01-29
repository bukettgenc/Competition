using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace competition.Models
{
    public class MessagesBox
    {
        public int Id { get; set; }
        public int ProfileNu { get; set; }
        public int MessageSender { get; set; }

        public string Message { get; set; }
    }
}