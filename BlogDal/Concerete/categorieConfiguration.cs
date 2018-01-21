using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Concerete
{
    public class categorieConfiguration: EntityTypeConfiguration<categorie>
    {
        public categorieConfiguration()
        {
            HasKey(k => k.categoryId);
            HasMany(c => c.posts).WithRequired(i => i.categorie);
        }
    }
}
