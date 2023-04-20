using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("QuoteLinesFinanceOption")]
    public class QuoteLinesFinanceOption: BaseEntity
    {
        public int Id { get; set; }
        public int QuoteLinesId { get; set; }
        public int Funder { get; set; }
        public int Duration { get; set; }
        public float Rate { get; set; }
        public decimal Value { get; set; }
    }
}
