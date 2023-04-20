using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.OTC
{
    public class OTCModel
    {
        public string Code { get; set; }
        public string QuoteId { get; set; }
    }


    public class OTCEncryptionModel
    {
        public string EncryptedQuoteId { get; set; }        
    }
}
