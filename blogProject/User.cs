using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace blogProject
{
    public class User
    {
        public int userId { get; set; }
        public string name { get; set; }
        public decimal lat { get; set; }
        
        public string status { get; set; }
        public decimal lng { get; set; }
        public string activeStatus { get; set; }
    }
}