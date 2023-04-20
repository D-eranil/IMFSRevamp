using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("EmailAttachments")]
    public partial class EmailAttachment: BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public int? EmailId { get; set; }

        public string FileName { get; set; }

        public string PhysicalPath { get; set; }

        public string ContentId { get; set; }
    }
}
