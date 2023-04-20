using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("Attachments")]
    public partial class Attachments : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int? QuoteId { get; set; }
        public int? ApplicationId { get; set; }
        public int? ApplicationNumber { get; set; }
        public int? Source { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string PhysicalPath { get; set; }
        public string UploadBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastAccessDate { get; set; }

    }
}
