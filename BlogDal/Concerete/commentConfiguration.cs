using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Concerete
{
    public class commentConfiguration:EntityTypeConfiguration<comment>
    {
        public commentConfiguration()
        {
            HasKey(k => k.commentId);
           
        }
    }
}
