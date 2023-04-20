using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
	[Table("Product")]
	public partial class Product: BaseEntity
	{
		
		public Guid ProductID { get; set; }

		public string ImSKUID { get; set; }

		public string VendorSKUID { get; set; }

		public string ProductDescription { get; set; }

		public int? VSRID { get; set; }

		public bool? OnWeb { get; set; }

		public DateTime? LastModifiedDate { get; set; }

		public int? WebCategoryID { get; set; }

		public decimal? PIR { get; set; }

		public string LongDescriptionOverride { get; set; }

		public bool? HasAddOnOptions { get; set; }
		
    }
}
