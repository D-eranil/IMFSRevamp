using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.ResponseModel
{
    public class QuoteTotalRateResponse
    {
		public int Id { get; set; }

		public double Value { get; set; }
		
		public int? QuoteDurationID { get; set; }
		public int FinanceType { get; set; }
		public string FinanceTypeDescr { get; set; }
		public string PaymentType { get; set; }
		public string UpdatedBy { get; set; }

		
		public double Min { get; set; }
		public double Max { get; set; }

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

		
		public int? FunderID { get; set; }
		public int? FunderPlanID { get; set; }


	}
}
