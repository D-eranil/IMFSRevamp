using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Application
{
    public class ApplicationDetailsModel
    {
        public int Id { get; set; }
        public int ApplicationNumber { get; set; }
        public int QuoteID { get; set; }
        public int Status { get; set; }
        public int? FunderPlan { get; set; }
        public decimal? FinanceValue { get; set; }
        public string FinanceType { get; set; }
        public decimal? QuoteTotal { get; set; }
        public decimal? FinanceTotal { get; set; }
        public decimal? AveAnnualSales { get; set; }
        public bool? IsGuarantorPropertyOwner { get; set; }

        public decimal? GuarantorSecurityValue { get; set; }
        public decimal? GuarantorSecurityOwing { get; set; }
        public bool? IsGuarantorSecurity { get; set; }

        public string FinanceLink { get; set; }
        public string FinanceDuration { get; set; }
        public string FinanceFrequency { get; set; }
        public string FinanceFunder { get; set; }
        public string FinanceFunderName { get; set; }
        public string FinanceFunderEmail { get; set; }

        public DateTime CreatedDate { get; set; }
        public string IMFSContactName { get; set; }
        public string IMFSContactEmail { get; set; }
        public string IMFSContactPhone { get; set; }
        public string BusinessActivity { get; set; }
        public string GoodsDescription { get; set; }
        public bool? IsApplicationSigned { get; set; }


        public string LeaseAgreementSignorEmail { get; set; } = "";
        public string CertificateOfAcceptanceSignorEmail { get; set; } = "";
        public string AntiMoneyLaunderingSignorEmail { get; set; } = "";
        public string DirectDebitACHSignorEmail { get; set; } = "";

        public string FunderQuote { get; set; } = "";
        public decimal? RevisedApprovedAmount { get; set; }
        public decimal? DownPaymentAmount { get; set; }
        public decimal? PaymentWithoutTax { get; set; }
        public decimal? EstimatedTaxAmount { get; set; }
        public decimal? PaymentAmount { get; set; }

        public decimal? UpFrontPayment { get; set; }
        public decimal? BankGuaranteeLetterOfCreditPcr { get; set; }
        public string TermUpperLimit { get; set; } = "";
        public bool? ArrearsMonthlyNotPermitted { get; set; }
        public bool? ArrearsQuarterlyNotPermitted { get; set; }
        public string LeaseStructureNotPermitted { get; set; } = "";

        //Quotes data
        public string Quote_FunderQuote { get; set; } = "";
        public DateTime? Quote_FunderQuoteEffectiveDate { get; set; }
        public DateTime? Quote_FunderQuoteExpirationDate { get; set; }


        public EntityDetails EntityDetails { get; set; }
        public EndCustomerDetails EndCustomerDetails { get; set; }
        public ResellerDetails ResellerDetails { get; set; }

        public List<ApplicationContact> ApplicationContacts { get; set; }
        public string StatusDescription { get; set; }

        public ApplicationDetailsModel()
        {
            EntityDetails = new EntityDetails();
            EndCustomerDetails = new EndCustomerDetails();
            ResellerDetails = new ResellerDetails();
            ApplicationContacts = new List<ApplicationContact>();
        }
    }

    public class EntityDetails
    {
        public string EntityType { get; set; }
        public string EntityTrustName { get; set; }
        public string EntityTrustABN { get; set; }
        public string EntityTrustType { get; set; }
        public string EntityTypeOther { get; set; }
        public string EntityTrustOther { get; set; }
    }


    public class ResellerDetails
    {
        public string ResellerID { get; set; }
        public string ResellerName { get; set; }
        public string ResellerContactName { get; set; }
        public string ResellerContactEmail { get; set; }
        public string ResellerContactPhone { get; set; }
}

    public class EndCustomerDetails
    {
        public string EndCustomerName { get; set; }
        public string EndCustomerTradingAs { get; set; }
        public string EndCustomerYearsTrading { get; set; }
        public string EndCustomerABN { get; set; }
        public string EndCustomerType { get; set; }
        public string EndCustomerContactName { get; set; }
        public string EndCustomerContactPhone { get; set; }
        public string EndCustomerContactEmail { get; set; }
        public string EndCustomerSignatoryName { get; set; }
        public string EndCustomerSignatoryPosition { get; set; }
        public string EndCustomerPhone { get; set; }
        public string EndCustomerFax { get; set; }

        //Primary Address
        public string EndCustomerPrimaryAddressLine1 { get; set; }
        public string EndCustomerPrimaryAddressLine2 { get; set; }
        public string EndCustomerPrimaryCity { get; set; }
        public string EndCustomerPrimaryState { get; set; }
        public string EndCustomerPrimaryCountry { get; set; }
        public string EndCustomerPostalPostcode { get; set; }

        //Delivery Address
        public string EndCustomerDeliveryAddressLine1 { get; set; }
        public string EndCustomerDeliveryAddressLine2 { get; set; }
        public string EndCustomerDeliveryCity { get; set; }
        public string EndCustomerDeliveryState { get; set; }
        public string EndCustomerDeliveryCountry { get; set; }
        public string EndCustomerDeliveryPostcode { get; set; }

        //Postal Address
        public string EndCustomerPostalAddressLine1 { get; set; }
        public string EndCustomerPostalAddressLine2 { get; set; }
        public string EndCustomerPostalCity { get; set; }
        public string EndCustomerPostalState { get; set; }
        public string EndCustomerPostalCountry { get; set; }
        public string EndCustomerPrimaryPostcode { get; set; }
        


    }

    public class ApplicationContact
    {
        public int ContactType { get; set; }
        public string ContactDescription { get; set; }
        public string ContactID { get; set; }
        public string ContactEmail { get; set; }
        public string ResellerID { get; set; }
        public string ContactName { get; set; }
        public DateTime? ContactDOB { get; set; }
        public string ContactAddress { get; set; }
        public string ContactDriversLicNo { get; set; }
        public string ContactABNACN { get; set; }
        public string ContactPosition { get; set; }
        public bool IsContactSignatory { get; set; }
        public string ContactPhone { get; set; }
    }


}

