using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IMFS.Web.Models.DBModel
{
    [Table("QuoteBreakTotalRate")]
    public class QuoteBreakTotalRate: BaseEntity
    {
		public int Id { get; set; }
		public double Value { get; set; }
		public double? Min { get; set; }
		public double? Max { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public int? QuoteDurationID { get; set; }
		public int FinanceType { get; set; }
		public string PaymentType { get; set; }
		public string UpdatedBy { get; set; }
		public int? FunderID { get; set; }
		public int? FunderPlanID { get; set; }

	}
}
