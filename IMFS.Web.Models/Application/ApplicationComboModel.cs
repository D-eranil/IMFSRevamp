using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Application
{
    public class ApplicationResponse : ErrorModel
    {
        public int ApplicationId { get; set; }
        public string Result { get; set; }

        public bool IsHPEFSFunder { get; set; }
        public HPEAPIRequestResponseObjects GDSAPIModel { get; set; }
        public HPEAPIRequestResponseObjects GCRSAPIModel { get; set; }
        public HPEAPIRequestResponseObjects ACAPIModel { get; set; }
        public List<string> apiErrorLogs { get; set; }
    }
    public class HPEAPIRequestResponseObjects
    {
        public string Request { get; set; }
        public string Response { get; set; }
    }
    public class ConfigDataRequest
    {
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
    }

    public class ApplicationDataRequest
    {
        public int ApplicationId { get; set; }
        public string EndCustomerContactName { get; set; }
        public string EndCustomerContactEmail { get; set; }
        public string EndCustomerName { get; set; }
        public string EndCustomerPrimaryAddressLine1 { get; set; }
        public string EndCustomerPrimaryCity { get; set; }
        public string EndCustomerPrimaryState { get; set; }
        public string EndCustomerPrimaryPostcode { get; set; }
        public string EndCustomerPrimaryCountry { get; set; }
        public string EndCustomerABN { get; set; }
        public string FinanceDuration { get; set; }
        public bool DirectCustomer { get; set; } = true;
        public string EndCustomerDeliveryAddressLine1 { get; set; }
        public string EndCustomerDeliveryAddressLine2 { get; set; }
        public string EndCustomerDeliveryCity { get; set; }
        public string EndCustomerDeliveryState { get; set; }
        public string EndCustomerDeliveryPostcode { get; set; }
        public string EndCustomerDeliveryCountry { get; set; }
        public bool PublicSectorIndicator { get; set; } = true;
        //public string FinanceDuration { get; set; }
        public string FinanceType { get; set; }
        public string FinanceFrequency { get; set; }
        public int? FunderPlan { get; set; }
        public int QuoteID { get; set; }
        public string ResellerID { get; set; }

    }

    public class FunderPlanDataRequest
    {
        public string FunderPlan { get; set; }
    }

    public class QuoteDataRequest
    {
        public List<QuoteLineDataRequest> QuoteLine { get; set; }
    }

    public class CustomerAuxDataRequest
    {
        public string CustomerID { get; set; }
        public bool? AccreditationStatus { get; set; }
        public string PartnerID { get; set; }
    }

    public class QuoteLineDataRequest
    {
        public string ManufacturerPartNumber { get; set; }
        public decimal? UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;
        public string PCTCategory { get; set; } = "HPD";
        public string ProductDescription { get; set; } = "LAPTOP";
        public int BundleId { get; set; } = 0;
        public string ProductManufacturer { get; set; } = "HP";
        public string ManufacturerProductLine { get; set; } = "ELITEBOOK 840GS";
    }



    #region Request Objects

    #region CCR - combo request
    public class GetCCRequest
    {
        public GetCCRTransactionDetail TransactionDetail { get; set; }
        public GetCCRSourceEntryPerson SourceEntryPerson { get; set; }
        public GetCCRPartnerRep PartnerRep { get; set; }
        public GetCCRAPIRequestDetails GetComboAPIRequestDetails { get; set; }

    }
    public class GetCCRTransactionDetail
    {
        public string TransactionId { get; set; }
        public string ClientProgramId { get; set; }
        public long PartnerSupplierId { get; set; }
        public string PartnerSupplierName { get; set; }
        public long DistributorId { get; set; }
        public string DistributorName { get; set; }
        public string RelationshipCode { get; set; }
        public string LanguageCode { get; set; }
    }

    public class GetCCRSourceEntryPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class GetCCRPartnerRep
    {
        public string ExistingFamID { get; set; }
        public int PartnerRepID { get; set; }
        public string PartnerRepFirstName { get; set; }
        public string PartnerRepLastName { get; set; }
        public string PartnerRepPhone { get; set; }
        public string PartnerRepEmail { get; set; }
    }

    public class GetCCRAPIRequestDetails
    {
        public CreateCCRequest CreateCreditRequestRequest { get; set; }
        public CCREquipment Equipment { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
        public GetCCRFirmQuoteRequest GetFirmQuoteRequest { get; set; }
    }

    public class CreateCCRequest
    {
        public string CustomerContactFirstName { get; set; }
        public string CustomerContactemail { get; set; }
        public string CustomerLegalName { get; set; }
        public CCRCustomerAddress CustomerAddress { get; set; }
        public string CustomerTaxOrRegistrationId { get; set; }
        public string DNBNumber { get; set; }
        public string FinaceTerm { get; set; }
        public bool DirectCustomer { get; set; }
    }

    public class CCRCustomerAddress
    {
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCodeIso2 { get; set; }
    }

    public class CCREquipment
    {
        public List<CCRInstallLocation> InstallLocations { get; set; }
        public string PCTCategory { get; set; }
        public string ProductDescription { get; set; }
        public int BundleId { get; set; }
        public string ProductManufacturer { get; set; }
        public string ManufacturerProductLine { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }

    public class CCRInstallLocation
    {
        public CCRAddress Address { get; set; }
    }
    public class CCRAddress
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCodeIso2 { get; set; }
    }

    public class GetCCRFirmQuoteRequest
    {
        public bool PublicSectorIndicator { get; set; }
        public string LeaseTerm { get; set; }
        public string LeaseType { get; set; }
        public string PaymentFrequency { get; set; }
        public string ProgramType { get; set; }
        public List<CCREquipment> Equipment { get; set; }
        public string Comments { get; set; }
    }
    #endregion


    public class CreateComboRequest
    {
        public TransactionDetail TransactionDetail { get; set; }
        public TransactionResult TransactionResult { get; set; }
        public SourceEntryPerson SourceEntryPerson { get; set; }
        public PartnerRep PartnerRep { get; set; }
        public GetComboAPIRequestDetails GetComboAPIRequestDetails { get; set; }
        public string CreditApplicationID { get; set; }
        public string RequestID { get; set; }
    }

    public class TransactionDetail
    {
        public string TransactionId { get; set; } = "TEST ABC FOR Suppliers";
        public string ClientProgramId { get; set; } = "IngramTest";
        public string HPEPartyId { get; set; }
        public long PartnerSupplierId { get; set; }
        public string PartnerSupplierName { get; set; } = "Ingrammicro";
        public long DistributorId { get; set; }
        public string DistributorName { get; set; } = "";
        public string RelationshipCode { get; set; } = "HPE";
        public string ClientAuthenticationKey { get; set; }
        public string LanguageCode { get; set; } = "EN";

    }

    public class SourceEntryPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class PartnerRep
    {
        public string ExistingFamID { get; set; }
        public int PartnerRepID { get; set; } = 0;
        public string PartnerRepFirstName { get; set; }
        public string PartnerRepLastName { get; set; }
        public string PartnerRepPhone { get; set; }
        public string PartnerRepEmail { get; set; }
    }

    public class GetComboAPIRequestDetails
    {
        public CreateCreditRequestRequest CreateCreditRequestRequest { get; set; }
        public Equipment Equipment { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
        public GetFirmQuoteRequest GetFirmQuoteRequest { get; set; }
    }

    public class CreateCreditRequestRequest
    {
        public string CustomerContactFirstName { get; set; }
        public string CustomerContactLastName { get; set; }
        public string CustomerContactemail { get; set; }
        public string CustomerContactPhone { get; set; }
        public string CustomerLegalName { get; set; }
        public CustomerAddress CustomerAddress { get; set; }
        public string Phone { get; set; }
        public string CustomerTaxOrRegistrationId { get; set; }
        public string HpfsCustomerNumber { get; set; }
        public string DNBNumber { get; set; }
        public int SICCode { get; set; }
        public int NAICCode { get; set; }
        public string NumberOfEmployees { get; set; }
        public string FinaceTerm { get; set; }
        public bool DirectCustomer { get; set; }
    }

    public class CustomerAddress
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCodeIso2 { get; set; }
        public string TaxState { get; set; }

    }

    public class Equipment
    {
        public List<InstallLocations> InstallLocations { get; set; }
    }

    public class InstallLocations
    {
        public Address Address { get; set; }
    }

    public class Address
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCodeIso2 { get; set; }
        public string TaxState { get; set; }

    }

    public class GetFirmQuoteRequest
    {
        public string OfferPromotionId { get; set; }
        public string OfferPromotionDesc { get; set; }
        public bool PublicSectorIndicator { get; set; }
        public string LeaseTerm { get; set; }
        public string LeaseType { get; set; }
        public string PaymentFrequency { get; set; }
        public string ProgramType { get; set; }
        public string FinanceMargin { get; set; }
        public string HirePurchaseMargin { get; set; }
        public string FairMarketValueMargin { get; set; }
        public string LoanMargin { get; set; }
        public string MPSMargin { get; set; }
        public string SubscriptionMargin { get; set; }
        public List<FirmQuoteEquipment> Equipment { get; set; }
        public string Comments { get; set; }
    }

    public class FirmQuoteEquipment
    {
        public string Item { get; set; }
        public string PCTCategory { get; set; }
        public string ProductDescription { get; set; }
        public int BundleId { get; set; }
        public string ProductManufacturer { get; set; }
        public string ManufacturerProductLine { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public double? UnitPrice { get; set; }
        public double? Quantity { get; set; }
    }

    #endregion

    #region Response

    public class CreateComboResponse
    {
        public TransactionDetailResponse TransactionDetail { get; set; }
        public TransactionResult TransactionResult { get; set; }
        public string CreditApplicationID { get; set; }
        public string RequestID { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }

        public HPEAPIRequestResponseObjects CCRAPIModel { get; set; }
        public HPEAPIRequestResponseObjects GDSAPIModel { get; set; }
        public HPEAPIRequestResponseObjects GCRSAPIModel { get; set; }
        public HPEAPIRequestResponseObjects ACAPIModel { get; set; }
        public List<string> apiErrorLog { get; set; }
    }
    public class GetComboAPIResponseResult
    {
        public bool Error { get; set; }
        public string Message { get; set; }
    }

    public class GetComboAPIResponseType
    {
        //public TransactionDetailResponse TransactionDetail { get; set; }
        //public TransactionResult TransactionResult { get; set; }
        public string CreditApplicationID { get; set; }
        public string RequestID { get; set; }
    }

    public class TransactionDetailResponse
    {
        public string TransactionId { get; set; }
        public string ClientProgramId { get; set; }
        public string HPEPartyId { get; set; }
        public long PartnerSupplierId { get; set; }
        public string PartnerSupplierName { get; set; }
        public long DistributorId { get; set; }
        public string DistributorName { get; set; }
        public string RelationshipCode { get; set; }
        public string ClientAuthenticationKey { get; set; }
        public string LanguageCode { get; set; }
    }

    public class TransactionResult
    {
        public bool ResultSuccess { get; set; }
        public List<Validation> Validation { get; set; }
        public List<ValidationEquipmentMapping> ValidationEquipmentMapping { get; set; }
    }

    public class Validation
    {
        public string ValidationId { get; set; }
        public string ValidationText { get; set; }
    }

    public class ValidationEquipmentMapping
    {
        public ValidationEquipmentMessageType ValidationEquipmentMessageType { get; set; }
    }
    public class ValidationEquipmentMessageType
    {
        public string ValidationId { get; set; }
        public string ValidationText { get; set; }
    }

    public class ApplicationHPEUpdateRequest
    {
        public int ApplicationID { get; set; }
        public int QuoteId { get; set; }
        public string? CreditApplicationID { get; set; }
        public string? RequestID { get; set; }
    }


    #endregion

    #region Get Combo Request Response Status

    //Request
    public class GetComboRequest
    {
        public TransactionDetailGCRS TransactionDetail { get; set; }
        public ComboInformation ComboInformation { get; set; }
        public int ApplicationId { get; set; }
        public int QuoteId { get; set; }

    }

    public class TransactionDetailGCRS
    {
        public string TransactionId { get; set; }
        public string ClientProgramId { get; set; }
        public long PartnerSupplierId { get; set; }
        public string PartnerSupplierName { get; set; }
        public long DistributorId { get; set; }
        public string DistributorName { get; set; }
        public string RelationshipCode { get; set; }
        public string LanguageCode { get; set; }

    }

    public class ComboInformation
    {
        public CIFSourceEntryPerson SourceEntryPerson { get; set; }
        public int CreditApplicationID { get; set; }
        public int RequestID { get; set; }
    }

    public class CIFSourceEntryPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

    }

    //Response

    public class GetComboResponse
    {
        public GetFirmQuoteResponse GetFirmQuoteResponse { get; set; }
        public ComboInformationResponse ComboInformationResponse { get; set; }
        //public string CreditApplDocDetailType { get; set; }
        public ConditionDetails ConditionDetails { get; set; }
        public double RevisedApprovedAmount { get; set; }
        public string RequestAmountCurrency { get; set; }
        public int QuoteId { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
        public HPEAPIRequestResponseObjects GCRSApiModel { get; set; }
    }

    public class AlternateDocumentList
    {
        public AlternateDocumentType AlternateDocumentType { get; set; }
    }

    public class AlternateDocumentType
    {
        public int AlternateDocumentID { get; set; }
        public string AlternateDocumentDesc { get; set; }
    }

    public class ComboInformationResponse
    {
        public SourceEntryPerson SourceEntryPerson { get; set; }
        public int RequestID { get; set; }
        public int CreditApplicationID { get; set; }
        public string CreditDecisionCode { get; set; }
        public string CreditDecisionDescription { get; set; }
    }

    public class ConditionDetails
    {
        public int UpFrontPayment { get; set; }
        public int BankGuaranteeLetterOfCreditPcr { get; set; }
        public bool ArrearsMonthlyNotPermitted { get; set; }
        public bool ArrearsQuarterlyNotPermitted { get; set; }
        public bool RevisedCreditAmount { get; set; }
        public string TermUpperLimit { get; set; }
        public string LeaseStructureNotPermitted { get; set; }

        public bool? Dys6090DfelInd { get; set; }
        public string Guarantors { get; set; }
        public string OtherCondition { get; set; }
    }

    public class DocumentSignatureRequiredActions
    {
        public string LeaseAgreementSignorEmail { get; set; }
        public string CertificateOfAcceptanceSignorEmail { get; set; }
        public string AntiMoneyLaunderingSignorEmail { get; set; }
        public string DirectDebitACHSignorEmail { get; set; }
    }

    public class FirmQuoteLeasePaymentOptionType
    {
        public int QuoteNumber { get; set; }
        public int CreditApplicationNumber { get; set; }
        public string Type { get; set; }
        public string Program { get; set; }
        public string PaymentTerm { get; set; }
        public int LeaseTerm { get; set; }
        public double PaymentWithoutTax { get; set; }
        public int EstimatedTaxAmount { get; set; }
        public double PaymentAmount { get; set; }
        public int DownPaymentAmount { get; set; }
        public string PassThrough { get; set; }
        public string QuotedStatus { get; set; }
        public string QuoteEffectiveDate { get; set; }
        public string QuoteExpirationDate { get; set; }
        public int MarginUpliftPercentage { get; set; }
    }

    public class GetDetailedPricingResponseDetail
    {
        public int? PricingRequestId { get; set; }
        public List<LeasePaymentOptions> LeasePaymentOptions { get; set; }
    }

    public class GetFirmQuoteResponse
    {
        public TransactionDetail TransactionDetail { get; set; }
        public TransactionResult TransactionResult { get; set; }
        //public AlternateDocumentList AlternateDocumentList { get; set; }
        public DocumentSignatureRequiredActions DocumentSignatureRequiredActions { get; set; }
        public GetDetailedPricingResponseDetail GetDetailedPricingResponseDetail { get; set; }
    }

    public class LeasePaymentOptions
    {
        public int QuoteNumber { get; set; }
        public int CreditApplicationNumber { get; set; }
        public string Type { get; set; }
        public string Program { get; set; }
        public string PaymentTerm { get; set; }
        public int LeaseTerm { get; set; }
        public double PaymentWithoutTax { get; set; }
        public int EstimatedTaxAmount { get; set; }
        public double PaymentAmount { get; set; }
        public int DownPaymentAmount { get; set; }
        public string PassThrough { get; set; }
        public string QuotedStatus { get; set; }
        public DateTime? QuoteEffectiveDate { get; set; }
        public DateTime? QuoteExpirationDate { get; set; }
        public int MarginUpliftPercentage { get; set; }
        //public FirmQuoteLeasePaymentOptionType FirmQuoteLeasePaymentOptionType { get; set; }
    }


    #endregion

    #region Get Combo Accept Quote

    //request
    public class AcceptQuoteRequest
    {
        public AcceptQuoteTransactionDetail TransactionDetail { get; set; }
        public AcceptQuoteSourceEntryPerson SourceEntryPerson { get; set; }
        public int CreditApplicationID { get; set; }
        public int FamID { get; set; }
        public int QuoteNumber { get; set; }
        public bool DocumenttoCustomer { get; set; }
        public bool PaymentDelegationFlag { get; set; }
        public bool UseAdobeEsign { get; set; }
        public CustomerSignatoryApprover CustomerSignatoryApprover { get; set; }
        public DocumentSignatureRequiredActions DocumentSignatureRequiredActions { get; set; }
        public int ApplicationId { get; set; }
        public int QuoteId { get; set; }
    }

    public class AcceptQuoteTransactionDetail
    {
        public string TransactionId { get; set; }
        public string ClientProgramId { get; set; }
        public long PartnerSupplierId { get; set; }
        public string PartnerSupplierName { get; set; }
        public long DistributorId { get; set; }
        public string DistributorName { get; set; }
        public string RelationshipCode { get; set; }
        public string LanguageCode { get; set; }

    }

    public class AcceptQuoteSourceEntryPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class CustomerSignatoryApprover
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
    }

    //response
    public class AcceptQuoteResponseType
    {
        public TransactionDetailAC TransactionDetail { get; set; }
        public TransactionResultAC TransactionResult { get; set; }
        public DocumentSignatureActionsProcessedAC DocumentSignatureActionsProcessed { get; set; }
        public bool DocGenResult { get; set; }
        public string DocGenMessage { get; set; }
    }

    public class DocumentSignatureActionsProcessedAC
    {
        public string LeaseAgreementSignorEmail { get; set; }
        public string CertificateOfAcceptanceSignorEmail { get; set; }
        public string AntiMoneyLaunderingSignorEmail { get; set; }
        public string DirectDebitACHSignorEmail { get; set; }
        public string LeaseAgreementSignorCCEmail { get; set; }
        public string CertificateOfAcceptanceSignorCCEmail { get; set; }
        public string AntiMoneyLaunderingSignorCCEmail { get; set; }
        public string DirectDebitACHSignorCCEmail { get; set; }
    }

    public class AcceptQuoteResponse
    {
        public AcceptQuoteResponseType AcceptQuoteResponseType { get; set; }
        public int QuoteId { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
        public HPEAPIRequestResponseObjects ACAPIModel { get; set; }
    }

    public class TransactionDetailAC
    {
        public string TransactionId { get; set; }
        public string ClientProgramId { get; set; }
        public string NilTrue { get; set; }
        public long PartnerSupplierId { get; set; }
        public string PartnerSupplierName { get; set; }
        public long DistributorId { get; set; }
        public string DistributorName { get; set; }
        public string RelationshipCode { get; set; }
        public string ClientAuthenticationKey { get; set; }
        public string LanguageCode { get; set; }
    }

    public class TransactionResultAC
    {
        public bool ResultSuccess { get; set; }
        public List<ValidationAC> Validation { get; set; }
        public List<ValidationEquipmentMapping> ValidationEquipmentMapping { get; set; }
    }

    public class ValidationAC
    {
        public string ValidationId { get; set; }
        public string ValidationText { get; set; }
    }

    #endregion

    #region Get Deal Status Request

    //request
    public class GetDealStatusRequest
    {
        public GetDealStatusTransactionDetail TransactionDetail { get; set; }
        public GetDealStatusSourceEntryPerson SourceEntryPerson { get; set; }
        public GetDealStatusCreditApplicationInfo CreditApplicationInfo { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }
    public class GetDealStatusTransactionDetail
    {
        public string TransactionId { get; set; }
        public string ClientProgramId { get; set; }
        public long PartnerSupplierId { get; set; }
        public string PartnerSupplierName { get; set; }
        public long DistributorId { get; set; }
        public string DistributorName { get; set; }
        public string RelationshipCode { get; set; }
        public string ClientAuthenticationKey { get; set; }
        public string LanguageCode { get; set; }

    }
    public class GetDealStatusSourceEntryPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
    public class GetDealStatusCreditApplicationInfo
    {
        public int CreditApplicationID { get; set; }
    }

    //response
    public class GetDealStatusResponse
    {
        public TransactionDetailGDS TransactionDetail { get; set; }
        public TransactionResultGDS TransactionResult { get; set; }
        public IList<GetDealStatusList> GetDealStatusList { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
    }

    public class GDSResponseForFE
    {
        public HPEAPIRequestResponseObjects GDSAPIModel { get; set; }
        public HPEAPIRequestResponseObjects GCRSAPIModel { get; set; }
        public HPEAPIRequestResponseObjects ACAPIModel { get; set; }
    }
    public class GetDealStatusAPIResponse
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        public int UpdatedApplicationCount { get; set; }
        public List<GDSResponseForFE> apiResponseLogs { get; set; }
        public HPEAPIRequestResponseObjects GDSAPIModel { get; set; }
        public HPEAPIRequestResponseObjects GCRSAPIModel { get; set; }
        public HPEAPIRequestResponseObjects ACAPIModel { get; set; }
        public List<string> apiErrorLogs { get; set; }
    }
    public class TransactionDetailGDS
    {
        public string TransactionId { get; set; }
        public string ClientProgramId { get; set; }
        public long? PartnerSupplierId { get; set; }
        public long? DistributorId { get; set; }
        public int ApplicationID { get; set; }
        public string ClientID { get; set; }
        public int? CreditID { get; set; }
        public string RelationshipCode { get; set; }
        public string ClientAuthenticationKey { get; set; }
        public string LanguageCode { get; set; }
    }
    public class TransactionResultGDS
    {
        public bool ResultSuccess { get; set; }
        public List<ValidationGDS> Validation { get; set; }
        public List<ValidationEquipmentMapping> ValidationEquipmentMapping { get; set; }
    }
    public class ValidationGDS
    {
        public string ValidationId { get; set; }
        public string ValidationText { get; set; }
    }
    public class GetDealStatusList
    {
        public string ApplicationRef { get; set; }
        public string PartnerName { get; set; }
        public long PartnerID { get; set; }
        public string DistiOrCallCentreName { get; set; }
        public string DistiOrCallCentreID { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByEmail { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public string StatusUpdateTimestamp { get; set; }
        public string NextAction { get; set; }
        public string RequestedDealAmount { get; set; }
        public string Currency { get; set; }
        public int WebServiceID { get; set; }
        public string SpecialFirmQuote { get; set; }
        public string HPEFSPONumber { get; set; }
    }
    #endregion

    #region Get e-sign status

    //Request
    public class GetEsignStatusRequest
    {
        public GetESignTransactionDetail TransactionDetail { get; set; }
        public GetESignSourceEntryPerson SourceEntryPerson { get; set; }
        public GetESignCreditApplicationInfo CreditApplicationInfo { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
    public class GetESignTransactionDetail
    {
        public string TransactionId { get; set; }
        public string ClientProgramId { get; set; }
        public long PartnerSupplierId { get; set; }
        public string PartnerSupplierName { get; set; }
        public long DistributorId { get; set; }
        public string DistributorName { get; set; }
        public string RelationshipCode { get; set; }
        public string ClientAuthenticationKey { get; set; }
        public string LanguageCode { get; set; }
    }
    public class GetESignSourceEntryPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class GetESignCreditApplicationInfo
    {
        public int CreditApplicationID { get; set; }
    }

    //Response
    public class GetESignStatusResponse
    {
        public GetESignTransactionResponseDetail TransactionDetail { get; set; }
        public GetESignTransactionResult TransactionResult { get; set; }
        public List<GetEsignStatus> GetEsignStatus { get; set; }

        public bool Error { get; set; }
        public string Message { get; set; }
    }
    public class GetESignTransactionResponseDetail
    {
        public string TransactionId { get; set; }
        public string ClientProgramId { get; set; }
        public long? HPEPartyId { get; set; }
        public long PartnerSupplierId { get; set; }
        public string PartnerSupplierName { get; set; }
        public long DistributorId { get; set; }
        public string DistributorName { get; set; }
        public string RelationshipCode { get; set; }
        public string ClientAuthenticationKey { get; set; }
        public string LanguageCode { get; set; }
    }

    public class GetESignTransactionResult
    {
        public bool ResultSuccess { get; set; }
        public List<object> Validation { get; set; }
    }
    public class GetEsignStatus
    {
        public string ApplicationRef { get; set; }
        public string PartnerName { get; set; }
        public string PartnerId { get; set; }
        public string DistiOrCallCentreName { get; set; }
        public string DistiOrCallCentreID { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByEmail { get; set; }
        public string CustomerName { get; set; }
        public string CreditAppStatus { get; set; }
        public string Information { get; set; }
        public List<EsignDocumentStatus> EsignDocumentStatus { get; set; }
    }
    public class EsignDocumentStatus
    {
        public string DocumentName { get; set; }
        public string DocumentStatus { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Comment { get; set; }
        public List<DocumentActivity> DocumentActivity { get; set; }
    }

    public class DocumentActivity
    {
        public string Activity { get; set; }
        public string ActivityStatus { get; set; }
        public string ActivityDate { get; set; }
    }
    #endregion

}

