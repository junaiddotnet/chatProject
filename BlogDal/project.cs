using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal
{
   public class project
    {
        public int projectId { get; set; }
        public string projectName { get; set; }
        public string projectDescription { get; set; }
        public string projectImg { get; set; }
        public DateTime createDate { get; set; }
    }
}
