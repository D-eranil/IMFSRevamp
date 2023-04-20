using IMFS.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IMFS.Web.Models.QuoteRateCalculation
{
    public class QuoteResponseModel
    {        
        //[JsonIgnore]
        public int?  Id { get; set; }
        public string CalculationType { get; set; }        
        public string FunderID { get; set; }
        public string Funder { get; set; }
        public string Duration { get; set; }
        public string FinanceType { get; set; }
        public string FinanceTypeID { get; set; }
        public string Frequency { get; set; }
        public decimal Rate { get; set; }
        public decimal FinanceTotal { get; set; }
        public string FunderPlanID { get; set; }
        public string FunderPlanDescription { get; set; }

    }
}

