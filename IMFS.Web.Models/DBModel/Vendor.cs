
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
	[Table("Vendor")]
	public partial class Vendor: BaseEntity
    {
		public int Id { get; set; }
		
		public string VendorCode { get; set; }
		
		public string VendorName { get; set; }
		
		public string SupplierNumber { get; set; }
		
		public string ManufacturerNumber { get; set; }
		
		public string Currency { get; set; }

		public bool? Active { get; set; }

		public DateTime? LastUpdatedDate { get; set; }
		
		public string SapId { get; set; }
		
		public string CompanyCode { get; set; }
		
    }
}
