using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Concerete
{
    public class postConfiguration:EntityTypeConfiguration<post>
    {
        public postConfiguration()
        {
            HasKey(k => k.postId);
            HasMany(c => c.comments).WithRequired(r => r.posts);
            HasMany(cc => cc.favPosts).WithRequired(rr => rr.posts);

        }
    }
}
