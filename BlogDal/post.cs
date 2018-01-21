using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal
{
    public class post
    {
        public int postId { get; set; }
        public string postName { get; set; }
        public string postText { get; set; }
        public DateTime createDate { get; set; }
        public int categoryId { get; set; }

        public virtual List<comment> comments { get; set; }
        public virtual categorie categorie { get; set; }
        //
        public virtual List<favPost> favPosts { get; set; }


    }
}
