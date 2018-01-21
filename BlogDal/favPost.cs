using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal
{
    public class favPost
    {
        public int favPostId { get; set; }
        public virtual post posts { get; set; }
        public int postId { get; set; }
        public virtual user users { get; set; }
        public int userId { get; set; }
        public string comment { get; set; }
        public DateTime favDate { get; set; }
    }
}
