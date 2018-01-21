using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDal
{
    [Table("contactuss")]
    public class contactus
    {
        
        public int contactId { get; set; }
        [Required(ErrorMessage = "Please Provide Name ...")]

        public string contactName { get; set; }
        [Required (ErrorMessage ="Please Provide Email ...")]
        public string contactEmail { get; set; }
        [Required (ErrorMessage ="Please provide Message ..")]
        public string contactMessage { get; set; }
        [Required (ErrorMessage ="Please provide Date ..")]

        public DateTime contactDate { get; set; }
    }
}
