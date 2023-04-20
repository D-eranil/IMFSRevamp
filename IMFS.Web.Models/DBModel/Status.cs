using System;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("Status")]

    public partial class Status : BaseEntity
    {
        public int Id { get; set; }

        public string Description { get; set; }
        public bool? IsQuote { get; set; }
        public bool? IsApplication { get; set; }
        public string HPCode { get; set; }
        public string HPDescription { get; set; }
    }
}
