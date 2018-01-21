using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Abstract
{
    public interface IadminRepository
    {
        IQueryable<contactus> posts { get; }

        void ContactUs(contactus prmConactUs);
    }
}
