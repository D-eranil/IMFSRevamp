using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IMFS.Web.Models.DBModel
{
    [Table("QuoteLogs")]
    public partial class QuoteLog: BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int? QuoteId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
