using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("Quotes")]
    public class Quotes: BaseEntity
    {
        //Quote Header Properties
        [Key]
        public int Id { get; set; }
        public string QuoteName { get; set; }        
        public DateTime QuoteCreatedDate { get; set; }
        public string QuoteCreatedBy { get; set; }
        public DateTime? QuoteLastModified { get; set; }
        
        [Column(TypeName = "date")]
        public DateTime? QuoteExpiryDate { get; set; }
        public int LatestVersion { get; set; }
        
        public decimal QuoteTotal { get; set; }
        public int QuoteStatus { get; set; }
        
        public string QuoteOrigin { get; set; }
        public DateTime LastViewedDate { get; set; }
        public string FinanceLink { get; set; }
        public string FinanceDuration { get; set; }
        public string FinanceFrequency { get; set; }
        public decimal FinanceValue { get; set; }
        public string ResellerPO { get; set; }
        public string FinanceFunder { get; set; }
        public string QuoteFinanceType { get; set; }
        public string QuoteFinanceLineType { get; set; }
        public int? FunderPlan { get; set; }

        //Reseller  Properties
        public string ResellerAccount { get; set; }
        public string ResellerName { get; set; }
        public string ResellerABN { get; set; }
        public string ResellerAddressLine1 { get; set; }
        public string ResellerAddressLine2 { get; set; }
        public string ResellerCity { get; set; }
        public string ResellerState { get; set; }
        public string ResellerPostcode { get; set; }
        public string ResellerCountry { get; set; }
        public string ResellerContactName { get; set; }
        public string ResellerContactPhone { get; set; }
        public string ResellerContactEmail { get; set; }

        //End Customer Properties
        public string EndCustomerName { get; set; }
        public string EndCustomerABN { get; set; }
        public string EndCustomerType { get; set; }
        public string EndCustomerYearsTrading { get; set; }
        public string EndCustomerPrimaryAddressLine1 { get; set; }
        public string EndCustomerPrimaryAddressLine2 { get; set; }
        public string EndCustomerPrimaryCity { get; set; }
        public string EndCustomerPrimaryState { get; set; }
        public string EndCustomerPrimaryPostcode { get; set; }
        public string EndCustomerPrimaryCountry { get; set; }
        public string EndCustomerDeliveryAddressLine1 { get; set; }
        public string EndCustomerDeliveryAddressLine2 { get; set; }
        public string EndCustomerDeliveryCity { get; set; }
        public string EndCustomerDeliveryState { get; set; }
        public string EndCustomerDeliveryPostcode { get; set; }
        public string EndCustomerDeliveryCountry { get; set; }
        public string EndCustomerContactName { get; set; }
        public string EndCustomerContactPhone { get; set; }
        public string EndCustomerContactEmail { get; set; }
        public string EndCustomerSignatoryName { get; set; }
        public string EndCustomerSignatoryPosition { get; set; }

        //Added new columns 
        public string FunderCode { get; set; } 
        public int? GstInclude { get; set; }

        //added new columns for QuoteReject Reason and QuoteReject Comment
        public string Reason { get; set; }
        public string Comment { get; set; }
        public string FunderQuote { get; set; } = "";
        public DateTime? FunderQuoteEffectiveDate { get; set; }
        public DateTime? FunderQuoteExpirationDate { get; set; }

    }
}
