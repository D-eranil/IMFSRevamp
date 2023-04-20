using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("QuoteLineFormat")]
    public class QuoteLineFormat: BaseEntity
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
