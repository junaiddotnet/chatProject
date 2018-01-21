using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace blogProject.Models
{
    public class loginViewMdel
    {
        public BlogDal.user prmUser { get; set; }
        public string confirmPassword { get; set; }

    }
}