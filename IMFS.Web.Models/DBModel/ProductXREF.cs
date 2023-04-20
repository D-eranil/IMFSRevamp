using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("ProductXref")]
    public partial class ProductXref : BaseEntity
    {
        [Key]
        public string InternalSKUID { get; set; }
        public string VPN { get; set; }
        public string Level1Category { get; set; }
        public string Level2Category { get; set; }
        public string Level3Category { get; set; }
        public string VendorName { get; set; }
        public string HPFSCategory { get; set; }
    }
}
