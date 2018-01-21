using BlogDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace blogProject.Models
{
    public class postdetailViewMode
    {
        public post postDetails { get; set; }
        public User currentUser { get; set; }
    }
}