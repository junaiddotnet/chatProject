using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal
{
    public class groupUser
    {
      public int groupUsersId { get; set; }
      public int groupId { get; set; }
      public int userId { get; set; }
      public string activeStatus { get; set; }
      public virtual user users { get; set; }
      public virtual group groups { get; set; }

    }
}
