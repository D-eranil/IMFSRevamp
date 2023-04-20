namespace IMFS.Web.Models.BestRates
{
	public class BestRateBySKU
    {		
		public double Value { get; set; }
		
		public int? Id { get; set; }
		public int? TypeID { get; set; }
		public int? QuoteDurationID { get; set; }
		public int QuoteDurationType { get; set; }
		public string PaymentType { get; set; }		
		public int? CategoryID { get; set; }
		public string ImSKUID { get; set; }
		public string VendorSKUID { get; set; }
		public int? FunderID { get; set; }
		
	}
}
