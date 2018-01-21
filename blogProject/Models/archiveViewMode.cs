using BlogDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace blogProject.Models
{
    public class archive
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int Year { get; set; }
    }
    public class archiveViewMode
    {
       // public Dictionary<string,int> monthsList { get; set; }
        public IList<archive> listArchive { get; set; }
    }
}