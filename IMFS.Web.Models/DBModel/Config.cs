using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("Config")]
    public class Config : BaseEntity
    {
        public int id { get; set; }
        public string name { get; set; }
        public string value { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime expireDateTime { get; set; }
    }
}
