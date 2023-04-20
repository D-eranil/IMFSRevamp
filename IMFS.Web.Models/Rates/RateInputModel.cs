using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Rates
{
    public class RateInputModel
    {
        public int? FunderId { get; set; }
        public int? ProductType { get; set; }
        public int? FinanceType { get; set; }
        public int? VendorId { get; set; }
        
    }
}
