using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace blogProject.Models
{
    public class leaveCommentModel
    {
        public int postId { get; set; }
        public bool loged { get; set; }
        public string postName { get; set; }
    }
}