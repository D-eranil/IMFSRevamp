using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("OTC")]
    public partial class OTC: BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public int? QuoteId { get; set; }

        public DateTime? Sent { get; set; }

        public DateTime? Accepted { get; set; }

        public string Code { get; set; }

        public string Recipient { get; set; }

        public string RequestedIP { get; set; }

        public string AcceptedIP { get; set; }
    }
}
