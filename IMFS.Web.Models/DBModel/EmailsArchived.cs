using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("EmailsArchived")]
    public partial class EmailsArchived: BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public int? EmailQueue { get; set; }

        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public string CCEmail { get; set; }
        public string BCCEmail { get; set; }

        public string Subject { get; set; }
        public string ReplyTo { get; set; }

        public string Body { get; set; }

        public string BodyType { get; set; }

        public string Status { get; set; }

        public string Importance { get; set; }

        public string Notes { get; set; }

        public string EmailUniqueID { get; set; }

        public string InternetMessageId { get; set; }

        public bool? IsNew { get; set; }

        public string EmailType { get; set; }

        public int? ParentEmailId { get; set; }

        public DateTime? DateTimeReceived { get; set; }

        public DateTime? DateTimeCreated { get; set; }
    }
}
