using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.ResponseModel
{
    public class QuoteRateResponse
    {
		public int Id { get; set; }

		public double Value { get; set; }

		public DateTime LastSavedUtc { get; set; }

		public int? TypeID { get; set; }
		public int? CategoryID { get; set; }
		public int? QuoteDurationID { get; set; }
		public int QuoteDurationType { get; set; }
		public string QuoteDurationTypeDescr { get; set; }
		public string PaymentType { get; set; }
		public int? UpdatedByID { get; set; }

		public string TypeDescription { get; set; }
		public string CategoryDescription { get; set; }

		public string ImSKUID { get; set; }

		public string VendorSKUID { get; set; }
		
		public double? months12Monthly { get; set; }
		public double? months12Quarterly { get; set; }
		public double? months12Upfront { get; set; }
		public double? months24Monthly { get; set; }
		public double? months24Quarterly { get; set; }
		public double? months24Upfront { get; set; }
		public double? months36Monthly { get; set; }
		public double? months36Quarterly { get; set; }
		public double? months36Upfront { get; set; }
		public double? months48Monthly { get; set; }
		public double? months48Quarterly { get; set; }
		public double? months48Upfront { get; set; }
		public double? months60Monthly { get; set; }
		public double? months60Quarterly { get; set; }
		public double? months60Upfront { get; set; }

		public bool? IsRental { get; set; }
		public bool? IsLease { get; set; }
		public bool? IsInstalment { get; set; }

		public int? FunderID { get; set; }
		public int? VendorID { get; set; }
		public int? ProductID { get; set; }

		public string VendorCode { get; set; }
		public string VendorName { get; set; }
	}

}
