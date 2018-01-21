using BlogDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace blogProject.Models
{
    public class projectViewModel
    {
        public project ProjectView { get; set; }
        public bool IsAuthorize { get; set; }
    }
}