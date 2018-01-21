using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal
{
    public class categorie
    {
      public int  categoryId { get; set; }
      public string categoryName { get; set; }
      public virtual List<post> posts { get; set; }
    }
}
