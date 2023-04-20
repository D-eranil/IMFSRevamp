using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.QuoteRateCalculation
{
    public class QuoteRateCalculationModel
    {
        public string Source { get; set; }

        public decimal QuoteTotal { get; set; }

        public string Funder { get; set; }

        public string FunderPlan { get; set; }

        public string[] Duration { get; set; }

        public string FinanceType { get; set; }

        public string[] Frequency { get; set; }

        public bool IncludeTax { get; set; }

        public double TaxRate { get; set; }

        public int? GstInclude { get; set; }

        public List<QuoteLineModel> QuoteLines { get; set; }

    }
}
