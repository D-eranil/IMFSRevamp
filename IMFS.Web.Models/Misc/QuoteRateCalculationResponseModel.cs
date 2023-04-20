using IMFS.Web.Models.QuoteRateCalculation;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Misc
{
    public class QuoteRateCalculationResponseModel: ErrorModel
    {
        public List<QuoteResponseModel> QuoteResponse { get; set; }


        public QuoteRateCalculationResponseModel()
        {
            QuoteResponse = new List<QuoteResponseModel>();
        }

    }
}
