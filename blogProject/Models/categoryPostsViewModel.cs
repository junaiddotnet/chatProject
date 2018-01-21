using BlogDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace blogProject.Models
{
    public class categoryPostsViewModel
    {
        public IEnumerable<post> posts { get; set; }
        public string postCategory { get; set; }
    }
}