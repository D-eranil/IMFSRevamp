using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Quote
{
    public class QuoteSearchResponseModel: ErrorModel
    {
        public List<QuoteSearchResponse> SearchResult { get; set; }


        public QuoteSearchResponseModel()
        {
            SearchResult = new List<QuoteSearchResponse>();
        }
    }
    
    public class QuoteSearchResponse
    {
        public int? QuoteNumber { get; set; }

        public string QuoteName { get; set; }
        public string DisplayLabel { get; set; }
        public string EndCustomer { get; set; }
        public decimal? QuoteTotal { get; set; }
        public int? QuoteStatus { get; set; }
        public string QuoteStatusDescr { get; set; }
        public string QuoteFinanceType { get; set; }
        public string ResellerAccount { get; set; }
        public string ResellerName { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
