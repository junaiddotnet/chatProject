using BlogDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace blogProject.Models
{
    public class monthSelectPostViewModel
    {
        public IEnumerable<post> posts { get; set; }
        public string monthName { get; set; }
        public int year { get; set; }
    }
}