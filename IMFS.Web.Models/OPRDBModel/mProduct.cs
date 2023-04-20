using IMFS.Web.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IMFS.Web.Models.OPRDBModel
{
    [Table("mProduct")]
    public partial class mProduct : BaseEntity
    {
        [Key]
        public Guid ProductID { get; set; }
        public string InternalSKUID { get; set; }
        public string VendorSKUID { get; set; }
        public string ProductDescription { get; set; }
        public int VSRID { get; set; }
        public string PurchasingBlock { get; set; }
        public string SalesBlock { get; set; }
    }
}
