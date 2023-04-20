using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IMFS.Web.Models.DBModel
{
    [Table("Applications")]
    public class Applications : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }

        public int ApplicationNumber { get; set; }
        public int? ApplicationStatus { get; set; }
        public int? FunderPlan { get; set; }
        public int QuoteId { get; set; }

        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string RejectedBy { get; set; }

        public bool? IsGuarantorPropertyOwner { get; set; }
        public bool? IsGuarantorSecurity { get; set; }
        public bool? IsApplicationSigned { get; set; }

        public string EntityType { get; set; }
        public string EntityTypeOther { get; set; }
        public string EntityTrustName { get; set; }
        public string EntityTrustABN { get; set; }
        public string EntityTrustType { get; set; }
        public string EntityTrustOther { get; set; }
        public string EndCustomerTradingAs { get; set; }
        public string FinanceType { get; set; }


        public string EndCustomerYearsTrading { get; set; }

        public string EndCustomerABN { get; set; }
        public string FinanceLink { get; set; }
        public string FinanceDuration { get; set; }
        public string FinanceFrequency { get; set; }
        public string EndCustomerPrimaryAddressLine1 { get; set; }
        public string EndCustomerPrimaryAddressLine2 { get; set; }
        public string EndCustomerPrimaryCity { get; set; }
        public string EndCustomerPrimaryState { get; set; }
        public string EndCustomerPrimaryCountry { get; set; }

        public decimal? FinanceValue { get; set; }
        public decimal? QuoteTotal { get; set; }
        public decimal? FinanceTotal { get; set; }
        public decimal? AveAnnualSales { get; set; }
        public decimal? GuarantorSecurityValue { get; set; }
        public decimal? GuarantorSecurityOwing { get; set; }

        public string EndCustomerDeliveryAddressLine1 { get; set; }
        public string EndCustomerDeliveryAddressLine2 { get; set; }
        public string EndCustomerDeliveryCity { get; set; }
        public string EndCustomerDeliveryState { get; set; }
        public string EndCustomerDeliveryCountry { get; set; }
        public string EndCustomerPostalAddressLine1 { get; set; }
        public string EndCustomerPostalAddressLine2 { get; set; }
        public string EndCustomerPostalCity { get; set; }
        public string EndCustomerPostalState { get; set; }
        public string EndCustomerPostalCountry { get; set; }
        public string EndCustomerPrimaryPostcode { get; set; }
        public string EndCustomerDeliveryPostcode { get; set; }
        public string EndCustomerPostalPostcode { get; set; }
        public string ResellerID { get; set; }
        public string ResellerName { get; set; }
        public string FinanceFunder { get; set; }
        public string EndCustomerType { get; set; }
        public string EndCustomerContactName { get; set; }
        public string EndCustomerContactPhone { get; set; }
        public string EndCustomerPhone { get; set; }
        public string EndCustomerFax { get; set; }
        public string EndCustomerContactEmail { get; set; }
        public string EndCustomerSignatoryName { get; set; }
        public string EndCustomerSignatoryPosition { get; set; }
        public string IMFSContactName { get; set; }
        public string IMFSContactEmail { get; set; }
        public string IMFSContactPhone { get; set; }
        public string ResellerContactName { get; set; }
        public string ResellerContactEmail { get; set; }
        public string ResellerContactPhone { get; set; }
        public string EndCustomerName { get; set; }
        public string BusinessActivity { get; set; }
        public string GoodsDescription { get; set; }
        public int? HPECreditApplicationID { get; set; }
        public int? HPERequestID { get; set; }

        public string LeaseAgreementSignorEmail { get; set; }
        public string CertificateOfAcceptanceSignorEmail { get; set; }
        public string AntiMoneyLaunderingSignorEmail { get; set; }
        public string DirectDebitACHSignorEmail { get; set; }

        public string FunderQuote { get; set; }
        public decimal? RevisedApprovedAmount { get; set; }
        public decimal? DownPaymentAmount { get; set; }
        public decimal? PaymentWithoutTax { get; set; }
        public decimal? EstimatedTaxAmount { get; set; }
        public decimal? PaymentAmount { get; set; }

        public decimal? UpFrontPayment { get; set; }
        public decimal? BankGuaranteeLetterOfCreditPcr { get; set; }
        public string TermUpperLimit { get; set; }
        public bool? ArrearsMonthlyNotPermitted { get; set; }
        public bool? ArrearsQuarterlyNotPermitted { get; set; }
        public string LeaseStructureNotPermitted { get; set; }

        public bool? Dys6090DfelInd { get; set; }
        public bool? RevisedCreditAmount { get; set; }
        public string Guarantors { get; set; }
        public string OtherCondition { get; set; }
    }
}
