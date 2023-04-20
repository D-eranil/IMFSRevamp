using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("ContactsTypes")]
    public class ContactsTypes: BaseEntity
    {
        [Key]
        public int ContactType { get; set; }
        public string ContactDescription { get; set; }
    }
}
