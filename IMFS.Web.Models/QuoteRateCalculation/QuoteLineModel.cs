using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.QuoteRateCalculation
{
    public class QuoteLineModel
    {
        public string IMSKU { get; set; }
        public string VendorSKU { get; set; }

        public int Qty { get; set; }
        public string Vendor { get; set; }
        public string VSR { get; set; }
        public string SellPrice { get; set; }
        public double LineTotal { get; set; }
        public string ProductType { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public int LineNumber { get; set; }
        public string Description { get; set; }
        public double TotalGST { get; set; }


    }
}
