using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Email
{
    public class EmailHistory
    {
        public int EmailId { get; set; }
        public int QuoteId { get; set; }
        public int ApplicationId { get; set; }
        public int ContractId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }

    
    }
}
