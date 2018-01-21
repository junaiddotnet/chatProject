using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal
{
    public class group
    { 
        public int groupId { get; set; }
        public string groupName { get; set; }
        public List<groupChat> groupChats { get; set; }
        public List<groupUser> groupUsers { get; set; }
    }
}
