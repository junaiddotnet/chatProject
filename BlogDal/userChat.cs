using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal
{
    public class userChat
    {
        public int userChatId { get; set; }
        public string commentTxt { get; set; }
        public DateTime commentDate { get; set; }
        
        public int senderId { get; set; }
        public int receiverId { get; set; }
        public bool messageStatus { get; set; }
        [ForeignKey("senderId")]
        public virtual user sender { get; set; }
        [ForeignKey("receiverId")]
        public virtual user receiver { get; set; }
    }
}
