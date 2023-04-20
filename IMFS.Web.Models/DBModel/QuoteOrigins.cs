using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("QuoteOrigins")]
    public class QuoteOrigins: BaseEntity
    {
        public int Id { get; set; }
        public string Description { get; set; }

    }
}
