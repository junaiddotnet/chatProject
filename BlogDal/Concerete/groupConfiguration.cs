using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Concerete
{
    public class groupConfiguration:EntityTypeConfiguration<group>
    {
        public groupConfiguration()
        {
            HasKey(k => k.groupId);
            HasMany(f => f.groupChats).WithRequired(r => r.groups);
        }
    }
}
