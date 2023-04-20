using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Application
{

    public class ApplicationSearchResponseModel : ErrorModel
    {
        public List<ApplicationSearchResponse> SearchResult { get; set; }


        public ApplicationSearchResponseModel()
        {
            SearchResult = new List<ApplicationSearchResponse>();
        }
    }

    public class ApplicationSearchResponse
    {
        public int Id { get; set; }
        public int? ApplicationNumber { get; set; }

        public string FinanceType { get; set; }
        public string FinanceTypeName { get; set; }
        public string DisplayLabel { get; set; }
        public string EndCustomerName { get; set; }
        public decimal? QuoteTotal { get; set; }
        public int? Status { get; set; }
        public string StatusDescr { get; set; }       

        public DateTime? CreatedDate { get; set; }
        public string ResellerId { get; set; }
        public string ResellerName { get; set; }
    }

}
