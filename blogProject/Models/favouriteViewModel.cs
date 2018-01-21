using BlogDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace blogProject.Models
{
    public class favouriteViewModel
    {
        public int userId { get; set; }
        public IEnumerable<favPost> favPostList { get; set; }
    }
}