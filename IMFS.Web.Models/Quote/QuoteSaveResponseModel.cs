using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Quote
{
    public class QuoteSaveResponseModel: ErrorModel
    {
        public int QuoteId { get; set; }
        public QuoteSaveResponseModel()
        {

        }
    }
}
