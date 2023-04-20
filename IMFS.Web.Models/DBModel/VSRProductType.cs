using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IMFS.Web.Models.DBModel
{
    [Table("VSRProductType")]
    public partial class VSRProductType: BaseEntity
    {
		public int Id { get; set; }

		public string VSRID { get; set; }

		public string VSRDescription { get; set; }

		public string VendorName { get; set; }

		public string LOBDescription { get; set; }

		public string ProductType { get; set; }

    }
}
