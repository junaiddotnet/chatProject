using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal
{
    public class user
    {
        public int userId { get; set; }

        [Required(ErrorMessage ="Enter user name ...")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Enter user name ...")]
        public string password { get; set; }
        public string connectionId { get; set; }
        public decimal lat { get; set; }
        public decimal lng { get; set; }
        public string country { get; set; }
        public string status { get; set; }

        [NotMapped]
        public string confirmPassword { get; set; }
        [NotMapped]
        public string keyCode { get; set; }
        [NotMapped]
        public string confirmKeyCode { get; set; }

        public List<groupUser> groupUsers { get; set; }
        //
        public virtual List<favPost> favPosts { get; set; }

    }
}
