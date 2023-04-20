using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("ContactsXref")]
    public class ContactsXref: BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public string ContactID { get; set; }
        
        public int ApplicationNumber { get; set; }
    }
}
