using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Quote
{
    public class QuoteDetailsResponseModel: ErrorModel
    {
        public QuoteDetailsModel QuoteDetails { get; set; }

        public QuoteDetailsResponseModel()
        {

        }

    }
}
