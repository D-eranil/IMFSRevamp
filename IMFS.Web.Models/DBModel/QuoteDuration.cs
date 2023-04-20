using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("QuoteDuration")]
    public partial class QuoteDuration:BaseEntity
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public bool IsActive { get; set; }
    }
}
