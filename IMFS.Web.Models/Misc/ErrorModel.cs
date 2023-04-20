using IMFS.Web.Models.QuoteRateCalculation;
using System.Collections.Generic;

namespace IMFS.Web.Models.Misc
{
    public class ErrorModel
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public ErrorModel()
        {
            HasError = false;
        }
    }


    
}
