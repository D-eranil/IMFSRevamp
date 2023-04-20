using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IMFS.Web.Models.DBModel
{
	[Table("QuoteBreakPercentRate")]
	public class QuoteBreakPercentRate: BaseEntity
    {
		public int Id { get; set; }
		public double Value { get; set; }
		public double? MinPercent { get; set; }
		public double? MaxPercent { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public int? QuoteDurationID { get; set; }
		public int FinanceType { get; set; }
		public int ProductType { get; set; }
		public string PaymentType { get; set; }
		public string UpdatedBy { get; set; }
		public int? FunderID { get; set; }
		public int? FunderPlanID { get; set; }
	}
}
