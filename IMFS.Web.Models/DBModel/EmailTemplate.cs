using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("EmailTemplates")]
    public partial class EmailTemplate : BaseEntity
    {
        [Key]
        public int DefaultID { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }
        public string Subject { get; set; }
        public string ToAddress { get; set; }
        public string CCAddress { get; set; }

        public string FromAddress { get; set; }

    }
}
