using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("EmailAttachmentsTemp")]
    public partial class EmailAttachmentsTemp: BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public Guid? TempEmailId { get; set; }

        public string FileName { get; set; }

        public string PhysicalPath { get; set; }

        public string ContentId { get; set; }
        
        public string UploadedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
