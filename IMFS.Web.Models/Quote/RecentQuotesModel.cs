using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Quote
{
    public class RecentQuotesModel : BaseEntity
    {
        public int? QuoteNumber { get; set; }
     
        public string QuoteName { get; set; }
        public string EndCustomerName { get; set; }

        public Decimal? QuoteTotal { get; set; }
       
        public string Status { get; set; }
        public DateTime? QuoteExpiryDate { get; set; }
        public DateTime? QuoteCreatedDate { get; set; }
        public DateTime? QuoteLastModified { get; set; }
    }
}
