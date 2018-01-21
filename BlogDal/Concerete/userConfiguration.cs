using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Concerete
{
    public class userConfiguration:EntityTypeConfiguration<user>
    {
        public userConfiguration()
        {
            HasKey(k => k.userId);
            HasMany(f => f.groupUsers).WithRequired(r => r.users);
            HasMany(ff => ff.favPosts).WithRequired(rr => rr.users);

        }
    }
}
