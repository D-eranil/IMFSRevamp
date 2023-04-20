using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.QuotePercentRate
{
    public class QuotePercentRateModel
    {
        public int? FunderId { get; set; }
        public int? FinanceType { get; set; }
        public int? ProductType { get; set; }
        public int? FunderPlan { get; set; }
    }
}
