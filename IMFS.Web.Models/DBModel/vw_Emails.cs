using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("vw_Emails")]
    public partial class vw_Emails: BaseEntity
    {
        public int Id { get; set; }
        public Nullable<int> EmailQueue { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string CCEmail { get; set; }
        public string ReplyTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string BodyType { get; set; }
        public string Status { get; set; }
        public string Importance { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> DateTimeReceived { get; set; }
        public string EmailUniqueID { get; set; }
        public string InternetMessageId { get; set; }
        public Nullable<bool> IsNew { get; set; }
        public string EmailType { get; set; }
        public Nullable<int> ParentEmailId { get; set; }
        public Nullable<System.DateTime> DateTimeCreated { get; set; }
        public string BCCEmail { get; set; }
        public string Table { get; set; }
    }
}
