using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.QuoteTotalRate
{
    public class QuoteTotalRateModel
    {
        public int? FunderId { get; set; }        
        public int? FinanceType { get; set; }
        public int? FunderPlan { get; set; }
    }
}
