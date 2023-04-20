using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Application
{
    public class ApplicationSearchModel
    {

        public int? ApplicationNumber { get; set; }
        public int? Status { get; set; }
        public string[] FinanceType { get; set; }
        public string EndCustomerName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string TriggerSource { get; set; }
        public int? ResellerId { get; set; }

    }
}
