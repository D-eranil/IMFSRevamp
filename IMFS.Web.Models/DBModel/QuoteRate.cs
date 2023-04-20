using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
	[Table("QuoteRate")]
	public partial class QuoteRate : BaseEntity
	{

		public int Id { get; set; }		
		public double Value { get; set; }	
		public DateTime? ModifiedDate { get; set; }
		public int? TypeID { get; set; }		
		public int? QuoteDurationID { get; set; }
		public int QuoteDurationType { get; set; }
		public string PaymentType { get; set; }
		public int? UpdatedByID { get; set; }
		public int? CategoryID { get; set; }
		
		public string ImSKUID { get; set; }
		
		public string VendorSKUID { get; set; }

		public int? FunderID { get; set; }
		public int? VendorID { get; set; }
		public int? ProductID { get; set; }
	}
}

