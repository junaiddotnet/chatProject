using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal
{
    public class groupChat
    { 
        public int groupChatId { get; set; }
        public string chatTxt { get; set; }
        public DateTime txtDate { get; set; }
        public int groupId { get; set; }
        public int userId { get; set; }
        public string txtType { get; set; }
        public string txtStatus { get; set; }
        public virtual user users { get; set; }
        public virtual group groups { get; set; }
    }
}
