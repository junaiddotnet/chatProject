using BlogDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace blogProject.Models
{
    public class categoriesViewModel
    {
        public IEnumerable<categorie> listCategories { get; set; }
    }
}