using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace IMFS.Web.Models.QuotePercentRate
{
    public class QuotePercentRateInputExcelModel
    {
        public double? MinPercent { get; set; }
        public double? MaxPercent { get; set; }

        [DisplayName("12 months monthly")]
        public double? months12Monthly { get; set; }

        [DisplayName("12 months quarterly")]
        public double? months12Quarterly { get; set; }

        [DisplayName("12 months upfront")]
        public double? months12Upfront { get; set; }

        [DisplayName("24 months monthly")]
        public double? months24Monthly { get; set; }

        [DisplayName("24 months quarterly")]
        public double? months24Quarterly { get; set; }

        [DisplayName("24 months upfront")]
        public double? months24Upfront { get; set; }

        [DisplayName("36 months monthly")]
        public double? months36Monthly { get; set; }

        [DisplayName("36 months quarterly")]
        public double? months36Quarterly { get; set; }

        [DisplayName("36 months upfront")]
        public double? months36Upfront { get; set; }

        [DisplayName("48 months monthly")]
        public double? months48Monthly { get; set; }

        [DisplayName("48 months quarterly")]
        public double? months48Quarterly { get; set; }

        [DisplayName("48 months  upfront")]
        public double? months48Upfront { get; set; }

        [DisplayName("60 months monthly")]
        public double? months60Monthly { get; set; }

        [DisplayName("60 months quarterly")]
        public double? months60Quarterly { get; set; }

        [DisplayName("60 months upfront")]
        public double? months60Upfront { get; set; }
    }


    public class PercentRateInputExcelModelMap : ClassMap<QuotePercentRateInputExcelModel>
    {
        public PercentRateInputExcelModelMap()
        {
            Map(m => m.MinPercent).Name("MinPercent");
            Map(m => m.MaxPercent).Name("MaxPercent");

            Map(m => m.months12Monthly).Name("12 months monthly");
            Map(m => m.months12Quarterly).Name("12 months quarterly");
            Map(m => m.months12Upfront).Name("12 months upfront");

            Map(m => m.months24Monthly).Name("24 months monthly");
            Map(m => m.months24Quarterly).Name("24 months quarterly");
            Map(m => m.months24Upfront).Name("24 months upfront");

            Map(m => m.months36Monthly).Name("36 months monthly");
            Map(m => m.months36Quarterly).Name("36 months quarterly");
            Map(m => m.months36Upfront).Name("36 months upfront");

            Map(m => m.months48Monthly).Name("48 months monthly");
            Map(m => m.months48Quarterly).Name("48 months quarterly");
            Map(m => m.months48Upfront).Name("48 months upfront");

            Map(m => m.months60Monthly).Name("60 months monthly");
            Map(m => m.months60Quarterly).Name("60 months quarterly");
            Map(m => m.months60Upfront).Name("60 months upfront");
        }
    }
}
