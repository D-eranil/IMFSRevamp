using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IMFS.Web.Models.DBModel
{
    [Table("EmailXref")]
    public partial class EmailXref: BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int EmailID { get; set; }
        public int? QuoteID { get; set; }
        public int? ApplicationID { get; set; }
        public int? ContractID { get; set; }

    }
}
