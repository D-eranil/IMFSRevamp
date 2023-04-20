namespace IMFS.Web.Models.BestRates
{
    public class BestRateByPercent
    {
        public int Id { get; set; }
        public double? MinPercent { get; set; }
        public double? MaxPercent { get; set; }
        public double Value { get; set; }
        public int? QuoteDurationID { get; set; }
        public int FinanceType { get; set; }
        public int ProductType { get; set; }
        public string PaymentType { get; set; }
        public int? FunderID { get; set; }
        public int? FunderPlanID { get; set; }
    }
}
