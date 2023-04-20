using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Quote
{
    public class QuoteSearchModel
    {
        public int? QuoteNumber { get; set; }
        public int? QuoteStatus { get; set; }
        public string QuoteFinanceType { get; set; }
        public string EndUser { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string TriggerSource { get; set; }
        public int? ResellerAccount { get; set; }

    }
}
