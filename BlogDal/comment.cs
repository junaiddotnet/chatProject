using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal
{
    public class comment
    {
        public int commentId { get; set; }
        public string commentTxt { get; set; }
        public DateTime commentDate { get; set; }
        public int postId { get; set; }
        public int userId { get; set; }
        [ForeignKey("userId")]

        public virtual user user { get; set; }
        public virtual post posts { get; set; }
    }
}
