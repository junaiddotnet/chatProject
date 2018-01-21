using BlogDal.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal.Concerete
{
    public class EFadminRepository : IadminRepository
    {
        private EFDbcontext context = new EFDbcontext();
        public IQueryable<contactus> posts
        {
            get
            {
                return context.contactuss;
            }
        }
        public void ContactUs(contactus prmConactUs)
        {
            context.contactuss.Add(prmConactUs);
            context.SaveChanges();
        }
    }
}
