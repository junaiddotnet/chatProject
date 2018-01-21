using BlogDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace blogProject.Models
{
    public class chatviewModel
    {
        public int uid { get; set; }
        public string name { get; set; }
        public string msg { get; set; }
        public string group { get; set; }
        public DateTime time { get; set; }
    }
    public class myProfileViewModel
    {
        public LoguserViewModel myUser { get; set; }
        public IEnumerable<comment> Comments { get; set; } // List of Posts which user Myself commented or Liked
        public IEnumerable<groupChat> chatMessagesList { get; set; }

    }
}