using IMFS.DataAccess.Repository;
using IMFS.Web.Models.Application;
using IMFS.Services.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using IMFS.Web.Models.DBModel;
using Microsoft.Extensions.Configuration;
using System.IO;
using iTextSharp.text.pdf;
using IMFS.Core;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using IMFS.BusinessLogic.Log;
using Microsoft.Extensions.Primitives;
using System.Drawing;
using IMFS.BusinessLogic.Emails;
using System.Net.Mail;
using System.Text.RegularExpressions;
using IMFS.Web.Models.Enums;

namespace IMFS.BusinessLogic.ApplicationManagement
{
    public class ApplicationManager : IApplicationManager
    {
        private readonly IIMFSEmailService _emailService;
        private IEmailManager _emailManager;

        private readonly IRepository<Web.Models.DBModel.Applications> _applicationsRepository;
        private readonly IRepository<Web.Models.DBModel.FinanceType> _financeTypesRepository;
        private readonly IRepository<Web.Models.DBModel.Status> _statusRepository;
        private readonly IRepository<Web.Models.DBModel.ContactsXref> _contactsXrefRepository;
        private readonly IRepository<Web.Models.DBModel.Contacts> _contactsRepository;
        private readonly IRepository<Web.Models.DBModel.Funder> _fundersRepository;
        private readonly IRepository<Web.Models.DBModel.ContactsTypes> _contactTypesRepository;
        private readonly IRepository<Web.Models.DBModel.Quotes> _quotesRepository;

        private readonly IRepository<Web.Models.DBModel.Config> _configRepository;
        private readonly IRepository<Web.Models.DBModel.CustomerAux> _customerAuxRepository;
        private readonly IRepository<Web.Models.DBModel.FunderPlan> _funderPlanRepository;
        private readonly IRepository<Web.Models.DBModel.QuoteLines> _quoteLinesRepository;
        private readonly IRepository<Web.Models.DBModel.ProductXref> _productXrefRepository;

        private readonly Quote.IQuoteManager _quoteManager;
        private readonly IIMFSLogManager _imfsLogManager;
        public IConfiguration Configuration { get; }

        public ApplicationManager(IRepository<Web.Models.DBModel.Applications> applicationsRepository,
            IIMFSEmailService emailService,
            IRepository<Web.Models.DBModel.FinanceType> financeTypesRepository,
            IRepository<Web.Models.DBModel.Status> statusRepository,
            IRepository<Web.Models.DBModel.ContactsXref> contactsXrefRepository,
            IRepository<Web.Models.DBModel.Contacts> contactsRepository,
            IRepository<Web.Models.DBModel.Funder> fundersRepository,
            IRepository<Web.Models.DBModel.ContactsTypes> contactTypesRepository,
            IRepository<Web.Models.DBModel.Quotes> quotesRepository,
            IRepository<Config> configRepository,
            IRepository<Web.Models.DBModel.CustomerAux> customerAuxRepository,
            IRepository<Web.Models.DBModel.FunderPlan> funderPlanRepository,
            IRepository<Web.Models.DBModel.QuoteLines> quoteLinesRepository,
            IRepository<Web.Models.DBModel.ProductXref> productXrefRepository,
            Quote.IQuoteManager quoteManager,
            IEmailManager emailManager,
        IConfiguration configuration,
        IIMFSLogManager imfsLogManager)
        {
            _applicationsRepository = applicationsRepository;
            _financeTypesRepository = financeTypesRepository;
            _statusRepository = statusRepository;
            _contactsXrefRepository = contactsXrefRepository;
            _contactsRepository = contactsRepository;
            _fundersRepository = fundersRepository;
            _contactTypesRepository = contactTypesRepository;
            _quotesRepository = quotesRepository;
            _configRepository = configRepository;
            _customerAuxRepository = customerAuxRepository;
            _funderPlanRepository = funderPlanRepository;
            _quoteLinesRepository = quoteLinesRepository;
            _quoteManager = quoteManager;
            Configuration = configuration;
            _imfsLogManager = imfsLogManager;
            _emailService = emailService;
            _productXrefRepository = productXrefRepository;
            _emailManager = emailManager;
        }

        public ApplicationDetailsResponseModel GetApplicationDetails(string applicationId)
        {
            var response = new ApplicationDetailsResponseModel();
            try
            {
                var inputApplicationId = Convert.ToInt32(applicationId);
                var status = (from st in _statusRepository.Table select st).ToList();

                ApplicationDetailsModel applicationDetailsModel = new ApplicationDetailsModel();
                var existingApplication = _applicationsRepository.Table.Where(a => a.Id == inputApplicationId).FirstOrDefault();
                if (existingApplication != null)
                {
                    applicationDetailsModel.Id = existingApplication.Id;
                    applicationDetailsModel.ApplicationNumber = existingApplication.ApplicationNumber;
                    applicationDetailsModel.AveAnnualSales = existingApplication.AveAnnualSales;
                    applicationDetailsModel.BusinessActivity = existingApplication.BusinessActivity;
                    applicationDetailsModel.CreatedDate = existingApplication.CreateDate;
                    applicationDetailsModel.FinanceDuration = existingApplication.FinanceDuration;
                    applicationDetailsModel.FinanceFrequency = existingApplication.FinanceFrequency;
                    applicationDetailsModel.FinanceFunder = existingApplication.FinanceFunder;
                    if (!string.IsNullOrEmpty(existingApplication.FinanceFunder))
                    {
                        int tempFunderId;
                        int.TryParse(existingApplication.FinanceFunder, out tempFunderId);

                        var funderRecord = _fundersRepository.GetById(Convert.ToInt32(tempFunderId));
                        if (funderRecord != null)
                        {
                            applicationDetailsModel.FinanceFunderName = funderRecord.FunderName;
                            applicationDetailsModel.FinanceFunderEmail = funderRecord.ContactEmailAdd;
                        }
                    }

                    applicationDetailsModel.FinanceLink = existingApplication.FinanceLink;
                    applicationDetailsModel.FinanceTotal = existingApplication.FinanceTotal;
                    applicationDetailsModel.FinanceType = existingApplication.FinanceType;
                    applicationDetailsModel.FinanceValue = existingApplication.FinanceValue;
                    if (existingApplication.FunderPlan.HasValue)
                        applicationDetailsModel.FunderPlan = existingApplication.FunderPlan.Value;

                    applicationDetailsModel.GoodsDescription = existingApplication.GoodsDescription;
                    applicationDetailsModel.GuarantorSecurityOwing = existingApplication.GuarantorSecurityOwing;
                    applicationDetailsModel.GuarantorSecurityValue = existingApplication.GuarantorSecurityValue;
                    applicationDetailsModel.IMFSContactEmail = existingApplication.IMFSContactEmail;
                    applicationDetailsModel.IMFSContactName = existingApplication.IMFSContactName;
                    applicationDetailsModel.IMFSContactPhone = existingApplication.IMFSContactPhone;
                    applicationDetailsModel.IsApplicationSigned = existingApplication.IsApplicationSigned;
                    applicationDetailsModel.IsGuarantorPropertyOwner = existingApplication.IsGuarantorPropertyOwner;
                    applicationDetailsModel.IsGuarantorSecurity = existingApplication.IsGuarantorSecurity;
                    applicationDetailsModel.QuoteID = existingApplication.QuoteId;
                    applicationDetailsModel.QuoteTotal = existingApplication.QuoteTotal;
                    if (existingApplication.ApplicationStatus.HasValue)
                    {
                        applicationDetailsModel.Status = existingApplication.ApplicationStatus.Value;
                        applicationDetailsModel.StatusDescription = status.Where(st => st.Id == existingApplication.ApplicationStatus.Value).FirstOrDefault().Description;
                    }

                    applicationDetailsModel.LeaseAgreementSignorEmail = string.IsNullOrEmpty(existingApplication.LeaseAgreementSignorEmail) ? "" : existingApplication.LeaseAgreementSignorEmail;
                    applicationDetailsModel.CertificateOfAcceptanceSignorEmail = string.IsNullOrEmpty(existingApplication.CertificateOfAcceptanceSignorEmail) ? "" : existingApplication.CertificateOfAcceptanceSignorEmail;
                    applicationDetailsModel.AntiMoneyLaunderingSignorEmail = string.IsNullOrEmpty(existingApplication.AntiMoneyLaunderingSignorEmail) ? "" : existingApplication.AntiMoneyLaunderingSignorEmail;
                    applicationDetailsModel.DirectDebitACHSignorEmail = string.IsNullOrEmpty(existingApplication.DirectDebitACHSignorEmail) ? "" : existingApplication.DirectDebitACHSignorEmail;

                    applicationDetailsModel.FunderQuote = string.IsNullOrEmpty(existingApplication.FunderQuote) ? "" : existingApplication.FunderQuote;
                    applicationDetailsModel.RevisedApprovedAmount = existingApplication.RevisedApprovedAmount;
                    applicationDetailsModel.DownPaymentAmount = existingApplication.DownPaymentAmount;
                    applicationDetailsModel.PaymentWithoutTax = existingApplication.PaymentWithoutTax;
                    applicationDetailsModel.EstimatedTaxAmount = existingApplication.EstimatedTaxAmount;
                    applicationDetailsModel.PaymentAmount = existingApplication.PaymentAmount;
                    applicationDetailsModel.UpFrontPayment = existingApplication.UpFrontPayment;
                    applicationDetailsModel.BankGuaranteeLetterOfCreditPcr = existingApplication.BankGuaranteeLetterOfCreditPcr;
                    applicationDetailsModel.TermUpperLimit = string.IsNullOrEmpty(existingApplication.TermUpperLimit) ? "" : existingApplication.TermUpperLimit;
                    applicationDetailsModel.ArrearsMonthlyNotPermitted = existingApplication.ArrearsMonthlyNotPermitted;
                    applicationDetailsModel.ArrearsQuarterlyNotPermitted = existingApplication.ArrearsQuarterlyNotPermitted;
                    applicationDetailsModel.LeaseStructureNotPermitted = string.IsNullOrEmpty(existingApplication.LeaseStructureNotPermitted) ? "" : existingApplication.LeaseStructureNotPermitted;

                    //Quotes details, to display on application document tab
                    if (existingApplication.QuoteId > 0)
                    {
                        Quotes quote = _quotesRepository.GetById(existingApplication.QuoteId);
                        if (quote != null)
                        {
                            applicationDetailsModel.Quote_FunderQuote = quote.FunderQuote;
                            applicationDetailsModel.Quote_FunderQuoteEffectiveDate = quote.FunderQuoteEffectiveDate;
                            applicationDetailsModel.Quote_FunderQuoteExpirationDate = quote.FunderQuoteExpirationDate;
                        }
                    }

                    //Reseller Details
                    applicationDetailsModel.ResellerDetails.ResellerContactEmail = existingApplication.ResellerContactEmail;
                    applicationDetailsModel.ResellerDetails.ResellerContactName = existingApplication.ResellerContactName;
                    applicationDetailsModel.ResellerDetails.ResellerContactPhone = existingApplication.ResellerContactPhone;
                    applicationDetailsModel.ResellerDetails.ResellerID = existingApplication.ResellerID;
                    applicationDetailsModel.ResellerDetails.ResellerName = existingApplication.ResellerName;
                    applicationDetailsModel.ResellerDetails.ResellerContactEmail = existingApplication.ResellerContactEmail;

                    //Entity Details
                    applicationDetailsModel.EntityDetails.EntityTrustABN = existingApplication.EntityTrustABN;
                    applicationDetailsModel.EntityDetails.EntityTrustName = existingApplication.EntityTrustName;
                    applicationDetailsModel.EntityDetails.EntityTrustOther = existingApplication.EntityTrustOther;
                    applicationDetailsModel.EntityDetails.EntityTrustType = existingApplication.EntityTrustType;
                    applicationDetailsModel.EntityDetails.EntityType = existingApplication.EntityType;
                    applicationDetailsModel.EntityDetails.EntityTypeOther = existingApplication.EntityTypeOther;

                    //End Customer Details
                    applicationDetailsModel.EndCustomerDetails.EndCustomerABN = existingApplication.EndCustomerABN;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerContactEmail = existingApplication.EndCustomerContactEmail;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerContactName = existingApplication.EndCustomerContactName;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerContactPhone = existingApplication.EndCustomerContactPhone;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerDeliveryAddressLine1 = existingApplication.EndCustomerDeliveryAddressLine1;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerDeliveryAddressLine2 = existingApplication.EndCustomerDeliveryAddressLine2;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerDeliveryCity = existingApplication.EndCustomerDeliveryCity;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerDeliveryCountry = existingApplication.EndCustomerDeliveryCountry;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerDeliveryPostcode = existingApplication.EndCustomerDeliveryPostcode;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerDeliveryState = existingApplication.EndCustomerDeliveryState;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerFax = existingApplication.EndCustomerFax;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerName = existingApplication.EndCustomerName;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerPhone = existingApplication.EndCustomerPhone;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerPostalAddressLine1 = existingApplication.EndCustomerPostalAddressLine1;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerPostalAddressLine2 = existingApplication.EndCustomerPostalAddressLine2;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerPostalCity = existingApplication.EndCustomerPostalCity;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerPostalCountry = existingApplication.EndCustomerPostalCountry;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerPostalPostcode = existingApplication.EndCustomerPostalPostcode;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerPostalState = existingApplication.EndCustomerPostalState;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerPrimaryAddressLine1 = existingApplication.EndCustomerPrimaryAddressLine1;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerPrimaryAddressLine2 = existingApplication.EndCustomerPrimaryAddressLine2;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerPrimaryCity = existingApplication.EndCustomerPrimaryCity;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerPrimaryCountry = existingApplication.EndCustomerPrimaryCountry;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerPrimaryPostcode = existingApplication.EndCustomerPrimaryPostcode;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerPrimaryState = existingApplication.EndCustomerPrimaryState;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerSignatoryName = existingApplication.EndCustomerSignatoryName;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerSignatoryPosition = existingApplication.EndCustomerSignatoryPosition;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerTradingAs = existingApplication.EndCustomerTradingAs;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerType = existingApplication.EndCustomerType;
                    applicationDetailsModel.EndCustomerDetails.EndCustomerYearsTrading = existingApplication.EndCustomerYearsTrading;

                    //Get Application Contacts
                    var applicationContacts = GetApplicationContacts(existingApplication.ApplicationNumber);

                    if (applicationContacts.Count > 0)
                        applicationDetailsModel.ApplicationContacts = applicationContacts;

                    response.ApplicationDetails = applicationDetailsModel;
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        private List<ApplicationContact> GetApplicationContacts(int applicationId)
        {
            var applicationContacts = new List<ApplicationContact>();
            try
            {
                var contactsXref = _contactsXrefRepository.Table.Where(c => c.ApplicationNumber == applicationId).ToList();
                foreach (var xref in contactsXref)
                {
                    var contact = _contactsRepository.Table.Where(c => c.ContactID == xref.ContactID).FirstOrDefault();
                    if (contact != null)
                    {
                        var applicationContact = new ApplicationContact();
                        applicationContact.ContactType = contact.ContactType;
                        if (contact.ContactType > 0)
                        {
                            var contactType = _contactTypesRepository.GetById(contact.ContactType);
                            if (contactType != null)
                                applicationContact.ContactDescription = contactType.ContactDescription;
                        }
                        applicationContact.ContactID = contact.ContactID;
                        applicationContact.ContactEmail = contact.ContactEmail;
                        applicationContact.ResellerID = contact.ResellerID;
                        applicationContact.ContactName = contact.ContactName;

                        applicationContact.ContactDOB = contact.ContactDOB;
                        applicationContact.ContactAddress = contact.ContactAddress;
                        applicationContact.ContactDriversLicNo = contact.ContactDriversLicNo;
                        applicationContact.ContactABNACN = contact.ContactABNACN;
                        applicationContact.ContactPosition = contact.ContactPosition;
                        applicationContact.IsContactSignatory = contact.IsContactSignatory;
                        applicationContact.ContactPhone = contact.ContactPhone;

                        //Add to the list
                        applicationContacts.Add(applicationContact);

                    }
                }

            }
            catch (Exception ex)
            {

            }
            return applicationContacts;
        }

        public ApplicationSearchResponseModel LookupApplicationNumber(ApplicationSearchModel appSearchModel, string resellerId)
        {
            var response = new ApplicationSearchResponseModel();
            try
            {
                var query = from apps in _applicationsRepository.Table select apps;
                if (appSearchModel.FromDate != null && appSearchModel.ToDate != null)
                {
                    var fromDate = appSearchModel.FromDate.Value.ToLocalTime();
                    var toDate = appSearchModel.ToDate.Value.ToLocalTime();

                    query = query.Where(q => q.CreateDate >= fromDate && q.CreateDate <= toDate);
                }

                var financeTypes = appSearchModel.FinanceType;

                if (financeTypes != null && financeTypes.Length > 0)
                {
                    query = query.Where(q => financeTypes.Contains(q.FinanceType));
                }

                if (appSearchModel.Status != null)
                {
                    query = query.Where(q => q.ApplicationStatus == appSearchModel.Status);
                }

                if (!string.IsNullOrEmpty(appSearchModel.EndCustomerName))
                {
                    query = query.Where(q => q.EndCustomerName.Contains(appSearchModel.EndCustomerName));
                }

                //Search Application Number with contains               
                if (appSearchModel.ApplicationNumber != null)
                {
                    string filter = Convert.ToString(appSearchModel.ApplicationNumber);
                    //if (appSearchModel.TriggerSource == "lookup")
                    query = query.Where(q => q.ApplicationNumber.ToString().Contains(filter));
                    //else
                    //    query = query.Where(q => q.ApplicationNumber == appSearchModel.ApplicationNumber);
                }

                if (appSearchModel.ResellerId != null && appSearchModel.ResellerId > 0)
                    resellerId = Convert.ToString(appSearchModel.ResellerId);

                if (!string.IsNullOrEmpty(resellerId))
                {
                    query = query.Where(q => q.ResellerID == resellerId);
                }

                //Status
                var status = (from st in _statusRepository.Table select st).ToList();
                var financeTypesList = (from fin in _financeTypesRepository.Table select fin).ToList();

                var results = query.Select(q => new ApplicationSearchResponse
                {
                    Id = q.Id,
                    ApplicationNumber = q.ApplicationNumber,
                    EndCustomerName = q.EndCustomerName,
                    FinanceType = (!string.IsNullOrEmpty(q.FinanceType)) ? q.FinanceType : string.Empty,
                    ResellerId = q.ResellerID,
                    ResellerName = q.ResellerName,
                    Status = q.ApplicationStatus,
                    QuoteTotal = q.QuoteTotal,
                    CreatedDate = q.CreateDate
                }).ToList();

                foreach (var result in results)
                {
                    result.StatusDescr = status.Where(st => st.Id == result.Status).FirstOrDefault().Description;
                    var finType = financeTypesList.Where(st => Convert.ToString(st.QuoteDurationType) == result.FinanceType).FirstOrDefault();
                    if (finType != null)
                        result.FinanceTypeName = finType.Description;

                }

                //Set the response
                response.SearchResult = results;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public ApplicationSearchResponseModel SearchApplication(ApplicationSearchModel appSearchModel, string resellerId)
        {
            var response = new ApplicationSearchResponseModel();
            try
            {
                var query = from apps in _applicationsRepository.Table select apps;

                if (appSearchModel.FromDate != null && appSearchModel.ToDate != null)
                {
                    var fromDate = appSearchModel.FromDate.Value.ToLocalTime();
                    var toDate = appSearchModel.ToDate.Value.ToLocalTime();

                    query = query.Where(q => q.CreateDate >= fromDate && q.CreateDate <= toDate);
                }

                var financeTypes = appSearchModel.FinanceType;

                if (financeTypes != null && financeTypes.Length > 0)
                {
                    query = query.Where(q => financeTypes.Contains(q.FinanceType));
                }

                if (appSearchModel.Status != null)
                {
                    query = query.Where(q => q.ApplicationStatus == appSearchModel.Status);
                }

                if (!string.IsNullOrEmpty(appSearchModel.EndCustomerName))
                {
                    query = query.Where(q => q.EndCustomerName.Contains(appSearchModel.EndCustomerName));
                }

                if (appSearchModel.ApplicationNumber != null)
                {
                    query = query.Where(q => q.ApplicationNumber == appSearchModel.ApplicationNumber);
                }

                if (!string.IsNullOrEmpty(resellerId))
                {
                    query = query.Where(q => q.ResellerID == resellerId);
                }


                //Status
                var status = (from st in _statusRepository.Table select st).ToList();
                var financeTypesList = (from fin in _financeTypesRepository.Table select fin).ToList();

                var results = query.Select(q => new ApplicationSearchResponse
                {
                    Id = q.Id,
                    ApplicationNumber = q.ApplicationNumber,
                    EndCustomerName = q.EndCustomerName,
                    FinanceType = (!string.IsNullOrEmpty(q.FinanceType)) ? q.FinanceType : string.Empty,
                    ResellerId = q.ResellerID,
                    ResellerName = q.ResellerName,
                    Status = q.ApplicationStatus,
                    QuoteTotal = q.QuoteTotal,
                    CreatedDate = q.CreateDate
                }).ToList();

                foreach (var result in results)
                {
                    result.StatusDescr = status.Where(st => st.Id == result.Status).FirstOrDefault().Description;
                    var finType = financeTypesList.Where(st => Convert.ToString(st.QuoteDurationType) == result.FinanceType).FirstOrDefault();
                    if (finType != null)
                        result.FinanceTypeName = finType.Description;
                }

                //Set the response
                response.SearchResult = results;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public ApplicationSaveResponseModel SaveApplication(ApplicationDetailsModel applicationDetailsModel, string userId)
        {
            var response = new ApplicationSaveResponseModel();
            try
            {

                var existingApplication = _applicationsRepository.Table.Where(a => a.Id == applicationDetailsModel.Id).FirstOrDefault();

                if (existingApplication != null)
                {
                    if (existingApplication.ApplicationStatus != applicationDetailsModel.Status)
                    {

                        existingApplication.ApplicationStatus = applicationDetailsModel.Status;

                        //if approved
                        if (applicationDetailsModel.Status == Configuration.GetValue<int>("ApplicationApprovedId"))
                        {
                            existingApplication.ApprovedBy = userId;
                            existingApplication.ApprovedDate = System.DateTime.Now.ToLocalTime();
                        }

                        //if rejected
                        if (applicationDetailsModel.Status == Configuration.GetValue<int>("ApplicationRejectedId"))
                        {
                            existingApplication.RejectedBy = userId;
                            existingApplication.RejectedDate = System.DateTime.Now.ToLocalTime();
                        }
                    }
                    //Entity Type Details 
                    existingApplication.EntityType = applicationDetailsModel.EntityDetails.EntityType;
                    existingApplication.EntityTypeOther = applicationDetailsModel.EntityDetails.EntityTypeOther;

                    existingApplication.EntityTrustType = applicationDetailsModel.EntityDetails.EntityTrustType;
                    existingApplication.EntityTrustOther = applicationDetailsModel.EntityDetails.EntityTrustOther;

                    existingApplication.EntityTrustName = applicationDetailsModel.EntityDetails.EntityTrustName;
                    existingApplication.EntityTrustABN = applicationDetailsModel.EntityDetails.EntityTrustABN;

                    existingApplication.EndCustomerName = applicationDetailsModel.EndCustomerDetails.EndCustomerName;
                    existingApplication.EndCustomerABN = applicationDetailsModel.EndCustomerDetails.EndCustomerABN;
                    existingApplication.EndCustomerTradingAs = applicationDetailsModel.EndCustomerDetails.EndCustomerTradingAs;
                    existingApplication.EndCustomerYearsTrading = applicationDetailsModel.EndCustomerDetails.EndCustomerYearsTrading;
                    existingApplication.EndCustomerPhone = applicationDetailsModel.EndCustomerDetails.EndCustomerPhone;
                    existingApplication.EndCustomerFax = applicationDetailsModel.EndCustomerDetails.EndCustomerFax;
                    existingApplication.BusinessActivity = applicationDetailsModel.BusinessActivity;
                    existingApplication.AveAnnualSales = applicationDetailsModel.AveAnnualSales;

                    existingApplication.EndCustomerContactName = applicationDetailsModel.EndCustomerDetails.EndCustomerContactName;
                    existingApplication.EndCustomerContactPhone = applicationDetailsModel.EndCustomerDetails.EndCustomerContactPhone;
                    existingApplication.EndCustomerContactEmail = applicationDetailsModel.EndCustomerDetails.EndCustomerContactEmail;


                    //Business Address
                    existingApplication.EndCustomerPrimaryAddressLine1 = applicationDetailsModel.EndCustomerDetails.EndCustomerPrimaryAddressLine1;
                    existingApplication.EndCustomerPrimaryAddressLine2 = applicationDetailsModel.EndCustomerDetails.EndCustomerPrimaryAddressLine2;
                    existingApplication.EndCustomerPrimaryCity = applicationDetailsModel.EndCustomerDetails.EndCustomerPrimaryCity;
                    existingApplication.EndCustomerPrimaryState = applicationDetailsModel.EndCustomerDetails.EndCustomerPrimaryState;
                    existingApplication.EndCustomerPrimaryPostcode = applicationDetailsModel.EndCustomerDetails.EndCustomerPrimaryPostcode;
                    existingApplication.EndCustomerPrimaryCountry = applicationDetailsModel.EndCustomerDetails.EndCustomerPrimaryCity;

                    //Postal Address     
                    existingApplication.EndCustomerPostalAddressLine1 = applicationDetailsModel.EndCustomerDetails.EndCustomerPostalAddressLine1;
                    existingApplication.EndCustomerPostalAddressLine2 = applicationDetailsModel.EndCustomerDetails.EndCustomerPostalAddressLine2;
                    existingApplication.EndCustomerPostalCity = applicationDetailsModel.EndCustomerDetails.EndCustomerPostalCity;
                    existingApplication.EndCustomerPostalState = applicationDetailsModel.EndCustomerDetails.EndCustomerPostalState;
                    existingApplication.EndCustomerPostalPostcode = applicationDetailsModel.EndCustomerDetails.EndCustomerPostalPostcode;
                    existingApplication.EndCustomerPostalCountry = applicationDetailsModel.EndCustomerDetails.EndCustomerPostalCountry;

                    //Delivery Address
                    existingApplication.EndCustomerDeliveryAddressLine1 = applicationDetailsModel.EndCustomerDetails.EndCustomerDeliveryAddressLine1;
                    existingApplication.EndCustomerDeliveryAddressLine2 = applicationDetailsModel.EndCustomerDetails.EndCustomerDeliveryAddressLine2;
                    existingApplication.EndCustomerDeliveryCity = applicationDetailsModel.EndCustomerDetails.EndCustomerDeliveryCity;
                    existingApplication.EndCustomerDeliveryState = applicationDetailsModel.EndCustomerDetails.EndCustomerDeliveryState;
                    existingApplication.EndCustomerDeliveryPostcode = applicationDetailsModel.EndCustomerDetails.EndCustomerDeliveryPostcode;
                    existingApplication.EndCustomerDeliveryCountry = applicationDetailsModel.EndCustomerDetails.EndCustomerDeliveryCountry;

                    existingApplication.GuarantorSecurityValue = applicationDetailsModel.GuarantorSecurityValue;
                    existingApplication.GuarantorSecurityOwing = applicationDetailsModel.GuarantorSecurityOwing;
                    existingApplication.IsGuarantorPropertyOwner = applicationDetailsModel.IsGuarantorPropertyOwner;

                    //update email fields
                    existingApplication.LeaseAgreementSignorEmail = applicationDetailsModel.LeaseAgreementSignorEmail;
                    existingApplication.CertificateOfAcceptanceSignorEmail = applicationDetailsModel.CertificateOfAcceptanceSignorEmail;
                    existingApplication.AntiMoneyLaunderingSignorEmail = applicationDetailsModel.AntiMoneyLaunderingSignorEmail;
                    existingApplication.DirectDebitACHSignorEmail = applicationDetailsModel.DirectDebitACHSignorEmail;

                    //update in DB
                    _applicationsRepository.Update(existingApplication);

                    response.ApplicationId = existingApplication.Id;
                    response.ApplicationNumber = existingApplication.ApplicationNumber;

                }

                if (applicationDetailsModel.ApplicationContacts != null && applicationDetailsModel.ApplicationContacts.Count > 0)
                {
                    //Get all ContactsXref
                    var contactsXRef = _contactsXrefRepository.Table.Where(xref => xref.ApplicationNumber == applicationDetailsModel.ApplicationNumber).ToList();

                    foreach (var applicationContact in applicationDetailsModel.ApplicationContacts)
                    {
                        if (applicationContact.ContactID != null)
                        {
                            var contactFound = false;
                            if (contactsXRef != null)
                            {
                                foreach (var xref in contactsXRef)
                                {
                                    if (xref.ContactID == applicationContact.ContactID)
                                    {
                                        contactFound = true;
                                    }
                                }

                                if (!contactFound)
                                {
                                    //Add it to the Xref
                                    var contactXRef = new ContactsXref();
                                    contactXRef.ContactID = applicationContact.ContactID;
                                    contactXRef.ApplicationNumber = applicationDetailsModel.ApplicationNumber;
                                    _contactsXrefRepository.Insert(contactXRef);
                                }
                            }
                        }
                        else
                        {
                            //Create contact and add it to contactsXref
                            var contact = new Contacts();
                            contact.ContactID = Guid.NewGuid().ToString();
                            contact.ContactName = applicationContact.ContactName;
                            contact.ContactAddress = applicationContact.ContactAddress;
                            contact.ContactEmail = applicationContact.ContactEmail;

                            if (applicationContact.ContactDOB != null)
                            {
                                contact.ContactDOB = Convert.ToDateTime(applicationContact.ContactDOB).ToLocalTime().Date;
                            }

                            contact.ContactDriversLicNo = applicationContact.ContactDriversLicNo;
                            contact.ContactPhone = applicationContact.ContactPhone;
                            contact.ContactType = applicationContact.ContactType;
                            if (!string.IsNullOrEmpty(applicationContact.ContactABNACN))
                            {
                                contact.ContactABNACN = applicationContact.ContactABNACN;
                            }
                            contact.IsContactSignatory = applicationContact.IsContactSignatory;

                            //Add Contact
                            _contactsRepository.Insert(contact);

                            //Add it to the Xref
                            var contactXRef = new ContactsXref();
                            contactXRef.ContactID = contact.ContactID;
                            contactXRef.ApplicationNumber = applicationDetailsModel.ApplicationNumber;
                            _contactsXrefRepository.Insert(contactXRef);

                        }
                    }

                }

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public List<Web.Models.DBModel.Contacts> GetContacts(string resellerID)
        {
            return _contactsRepository.Table.Where(c => c.ResellerID == resellerID).ToList();
        }


        /// <summary>
        /// Guarantor Contacts are filtered for Contact Types 1,2,3 and 4
        /// </summary>
        /// <param name="resellerID"></param>
        /// <returns></returns>
        public List<Web.Models.DBModel.Contacts> GetGuarantorContacts(string resellerID)
        {
            return _contactsRepository.Table.Where(c => (c.ContactType == 1 || c.ContactType == 2 || c.ContactType == 3 || c.ContactType == 4) && c.ResellerID == resellerID).ToList();
        }

        /// <summary>
        /// Accountant Contacts are filtered for Contact Type 6
        /// </summary>
        /// <param name="resellerID"></param>
        /// <returns></returns>
        public List<Web.Models.DBModel.Contacts> GetAccountantContacts(string resellerID)
        {
            return _contactsRepository.Table.Where(c => c.ContactType == 6 && c.ResellerID == resellerID).ToList();
        }

        /// <summary>
        /// Trustee Contacts are filtered for Contact Type 6
        /// </summary>
        /// <param name="resellerID"></param>
        /// <returns></returns>
        public List<Web.Models.DBModel.Contacts> GetTrusteeContacts(string resellerID)
        {
            return _contactsRepository.Table.Where(c => c.ContactType == 5 && c.ResellerID == resellerID).ToList();
        }

        public List<Web.Models.DBModel.Contacts> GetBeneficialOwnersContacts(string resellerID)
        {
            return _contactsRepository.Table.Where(c => c.ContactType == 7 && c.ResellerID == resellerID).ToList();
        }

        public ApplicationDownloadResponse DownloadApplication(ApplicationDownloadInput inputModel)
        {
            var response = new ApplicationDownloadResponse();
            try
            {
                var applicationDetailsResponse = GetApplicationDetails(inputModel.Id.ToString());
                if (applicationDetailsResponse.HasError)
                {
                    response.HasError = true;
                    response.ErrorMessage = applicationDetailsResponse.ErrorMessage;
                    return response;
                }

                var path = GenerateApplicationPdf(applicationDetailsResponse.ApplicationDetails);
                if (path != null)
                {

                    response.FileName = path.Substring(path.LastIndexOf("\\") + 1);
                    if (response.FileName.Length > 200)
                    {
                        response.FileName = response.FileName.Substring(0, 200);
                    }
                    response.DownloadFile = System.IO.File.ReadAllBytes(path);
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.ToString();
            }
            return response;
        }

        private string GenerateApplicationPdf(ApplicationDetailsModel applicationDetails)
        {
            string outputFile = string.Empty;
            try
            {
                string pdfTemplate = Configuration.GetValue<string>("ApplicationPdfTemplate");
                string physicalPath = Configuration.GetValue<string>("EmailAttachmentTempRootDirectory") + "\\" + "LoggedInUser";
                // create own folder for each user id if it does not exist
                Tools.CreateFolder(Configuration.GetValue<string>("EmailAttachmentTempRootDirectory"), "LoggedInUser");
                string fileName = "IMFS Equipment Finance Application - " + System.DateTime.Now.ToLocalTime().ToString("ddMMyyyyHHmmss") + " - " + applicationDetails.ApplicationNumber + ".pdf";
                outputFile = physicalPath + "\\" + fileName;
                using (Stream pdfInputStream = new FileStream(path: pdfTemplate, mode: FileMode.Open))
                using (Stream resultPDFOutputStream = new FileStream(path: outputFile, mode: FileMode.Create))
                using (Stream resultPDFStream = FillPdfForm(pdfInputStream, applicationDetails))
                {
                    // set the position of the stream to 0 to avoid corrupted PDF. 
                    resultPDFStream.Position = 0;
                    resultPDFStream.CopyTo(resultPDFOutputStream);
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return outputFile;
        }


        private Stream FillPdfForm(Stream inputStream, ApplicationDetailsModel model)
        {
            Stream outStream = new MemoryStream();
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            Stream inStream = null;
            try
            {
                pdfReader = new PdfReader(inputStream);
                pdfStamper = new PdfStamper(pdfReader, outStream);
                AcroFields form = pdfStamper.AcroFields;

                //Entity Type field
                if (model.EntityDetails != null)
                {
                    if (model.EntityDetails.EntityType == "1")
                        form.SetField("EntityTypePick", "Company");
                    if (model.EntityDetails.EntityType == "2")
                        form.SetField("EntityTypePick", "Partnership");
                    if (model.EntityDetails.EntityType == "3")
                        form.SetField("EntityTypePick", "Trust");
                    if (model.EntityDetails.EntityType == "4")
                        form.SetField("EntityTypePick", "Sole Trader");
                    if (model.EntityDetails.EntityType == "5")
                        form.SetField("EntityTypePick", "Other");

                    form.SetField("Entity_Other", model.EntityDetails.EntityTypeOther);
                    form.SetField("If trust name of trust", model.EntityDetails.EntityTrustName);
                    form.SetField("Trust ABN", model.EntityDetails.EntityTrustABN);

                    form.SetField("TrustTypePick", model.EntityDetails.EntityTrustType);
                    form.SetField("TrustType_other", model.EntityDetails.EntityTrustOther);
                }

                if (model.EndCustomerDetails != null)
                {
                    form.SetField("Customer", model.EndCustomerDetails.EndCustomerName);
                    form.SetField("ACNABN", model.EndCustomerDetails.EndCustomerABN);
                    form.SetField("Trading name", model.EndCustomerDetails.EndCustomerTradingAs);

                    form.SetField("Phone", model.EndCustomerDetails.EndCustomerPhone);
                    form.SetField("Fax", model.EndCustomerDetails.EndCustomerFax);

                }
                form.SetField("Finance quote number", Convert.ToString(model.QuoteID));
                form.SetField("Finance quote value", model.QuoteTotal.Value.ToString("0.00"));
                form.SetField("Equipment description", model.GoodsDescription);

                if (model.AveAnnualSales.HasValue)
                    form.SetField("Average annual sales", model.AveAnnualSales.Value.ToString("0.00"));

                if (model.IsGuarantorPropertyOwner == true)
                    form.SetField("GuarantorPropertyPick", "Yes");
                else if (model.IsGuarantorPropertyOwner == false)
                    form.SetField("GuarantorPropertyPick", "No");
                else
                    form.SetField("GuarantorPropertyPick", "--- Select ---");

                if (model.GuarantorSecurityValue.HasValue)
                    form.SetField("Guarantor_property_Value", model.GuarantorSecurityValue.Value.ToString("0.00"));

                if (model.GuarantorSecurityOwing.HasValue)
                    form.SetField("Guarantor_property_Owing", model.GuarantorSecurityOwing.Value.ToString("0.00"));


                form.SetField("Years in principal business activity", model.EndCustomerDetails?.EndCustomerYearsTrading);
                form.SetField("Principal business activity", model.BusinessActivity);


                if (model.FinanceType == "1")
                    form.SetField("FinanceTypePicker", "Lease");
                if (model.FinanceType == "2")
                    form.SetField("FinanceTypePicker", "Rental");
                if (model.FinanceType == "4")
                    form.SetField("FinanceTypePicker", "Instalment");

                form.SetField("Finance Term", model.FinanceDuration);

                if (model.FinanceFrequency == "Monthly")
                    form.SetField("PaymentFrequencyPicker", "Monthly");
                if (model.FinanceFrequency == "Quarterly")
                    form.SetField("PaymentFrequencyPicker", "Quarterly");
                if (model.FinanceFrequency == "Yearly")
                    form.SetField("PaymentFrequencyPicker", "Yearly");

                if (model.FinanceValue.HasValue)
                    form.SetField("Finance quote payment", model.FinanceValue.Value.ToString("0.00"));


                if (model.EndCustomerDetails != null)
                {
                    //Business Address                
                    form.SetField("BusinessAddressLine1", model.EndCustomerDetails.EndCustomerPrimaryAddressLine1);
                    form.SetField("BusinessAddressLine2", model.EndCustomerDetails.EndCustomerPrimaryAddressLine2);
                    form.SetField("BusinessAddressCity", model.EndCustomerDetails.EndCustomerPrimaryCity);
                    form.SetField("BusinessAddressState", model.EndCustomerDetails.EndCustomerPrimaryState);
                    form.SetField("BusinessAddressPostcode", model.EndCustomerDetails.EndCustomerPrimaryPostcode);
                    form.SetField("BusinessAddressCountry", model.EndCustomerDetails.EndCustomerPrimaryCountry);

                    //Postal Address                
                    form.SetField("PostalAddressLine1", model.EndCustomerDetails.EndCustomerPostalAddressLine1);
                    form.SetField("PostalAddressLine2", model.EndCustomerDetails.EndCustomerPostalAddressLine2);
                    form.SetField("PostalAddressCity", model.EndCustomerDetails.EndCustomerPostalCity);
                    form.SetField("PostalAddressState", model.EndCustomerDetails.EndCustomerPostalState);
                    form.SetField("PostalAddressPostCode", model.EndCustomerDetails.EndCustomerPostalPostcode);
                    form.SetField("PostalAddressCountry", model.EndCustomerDetails.EndCustomerPostalCountry);

                    //Delivery Address
                    form.SetField("DeliveryAddressLine1", model.EndCustomerDetails.EndCustomerDeliveryAddressLine1);
                    form.SetField("DeliveryAddressLine2", model.EndCustomerDetails.EndCustomerDeliveryAddressLine2);
                    form.SetField("DeliveryAddressCity", model.EndCustomerDetails.EndCustomerDeliveryCity);
                    form.SetField("DeliveryAddressState", model.EndCustomerDetails.EndCustomerDeliveryState);
                    form.SetField("DeliveryAddressPostCode", model.EndCustomerDetails.EndCustomerDeliveryPostcode);
                    form.SetField("DeliveryAddressCountry", model.EndCustomerDetails.EndCustomerDeliveryCountry);

                    form.SetField("Contact name", model.EndCustomerDetails.EndCustomerContactName);
                    form.SetField("Mobile", model.EndCustomerDetails.EndCustomerContactPhone);
                    form.SetField("Email", model.EndCustomerDetails.EndCustomerContactEmail);

                    form.SetField("Full name", model.EndCustomerDetails.EndCustomerSignatoryName);
                }

                if (model.ResellerDetails != null)
                {
                    form.SetField("Sales representative", model.ResellerDetails.ResellerContactName);
                    form.SetField("Dealername", model.ResellerDetails.ResellerName);
                }

                //Guarantors Contacts
                int guarantorCounter = 1;
                foreach (var guarantorContact in model.ApplicationContacts.Where(c => c.ContactType == 1 || c.ContactType == 2 || c.ContactType == 3 || c.ContactType == 4).ToList())
                {
                    //ContactType	ContactDescription
                    //    1             Director
                    //    2             Sole Trader
                    //    3             Partner
                    //    4             Guarantor

                    form.SetField("ContactName" + guarantorCounter, guarantorContact.ContactName);
                    if (guarantorContact.ContactType == 1)
                        form.SetField("ContactPicker" + guarantorCounter, "Director");
                    else if (guarantorContact.ContactType == 2)
                        form.SetField("ContactPicker" + guarantorCounter, "Sole Trader");
                    else if (guarantorContact.ContactType == 3)
                        form.SetField("ContactPicker" + guarantorCounter, "Partner");
                    else if (guarantorContact.ContactType == 4)
                        form.SetField("ContactPicker" + guarantorCounter, "Guarantor");

                    form.SetField("ContactResidentialAddress" + guarantorCounter, guarantorContact.ContactAddress);
                    form.SetField("ContactEmail" + guarantorCounter, guarantorContact.ContactEmail);
                    form.SetField("ContactDOB" + guarantorCounter, guarantorContact.ContactDOB != null ? guarantorContact.ContactDOB.Value.ToString("dd/MM/yyyy") : string.Empty);
                    form.SetField("ContactDrivers" + guarantorCounter, guarantorContact.ContactDriversLicNo);
                    form.SetField("ContactPhone" + guarantorCounter, guarantorContact.ContactPhone);

                    //Increment counter
                    guarantorCounter++;
                    if (guarantorCounter > 3)
                        continue;
                }


                //Trustee Contacts (5)
                int trusteeCounter = 1;
                foreach (var trusteeContact in model.ApplicationContacts.Where(c => c.ContactType == 5).ToList())
                {
                    form.SetField("TrusteeName" + trusteeCounter, trusteeContact.ContactName);
                    form.SetField("TrusteeABN" + trusteeCounter, trusteeContact.ContactABNACN);
                    form.SetField("TrusteeResidentialAddress" + trusteeCounter, trusteeContact.ContactAddress);
                    form.SetField("TrusteeEmail" + trusteeCounter, trusteeContact.ContactEmail);
                    form.SetField("TrusteeDOB" + trusteeCounter, trusteeContact.ContactDOB != null ? trusteeContact.ContactDOB.Value.ToString("dd/MM/yyyy") : string.Empty);
                    form.SetField("TrusteeDrivers" + trusteeCounter, trusteeContact.ContactDriversLicNo);
                    form.SetField("TrusteePhone" + trusteeCounter, trusteeContact.ContactPhone);

                    //Increment counter
                    trusteeCounter++;
                    if (trusteeCounter > 3)
                        continue;
                }


                //Accountant Contact   (6)             
                foreach (var accountantContact in model.ApplicationContacts.Where(c => c.ContactType == 6).ToList())
                {
                    form.SetField("Account_name", accountantContact.ContactName);
                    form.SetField("AccountPhone_number", accountantContact.ContactPhone);
                    form.SetField("AccountEmail", accountantContact.ContactEmail);
                }


                //Beneficial Owners Contacts (7)
                int beneficialCounter = 1;
                foreach (var ownerContact in model.ApplicationContacts.Where(c => c.ContactType == 7).ToList())
                {
                    form.SetField("OwnerName" + beneficialCounter, ownerContact.ContactName);
                    form.SetField("OwnersResidentialAddress" + beneficialCounter, ownerContact.ContactAddress);
                    form.SetField("OwnerEmail" + beneficialCounter, ownerContact.ContactEmail);
                    form.SetField("OwnersDOB" + beneficialCounter, ownerContact.ContactDOB != null ? ownerContact.ContactDOB.Value.ToString("dd/MM/yyyy") : string.Empty);
                    form.SetField("OwnerDrivers" + beneficialCounter, ownerContact.ContactDriversLicNo);
                    form.SetField("OwnerPhone" + beneficialCounter, ownerContact.ContactPhone);

                    //Increment counter
                    beneficialCounter++;
                    if (beneficialCounter > 3)
                        continue;
                }

                // set this if you want the result PDF to not be editable. 
                //pdfStamper.FormFlattening = true;
                pdfStamper.FormFlattening = false;
                return outStream;
            }
            finally
            {
                pdfStamper?.Close();
                pdfReader?.Close();
                inStream?.Close();
            }
        }

        public ApplicationUpdateResponseModel RejectApplication(int applicationId)
        {
            var response = new ApplicationUpdateResponseModel();
            try
            {
                var application = _applicationsRepository.Table.Where(a => a.Id == applicationId).FirstOrDefault();
                if (application != null)
                {
                    application.ApplicationStatus = Configuration.GetValue<int>("ApplicationRejectedId");
                    _applicationsRepository.Update(application);
                    response.ApplicationNumber = applicationId;
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        public ApplicationUpdateResponseModel CancelApplication(int applicationId)
        {
            var response = new ApplicationUpdateResponseModel();
            try
            {
                var application = _applicationsRepository.Table.Where(a => a.Id == applicationId).FirstOrDefault();
                if (application != null)
                {
                    application.ApplicationStatus = Configuration.GetValue<int>("ApplicationCancelledId");
                    _applicationsRepository.Update(application);
                    response.ApplicationNumber = applicationId;
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        public List<RecentApplicationsModel> GetRecentApplications(string resellerId)
        {
            var response = new List<RecentApplicationsModel>();
            try
            {

                return (from application in _applicationsRepository.Table
                        join status in _statusRepository.Table on application.ApplicationStatus equals status.Id
                        join financeType in _financeTypesRepository.Table on application.FinanceType equals financeType.Id.ToString()
                        join quote in _quotesRepository.Table on application.QuoteId equals quote.Id
                        where application.ResellerID == resellerId
                        select new RecentApplicationsModel
                        {
                            Id = application.Id,
                            ApplicationNumber = application.ApplicationNumber,
                            endUser = application.EndCustomerName != null || application.EndCustomerName != "" ? application.EndCustomerName : "N/A",
                            Status = status.Description != null || status.Description != "" ? status.Description : "N/A",
                            FinanaceAmount = application.FinanceTotal.HasValue ? application.FinanceTotal : 0,
                            FinanceType = financeType.Description != "" ? financeType.Description : "N/A",
                            CreatedDate = application.CreateDate,
                        }).OrderByDescending(x => x.CreatedDate).Take(10).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<AwaitingInvoiceModel> GetAwaitingInvoices(string resellerId)
        {
            var response = new List<AwaitingInvoiceModel>();
            try
            {
                List<int> statusIDs = new List<int>() { 8, 12 };
                return (from application in _applicationsRepository.Table
                        join status in _statusRepository.Table on application.ApplicationStatus equals status.Id
                        join financeType in _financeTypesRepository.Table on application.FinanceType equals financeType.Id.ToString()
                        join quote in _quotesRepository.Table on application.QuoteId equals quote.Id
                        where
                        statusIDs.Contains(application.ApplicationStatus ?? default(int))
                        &&
                        quote.ResellerAccount == resellerId
                        select new AwaitingInvoiceModel
                        {
                            Id = application.Id,
                            ApplicationNumber = application.ApplicationNumber,
                            EndCustomerName = application.EndCustomerName != null || application.EndCustomerName != "" ? application.EndCustomerName : "N/A",
                            Status = status.Description != null || status.Description != "" ? status.Description : "N/A",
                            FinanaceAmount = application.FinanceTotal.HasValue ? application.FinanceTotal : 0,
                            FinanceType = financeType.Description != "" ? financeType.Description : "N/A",
                            CreatedDate = application.CreateDate,
                            ApprovedDate = application.ApprovedDate,

                        }).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region Signatories Tabs

        /// <summary>
        /// 
        /// </summary>
        private enum ContactTypeTabs
        {
            Guarantors = 4,
            Trustees = 5,
            Owners = 7
        }

        /// <summary>
        /// Contact name, Email and Number for particular CustomerNumber
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns></returns>
        public SignatoriesTabModel GetSignatories(int applicationNumber)
        {
            var response = new SignatoriesTabModel();
            try
            {
                #region guarantors

                #region Client Query

                /****SQL****/
                //select apps.ApplicationNumber ,con.ContactName, con.ContactEmail, con.ContactAddress, con.ContactDOB, con.ContactDriversLicNo,
                //con.ContactPhone, cont.ContactDescription, con.IsContactSignatory from ContactsXref conx
                //inner join Applications apps on conx.ApplicationNumber = apps.ApplicationNumber
                //inner join Contacts con on con.ContactID = conx.ContactID
                //inner join ContactsTypes cont on cont.ContactType = con.ContactType
                //where apps.ApplicationNumber = '<<Current Application Number>>' and cont.ContactType = 4

                /****LINQ****/
                List<GuarantorsListResult> _guarantors = (from conx in _contactsXrefRepository.Table
                                                          join apps in _applicationsRepository.Table on conx.ApplicationNumber equals apps.ApplicationNumber
                                                          join con in _contactsRepository.Table on conx.ContactID equals con.ContactID
                                                          join cont in _contactTypesRepository.Table on con.ContactType equals cont.ContactType
                                                          where apps.ApplicationNumber == applicationNumber && cont.ContactType == 4
                                                          select new GuarantorsListResult
                                                          {
                                                              ApplicationNumber = apps.ApplicationNumber,
                                                              ContactName = !string.IsNullOrEmpty(con.ContactName) ? con.ContactName : "N/A",
                                                              ContactEmail = con.ContactEmail,
                                                              ContactAddress = con.ContactAddress,
                                                              ContactDOB = con.ContactDOB == null ? new DateTime() : con.ContactDOB,
                                                              ContactDriversLicNo = con.ContactDriversLicNo,
                                                              ContactPhone = con.ContactPhone,
                                                              ContactDescription = cont.ContactDescription,
                                                              IsContactSignatory = con.IsContactSignatory,
                                                              ContactID = con.ContactID,
                                                              ResellerID = con.ResellerID

                                                          }).OrderBy(x => x.ApplicationNumber).Distinct().ToList();


                #endregion

                var guarantors = new List<GuarantorsListModel>();

                if (_guarantors != null && _guarantors.Count > 0)
                {
                    foreach (var _guarantor in _guarantors)
                    {
                        //var contact = _contactsRepository.Table.Where(a => a.ContactID == _guarantor.ContactID).Select(x=>x.ContactDOB).FirstOrDefault();

                        var guarantor = new GuarantorsListModel();
                        //map
                        guarantor.FullName = _guarantor.ContactName;
                        guarantor.Email = _guarantor.ContactEmail;
                        guarantor.ResidentialAddress = _guarantor.ContactAddress;
                        guarantor.DOB = _guarantor.ContactDOB;
                        guarantor.DriversLicNo = _guarantor.ContactDriversLicNo;
                        guarantor.Phone = _guarantor.ContactPhone;
                        guarantor.Type = _guarantor.ContactDescription;
                        guarantor.Signatory = _guarantor.IsContactSignatory;
                        guarantor.ContactType = (int)ContactTypeTabs.Guarantors;
                        guarantor.ContactID = _guarantor.ContactID;
                        guarantor.ResellerID = _guarantor.ResellerID;
                        //bind
                        guarantors.Add(guarantor);
                    }
                }

                #endregion

                response.Guarantors = guarantors;

                #region trustees

                #region Client Query

                /****SQL****/
                //select apps.ApplicationNumber ,con.ContactName, con.ContactEmail, con.ContactAddress, con.ContactDOB, con.ContactDriversLicNo,
                //con.ContactPhone, cont.ContactDescription, con.IsContactSignatory from ContactsXref conx -- Added new , con.ContactABNACN
                //inner join Applications apps on conx.ApplicationNumber = apps.ApplicationNumber
                //inner join Contacts con on con.ContactID = conx.ContactID
                //inner join ContactsTypes cont on cont.ContactType = con.ContactType
                //where apps.ApplicationNumber = '<<Current Application Number>>' and cont.ContactType = 5

                /****LINQ****/
                List<TrusteesListResult> _trustees = (from conx in _contactsXrefRepository.Table
                                                      join apps in _applicationsRepository.Table on conx.ApplicationNumber equals apps.ApplicationNumber
                                                      join con in _contactsRepository.Table on conx.ContactID equals con.ContactID
                                                      join cont in _contactTypesRepository.Table on con.ContactType equals cont.ContactType
                                                      where apps.ApplicationNumber == applicationNumber && cont.ContactType == 5
                                                      select new TrusteesListResult
                                                      {
                                                          ApplicationNumber = apps.ApplicationNumber,
                                                          ContactName = !string.IsNullOrEmpty(con.ContactName) ? con.ContactName : "N/A",
                                                          ContactEmail = con.ContactEmail,
                                                          ContactAddress = con.ContactAddress,
                                                          ContactDOB = con.ContactDOB == null ? new DateTime() : con.ContactDOB,
                                                          ContactDriversLicNo = con.ContactDriversLicNo,
                                                          ContactPhone = con.ContactPhone,
                                                          ContactDescription = cont.ContactDescription,
                                                          IsContactSignatory = con.IsContactSignatory,
                                                          ContactABN = con.ContactABNACN,
                                                          ContactID = con.ContactID,
                                                          ResellerID = con.ResellerID

                                                      }).OrderBy(x => x.ApplicationNumber).Distinct().ToList();


                #endregion

                var trustees = new List<TrusteesListModel>();

                //get data
                if (_trustees != null && _trustees.Count > 0)
                {
                    foreach (var _trustee in _trustees)
                    {
                        var trustee = new TrusteesListModel();
                        //map
                        trustee.FullName = _trustee.ContactName;
                        trustee.Email = _trustee.ContactEmail;
                        trustee.ResidentialAddress = _trustee.ContactAddress;
                        trustee.DOB = _trustee.ContactDOB;
                        trustee.DriversLicNo = _trustee.ContactDriversLicNo;
                        trustee.Phone = _trustee.ContactPhone;
                        trustee.ABN = _trustee.ContactABN;
                        trustee.Signatory = _trustee.IsContactSignatory;
                        trustee.ContactType = (int)ContactTypeTabs.Trustees;
                        trustee.ContactID = _trustee.ContactID;
                        trustee.ResellerID = _trustee.ResellerID;
                        //bind
                        trustees.Add(trustee);
                    }
                }

                #endregion

                response.Trustees = trustees;

                #region accountant

                #region Client Query

                /****SQL****/
                //select apps.ApplicationNumber ,con.ContactName, con.ContactEmail, con.ContactAddress, con.ContactDOB, con.ContactDriversLicNo,
                //con.ContactPhone, cont.ContactDescription, con.IsContactSignatory from ContactsXref conx
                //inner join Applications apps on conx.ApplicationNumber = apps.ApplicationNumber
                //inner join Contacts con on con.ContactID = conx.ContactID
                //inner join ContactsTypes cont on cont.ContactType = con.ContactType
                //where apps.ApplicationNumber = '<<Current Application Number>>' and cont.ContactType = 6


                /****LINQ****/
                List<AccountantListResult> _accountants = (from conx in _contactsXrefRepository.Table
                                                           join apps in _applicationsRepository.Table on conx.ApplicationNumber equals apps.ApplicationNumber
                                                           join con in _contactsRepository.Table on conx.ContactID equals con.ContactID
                                                           join cont in _contactTypesRepository.Table on con.ContactType equals cont.ContactType
                                                           where apps.ApplicationNumber == applicationNumber && cont.ContactType == 6
                                                           select new AccountantListResult
                                                           {
                                                               ApplicationNumber = apps.ApplicationNumber,
                                                               ContactName = !string.IsNullOrEmpty(con.ContactName) ? con.ContactName : "N/A",
                                                               ContactEmail = con.ContactEmail,
                                                               ContactAddress = con.ContactAddress,
                                                               ContactDOB = con.ContactDOB == null ? new DateTime() : con.ContactDOB,
                                                               ContactDriversLicNo = con.ContactDriversLicNo,
                                                               ContactPhone = con.ContactPhone,
                                                               ContactDescription = cont.ContactDescription,
                                                               IsContactSignatory = con.IsContactSignatory,
                                                               ContactID = con.ContactID,
                                                               ResellerID = con.ResellerID

                                                           }).OrderBy(x => x.ApplicationNumber).Distinct().ToList();


                #endregion

                var accountants = new List<AccountantListModel>();
                //get data
                if (_accountants != null && _accountants.Count > 0)
                {
                    foreach (var _accountant in _accountants)
                    {
                        var accountant = new AccountantListModel();
                        //map
                        accountant.FullName = _accountant.ContactName;
                        accountant.Email = _accountant.ContactEmail;
                        accountant.Phone = _accountant.ContactPhone;
                        accountant.ContactID = _accountant.ContactID;
                        accountant.ResellerID = _accountant.ResellerID;

                        //bind
                        accountants.Add(accountant);
                    }
                }
                #endregion

                response.Accountants = accountants;

                #region owners
                #region Client Query

                /****SQL****/
                //select apps.ApplicationNumber ,con.ContactName, con.ContactEmail, con.ContactAddress, con.ContactDOB, con.ContactDriversLicNo, 
                //con.ContactPhone, cont.ContactDescription, con.IsContactSignatory from ContactsXref conx
                //inner join Applications apps on conx.ApplicationNumber = apps.ApplicationNumber
                //inner join Contacts con on con.ContactID = conx.ContactID
                //inner join ContactsTypes cont on cont.ContactType = con.ContactType
                //where apps.ApplicationNumber = '<<Current Application Number>>' and cont.ContactType = 7



                /****LINQ****/
                List<OwnerListResult> _owners = (from conx in _contactsXrefRepository.Table
                                                 join apps in _applicationsRepository.Table on conx.ApplicationNumber equals apps.ApplicationNumber
                                                 join con in _contactsRepository.Table on conx.ContactID equals con.ContactID
                                                 join cont in _contactTypesRepository.Table on con.ContactType equals cont.ContactType
                                                 where apps.ApplicationNumber == applicationNumber && cont.ContactType == 7
                                                 select new OwnerListResult
                                                 {
                                                     ApplicationNumber = apps.ApplicationNumber,
                                                     ContactName = !string.IsNullOrEmpty(con.ContactName) ? con.ContactName : "N/A",
                                                     ContactEmail = con.ContactEmail,
                                                     ContactAddress = con.ContactAddress,
                                                     ContactDOB = con.ContactDOB == null ? new DateTime() : con.ContactDOB,
                                                     ContactDriversLicNo = con.ContactDriversLicNo,
                                                     ContactPhone = con.ContactPhone,
                                                     ContactDescription = cont.ContactDescription,
                                                     IsContactSignatory = con.IsContactSignatory,
                                                     ContactID = con.ContactID,
                                                     ResellerID = con.ResellerID

                                                 }).OrderBy(x => x.ApplicationNumber).Distinct().ToList();


                #endregion

                var owners = new List<OwnersListModel>();
                //get data
                if (_owners != null && _owners.Count > 0)
                {
                    foreach (var _owner in _owners)
                    {
                        var owner = new OwnersListModel();
                        //map
                        owner.FullName = _owner.ContactName;
                        owner.Email = _owner.ContactEmail;
                        owner.ResidentialAddress = _owner.ContactAddress;
                        owner.DOB = _owner.ContactDOB;
                        owner.DriversLicNo = _owner.ContactDriversLicNo;
                        owner.Phone = _owner.ContactPhone;
                        owner.Signatory = _owner.IsContactSignatory;
                        owner.ContactType = (int)ContactTypeTabs.Owners;
                        owner.ContactID = _owner.ContactID;
                        owner.ResellerID = _owner.ResellerID;
                        //bind
                        owners.Add(owner);
                    }
                }
                #endregion

                response.Owners = owners;

                response.ErrorMessage = string.Empty;
                response.HasError = false;

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <param name="tabID"></param>
        /// <returns></returns>
        public ExistingContactsModel GetExistingContacts(string resellerId, int tabId)
        {
            var response = new ExistingContactsModel();
            try
            {
                var collection = new List<ExistingContactsSelectListModel>();

                if (tabId == (int)ContactTypeTabs.Guarantors)
                {
                    #region guarantors

                    #region Client Query

                    /****SQL****/
                    //select con.ContactName, con.ContactEmail, con.ContactAddress, con.ContactDOB, con.ContactDriversLicNo, 
                    //con.ContactPhone, cont.ContactDescription, con.IsContactSignatory, con.ResellerID from ContactsXref conx
                    //inner join Contacts con on con.ContactID = conx.ContactID
                    //inner join ContactsTypes cont on cont.ContactType = con.ContactType
                    //where con.resellerid = '<<current resellerid on application>>'
                    //and cont.ContactType = 4

                    /*NEW*/
                    //select con.ContactID,con.ContactName, con.ContactEmail, con.ContactAddress, con.ContactDOB, con.ContactDriversLicNo, 
                    //con.ContactPhone, cont.ContactDescription, con.IsContactSignatory, con.ResellerID from Contacts con
                    //inner join ContactsTypes cont on cont.ContactType = con.ContactType
                    //where con.resellerid = '<<current resellerid on application>>'
                    //and cont.ContactType = 4


                    /****LINQ****/
                    //List<GuarantorsSelectListResult> _guarantors = (from conx in _contactsXrefRepository.Table
                    //                                                join con in _contactsRepository.Table on conx.ContactID equals con.ContactID
                    //                                                join cont in _contactTypesRepository.Table on con.ContactType equals cont.ContactType
                    List<GuarantorsSelectListResult> _guarantors = (from con in _contactsRepository.Table
                                                                    join cont in _contactTypesRepository.Table on con.ContactType equals cont.ContactType
                                                                    where con.ResellerID == resellerId && cont.ContactType == 4
                                                                    select new GuarantorsSelectListResult
                                                                    {
                                                                        ContactID = con.ContactID,
                                                                        ContactName = !string.IsNullOrEmpty(con.ContactName) ? con.ContactName : "N/A",
                                                                        ContactEmail = con.ContactEmail,
                                                                        ContactAddress = con.ContactAddress,
                                                                        ContactDOB = con.ContactDOB == null ? new DateTime() : con.ContactDOB,
                                                                        ContactDriversLicNo = con.ContactDriversLicNo,
                                                                        ContactPhone = con.ContactPhone,
                                                                        ContactDescription = cont.ContactDescription,
                                                                        IsContactSignatory = con.IsContactSignatory,
                                                                        ResellerID = con.ResellerID

                                                                    }).OrderBy(x => x.ContactName).ToList();


                    #endregion

                    if (_guarantors != null && _guarantors.Count > 0)
                    {
                        foreach (var _guarantor in _guarantors)
                        {
                            var item = new ExistingContactsSelectListModel();
                            //select 
                            item.ContactID = _guarantor.ContactID;
                            item.ContactName = _guarantor.ContactName;

                            //map;
                            item.ResellerID = _guarantor.ResellerID;
                            item.ContactID = _guarantor.ContactID;
                            item.ContactName = _guarantor.ContactName;
                            item.ContactEmail = _guarantor.ContactEmail;
                            item.ContactAddress = _guarantor.ContactAddress;
                            item.ContactDOB = _guarantor.ContactDOB;
                            item.ContactDriversLicNo = _guarantor.ContactDriversLicNo;
                            item.ContactPhone = _guarantor.ContactPhone;
                            item.ContactTypeDescription = _guarantor.ContactDescription;
                            item.IsContactSignatory = _guarantor.IsContactSignatory;
                            item.ContactType = (int)ContactTypeTabs.Guarantors;
                            item.ContactABNACN = string.Empty;

                            collection.Add(item);
                        }

                    }

                    #endregion

                }
                else if (tabId == (int)ContactTypeTabs.Trustees)
                {
                    #region trustees

                    #region Client Query

                    /****SQL****/
                    //select con.ContactName, con.ContactEmail, con.ContactAddress, con.ContactDOB, con.ContactDriversLicNo, con.ContactPhone, 
                    //cont.ContactDescription, con.IsContactSignatory, con.ResellerID from ContactsXref conx
                    //inner join Contacts con on con.ContactID = conx.ContactID
                    //inner join ContactsTypes cont on cont.ContactType = con.ContactType
                    //where con.resellerid = '<<current resellerid on application>>'
                    //and cont.ContactType = 5

                    /*NEW*/
                    //select con.ContactName, con.ContactEmail, con.ContactAddress, con.ContactDOB, con.ContactDriversLicNo, con.ContactPhone, 
                    //cont.ContactDescription, con.IsContactSignatory, con.ResellerID from Contacts con
                    //inner join ContactsTypes cont on cont.ContactType = con.ContactType
                    //where con.resellerid = '<<current resellerid on application>>'
                    //and cont.ContactType = 5

                    /****LINQ****/
                    List<TrusteesSelectListResult> _trustees = (from con in _contactsRepository.Table
                                                                join cont in _contactTypesRepository.Table on con.ContactType equals cont.ContactType
                                                                where con.ResellerID == resellerId && cont.ContactType == 5
                                                                select new TrusteesSelectListResult
                                                                {
                                                                    ContactID = con.ContactID,
                                                                    ContactName = !string.IsNullOrEmpty(con.ContactName) ? con.ContactName : "N/A",
                                                                    ContactEmail = con.ContactEmail,
                                                                    ContactAddress = con.ContactAddress,
                                                                    ContactDOB = con.ContactDOB == null ? new DateTime() : con.ContactDOB,
                                                                    ContactDriversLicNo = con.ContactDriversLicNo,
                                                                    ContactPhone = con.ContactPhone,
                                                                    ContactDescription = cont.ContactDescription,
                                                                    IsContactSignatory = con.IsContactSignatory,
                                                                    ContactABN = con.ContactABNACN

                                                                }).OrderBy(x => x.ContactName).ToList();


                    #endregion

                    //get data

                    if (_trustees != null && _trustees.Count > 0)
                    {
                        foreach (var _trustee in _trustees)
                        {
                            var item = new ExistingContactsSelectListModel();
                            //select 
                            item.ContactID = _trustee.ContactID;
                            item.ContactName = _trustee.ContactName;

                            //map
                            item.ResellerID = _trustee.ResellerID;
                            item.ContactID = _trustee.ContactID;
                            item.ContactName = _trustee.ContactName;
                            item.ContactEmail = _trustee.ContactEmail;
                            item.ContactAddress = _trustee.ContactAddress;
                            item.ContactDOB = _trustee.ContactDOB;
                            item.ContactDriversLicNo = _trustee.ContactDriversLicNo;
                            item.ContactPhone = _trustee.ContactPhone;
                            item.ContactTypeDescription = _trustee.ContactDescription;
                            item.IsContactSignatory = _trustee.IsContactSignatory;
                            item.ContactType = (int)ContactTypeTabs.Trustees;
                            item.ContactABNACN = _trustee.ContactABN;
                            collection.Add(item);
                        }
                    }

                    #endregion

                }
                else if (tabId == (int)ContactTypeTabs.Owners)
                {
                    #region owners
                    #region Client Query

                    /****SQL****/
                    //select con.ContactName, con.ContactEmail, con.ContactAddress, con.ContactDOB, con.ContactDriversLicNo, con.ContactPhone, 
                    //cont.ContactDescription, con.IsContactSignatory, con.ResellerID from ContactsXref conx
                    //inner join Contacts con on con.ContactID = conx.ContactID
                    //inner join ContactsTypes cont on cont.ContactType = con.ContactType
                    //where con.resellerid = '<<current resellerid on application>>'
                    //and cont.ContactType = 7

                    //select con.ContactName, con.ContactEmail, con.ContactAddress, con.ContactDOB, con.ContactDriversLicNo, con.ContactPhone, 
                    //cont.ContactDescription, con.IsContactSignatory, con.ResellerID from Contacts con
                    //inner join ContactsTypes cont on cont.ContactType = con.ContactType
                    //where con.resellerid = '<<current resellerid on application>>'
                    //and cont.ContactType = 7



                    /****LINQ****/
                    List<OwnerSelectListResult> _owners = (from con in _contactsRepository.Table
                                                           join cont in _contactTypesRepository.Table on con.ContactType equals cont.ContactType
                                                           where con.ResellerID == resellerId && cont.ContactType == 7
                                                           select new OwnerSelectListResult
                                                           {
                                                               ContactID = con.ContactID,
                                                               ResellerID = con.ResellerID,
                                                               ContactName = !string.IsNullOrEmpty(con.ContactName) ? con.ContactName : "N/A",
                                                               ContactEmail = con.ContactEmail,
                                                               ContactAddress = con.ContactAddress,
                                                               ContactDOB = con.ContactDOB == null ? new DateTime() : con.ContactDOB,
                                                               ContactDriversLicNo = con.ContactDriversLicNo,
                                                               ContactPhone = con.ContactPhone,
                                                               ContactDescription = cont.ContactDescription,
                                                               IsContactSignatory = con.IsContactSignatory

                                                           }).OrderBy(x => x.ContactName).ToList();


                    #endregion

                    //get data
                    if (_owners != null && _owners.Count > 0)
                    {
                        foreach (var _owner in _owners)
                        {
                            var item = new ExistingContactsSelectListModel();
                            //select 
                            item.ContactID = _owner.ContactID;
                            item.ContactName = _owner.ContactName;

                            //map;
                            item.ResellerID = _owner.ResellerID;
                            item.ContactID = _owner.ContactID;
                            item.ContactName = _owner.ContactName;
                            item.ContactEmail = _owner.ContactEmail;
                            item.ContactAddress = _owner.ContactAddress;
                            item.ContactDOB = _owner.ContactDOB;
                            item.ContactDriversLicNo = _owner.ContactDriversLicNo;
                            item.ContactPhone = _owner.ContactPhone;
                            item.ContactTypeDescription = _owner.ContactDescription;
                            item.IsContactSignatory = _owner.IsContactSignatory;
                            item.ContactType = (int)ContactTypeTabs.Owners;
                            item.ContactABNACN = string.Empty;

                            collection.Add(item);
                        }
                    }
                    #endregion
                }

                response.ExistingContacts = collection;
                response.ErrorMessage = string.Empty;
                response.HasError = false;

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ContactsResponseModel SaveContacts(ContactsRequestModel request)
        {
            var response = new ContactsResponseModel();
            try
            {
                if (request != null)
                {
                    bool isCommit = false;

                    Contacts contact = null;
                    if (!string.IsNullOrEmpty(request.ContactID))
                    {
                        try
                        {
                            contact = _contactsRepository.Table.Where(a => a.ContactID == request.ContactID).FirstOrDefault();
                        }
                        catch (Exception)
                        {

                            var tempContact = (from c in _contactsRepository.Table
                                               where c.ContactID == request.ContactID
                                               select new TempContact
                                               {
                                                   ContactID = c.ContactID,
                                                   ContactEmail = c.ContactEmail,
                                                   ResellerID = c.ResellerID,
                                                   ContactType = c.ContactType,
                                                   ContactName = c.ContactName,
                                                   ContactDOB = c.ContactDOB == null ? "" : c.ContactDOB.ToString(),
                                                   ContactAddress = c.ContactAddress,
                                                   ContactDriversLicNo = c.ContactDriversLicNo,
                                                   ContactABNACN = c.ContactABNACN,
                                                   ContactPosition = c.ContactPosition,
                                                   IsContactSignatory = c.IsContactSignatory,
                                                   ContactPhone = c.ContactPhone
                                               }).FirstOrDefault();

                            contact = getContactByTempContact(tempContact);
                        }
                        //if exist
                        if (contact != null)
                        {
                            //map
                            //contact.ApplicationNumber = request.ContactName;
                            contact.ContactEmail = request.ContactEmail;
                            contact.ResellerID = request.ResellerID;
                            contact.ContactType = request.ContactType;
                            contact.ContactName = request.ContactName;
                            if (!string.IsNullOrEmpty(request.ContactDOB))
                                contact.ContactDOB = Convert.ToDateTime(request.ContactDOB);
                            contact.ContactAddress = request.ContactAddress;
                            contact.ContactDriversLicNo = request.ContactDriversLicNo;
                            contact.ContactABNACN = request.ContactABNACN;
                            contact.ContactPosition = request.ContactPosition;
                            contact.IsContactSignatory = request.IsContactSignatory;
                            contact.ContactPhone = request.ContactPhone;

                            //update in DB
                            _contactsRepository.Update(contact);
                            response.ContactID = contact.ContactID;

                            isCommit = true;

                        }
                    }
                    else
                    {
                        //case of save contact - Insert dbo.contacts
                        contact = new Web.Models.DBModel.Contacts();
                        contact.ContactID = Guid.NewGuid().ToString();
                        //contact.ApplicationNumber = request.ContactName;
                        contact.ContactEmail = request.ContactEmail;
                        contact.ResellerID = request.ResellerID;
                        contact.ContactType = request.ContactType;
                        contact.ContactName = request.ContactName;
                        if (!string.IsNullOrEmpty(request.ContactDOB))
                            contact.ContactDOB = Convert.ToDateTime(request.ContactDOB);
                        contact.ContactAddress = request.ContactAddress;
                        contact.ContactDriversLicNo = request.ContactDriversLicNo;
                        contact.ContactABNACN = request.ContactABNACN;
                        contact.ContactPosition = request.ContactPosition;
                        contact.IsContactSignatory = request.IsContactSignatory;
                        contact.ContactPhone = request.ContactPhone;

                        //created in DB
                        _contactsRepository.Insert(contact);

                        isCommit = true;
                    }

                    if (isCommit)
                    {
                        var existContactXref = _contactsXrefRepository.Table.Where(a => a.ContactID == request.ContactID && a.ApplicationNumber == request.ApplicationNumber).FirstOrDefault();

                        if (existContactXref == null)
                        {
                            //when the contact is created, insert a new row into dbo.contactsxref using the new contact ID and the current application id.
                            var contactsXref = new Web.Models.DBModel.ContactsXref();
                            contactsXref.ApplicationNumber = request.ApplicationNumber;
                            contactsXref.ContactID = contact.ContactID;
                            //created in DB
                            _contactsXrefRepository.Insert(contactsXref);
                        }

                    }
                    response.ContactID = contact.ContactID;
                    response.HasError = false;
                    response.ErrorMessage = "Successed";
                }
                else
                {
                    response.HasError = true;
                    response.ErrorMessage = "Failed";
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ContactsResponseModel DeleteContacts(string contactID)
        {
            var response = new ContactsResponseModel();
            try
            {
                if (!string.IsNullOrEmpty(contactID))
                {

                    try
                    {
                        var contact = _contactsRepository.Table.Where(a => a.ContactID == contactID).FirstOrDefault();

                        if (contact != null)
                        {

                            //deleted 
                            _contactsRepository.Delete(contact);

                            //get context
                            var contactsXref = _contactsXrefRepository.Table.Where(a => a.ContactID == contactID).FirstOrDefault();

                            if (contactsXref != null)
                            {
                                //Deleted in DB
                                _contactsXrefRepository.Delete(contactsXref);

                                response.ContactID = contactID;
                            }
                            response.HasError = false;
                            response.ErrorMessage = "Successed";
                        }
                        else
                        {
                            response.HasError = true;
                            response.ErrorMessage = "Contact Not Found";
                        }
                    }
                    catch
                    {

                        var contact = (from c in _contactsRepository.Table
                                       where c.ContactID == contactID
                                       select new TempContact
                                       {
                                           ContactID = c.ContactID,
                                           ContactEmail = c.ContactEmail,
                                           ResellerID = c.ResellerID,
                                           ContactType = c.ContactType,
                                           ContactName = c.ContactName,
                                           ContactDOB = c.ContactDOB == null ? "" : c.ContactDOB.ToString(),
                                           ContactAddress = c.ContactAddress,
                                           ContactDriversLicNo = c.ContactDriversLicNo,
                                           ContactABNACN = c.ContactABNACN,
                                           ContactPosition = c.ContactPosition,
                                           IsContactSignatory = c.IsContactSignatory,
                                           ContactPhone = c.ContactPhone
                                       }).FirstOrDefault();

                        if (contact != null)
                        {
                            var tempContact = getContactByTempContact(contact);

                            //Deleted in DB
                            _contactsRepository.AttachAndDelete(tempContact);

                            //get context
                            var contactsXref = _contactsXrefRepository.Table.Where(a => a.ContactID == contactID).FirstOrDefault();

                            if (contactsXref != null)
                            {
                                //Deleted in DB
                                _contactsXrefRepository.Delete(contactsXref);

                                response.ContactID = contactID;
                            }
                            response.HasError = false;
                            response.ErrorMessage = "Successed";
                        }
                        else
                        {
                            response.HasError = true;
                            response.ErrorMessage = "Contact Not Found";
                        }



                    }
                }
                else
                {
                    response.HasError = true;
                    response.ErrorMessage = "Invalid Contact Id";
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        private Contacts getContactByTempContact(TempContact temp)
        {
            var objContact = new Contacts
            {
                ContactID = temp.ContactID,
                ContactEmail = temp.ContactEmail,
                ResellerID = temp.ResellerID,
                ContactType = temp.ContactType,
                ContactName = temp.ContactName,
                ContactDOB = temp.ContactDOB == null ? new DateTime() : Convert.ToDateTime(temp.ContactDOB),
                ContactAddress = temp.ContactAddress,
                ContactDriversLicNo = temp.ContactDriversLicNo,
                ContactABNACN = temp.ContactABNACN,
                ContactPosition = temp.ContactPosition,
                IsContactSignatory = temp.IsContactSignatory,
                ContactPhone = temp.ContactPhone
            };

            return objContact;
        }

        #endregion

        #region CreateApplication - HPEFS - Notes and Logic

        #region Create Application

        /// <summary>
        /// Check if the FunderCode is ‘HPEFS’ (taken from dbo.Quotes.FinanceFunder, check dbo.Funder.FunderCode)
        /// </summary>
        /// <param name="funderId"></param>
        /// <returns></returns>
        private bool CheckFunderCode(int funderId)
        {
            bool isValidFunder = false;
            string _funderCode = Configuration.GetValue<string>("FunderCode");

            var funder = _fundersRepository.Table.Where(find => find.Id == funderId).FirstOrDefault();

            if (funder != null && funder.FunderCode == _funderCode)
            {
                isValidFunder = true;
            }
            return isValidFunder;
        }

        /// <summary>
        /// Check that the quote amount (dbo.Quotes.QuoteTotal) is within Country Threshold
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private bool GetCountryThreshhold(decimal quoteTotal)
        {
            bool isValidThreshhold = false;
            try
            {
                //get config
                var config = _configRepository.Table.ToList();
                //Get value
                var minimum = config.Where(x => x.name == "CountryMinimumThreshold").FirstOrDefault();
                var maximum = config.Where(x => x.name == "CountryMaximumThreshold").FirstOrDefault();
                //set value
                decimal minimumThresholdValue = minimum != null ? !string.IsNullOrEmpty(minimum.value) ? Convert.ToDecimal(minimum.value) : 0 : 0;
                decimal maximumThresholdValue = maximum != null ? !string.IsNullOrEmpty(maximum.value) ? Convert.ToDecimal(maximum.value) : 0 : 0;
                //verify 
                if (quoteTotal >= minimumThresholdValue && quoteTotal <= maximumThresholdValue)
                {
                    isValidThreshhold = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isValidThreshhold;
        }

        /// <summary>
        /// Check if the Reseller is Accredited - You can determine which country is accredited by checking if the Reseller ie. 
        /// DBO.CustomerAUX.AccreditationStatus is True.
        /// </summary>
        /// <param name="reselerId"></param>
        /// <returns></returns>
        private bool CheckResellerAccredited(string customerID)
        {
            bool accreditationStatus = true;//false

            try
            {
                var customerAux = _customerAuxRepository.Table.Where(find => find.CustomerID == customerID).FirstOrDefault();
                if (customerAux != null)
                    accreditationStatus = customerAux.AccreditationStatus == null ? false : (bool)customerAux.AccreditationStatus;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return accreditationStatus;
        }

        /// <summary>
        /// Update DBO.Quotes.Status = 7 (“End Customer Accepted”)
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        private bool UpdateQuoteStatus(int quoteId)
        {
            bool result = false;
            try
            {
                int quoteStatus = Convert.ToInt32(Configuration.GetValue<string>("EndCustomerAcceptedStatus"));
                //get approved quote information 
                Quotes quote = _quotesRepository.GetById(quoteId);

                // Update DBO.Quotes.Status = 7 (“End Customer Accepted”)
                quote.QuoteStatus = quoteStatus;
                //quote.QuoteLastModified = DateTime.Now.ToLocalTime();
                _quotesRepository.Update(quote);

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Update DBO.Quotes.Status = 7 (“End Customer Accepted”)
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        private bool UpdateQuoteStatus(int quoteId, int status)
        {
            bool result = false;
            try
            {
                //get approved quote information 
                Quotes quote = _quotesRepository.GetById(quoteId);

                // Update DBO.Quotes.Status = 7 (“End Customer Accepted”)
                quote.QuoteStatus = status;
                _quotesRepository.Update(quote);

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public ApplicationResponse VerifyCreatedApplication(int quoteId)
        {
            ApplicationResponse response = new ApplicationResponse();
            response.HasError = true;
            response.Result = "Failed";
            response.IsHPEFSFunder = false;
            response.apiErrorLogs = new List<string>();
            try
            {
                //get approved quote information 
                Quotes quote = _quotesRepository.GetById(quoteId);
                //save exist code status
                int quoteStatus = quote.QuoteStatus;

                //Check if the FunderCode is ‘HPEFS’
                bool isHPEFSFunder = CheckFunderCode(Convert.ToInt32(quote.FinanceFunder));
                if (isHPEFSFunder)
                {
                    response.IsHPEFSFunder = true;
                    //Check Country Threshold
                    bool isAmountVerified = GetCountryThreshhold(quote.QuoteTotal);
                    if (isAmountVerified)
                    {
                        //Check if the Reseller is Accredited
                        bool isResellerAccredited = CheckResellerAccredited(quote.ResellerAccount);
                        if (isResellerAccredited)
                        {
                            //update quote
                            bool isSetQuoteStatus = UpdateQuoteStatus(quoteId);

                            //Update DBO.Quotes.Status = 7
                            if (isSetQuoteStatus)
                            {
                                //create applications 
                                try
                                {
                                    int applicationStatus = Convert.ToInt32(Configuration.GetValue<string>("ApplicationCreatedStatus"));

                                    Applications application = new Applications();

                                    application.CreateDate = DateTime.Now;
                                    application.ApplicationNumber = quote.Id;
                                    application.ApplicationStatus = applicationStatus;
                                    //application.EntityType = ;//Get from EntityType.EnityType
                                    //application.EntityTypeOther = ;
                                    //application.EntityTrustName = ;
                                    //application.EntityTrustABN = ;
                                    //application.EntityTrustType = ;
                                    //application.EntityTrustOther = ;
                                    //application.EntityTrustOther = ;
                                    application.EndCustomerABN = quote.EndCustomerABN;
                                    application.EndCustomerPhone = quote.EndCustomerContactPhone;
                                    //application.EndCustomerFax = ;
                                    application.FunderPlan = quote.FunderPlan;//dbo.FunderPlan.Planid
                                    application.QuoteId = quote.Id;//dbo.Quotes.ID
                                    application.QuoteTotal = quote.QuoteTotal;
                                    application.GoodsDescription = quote.QuoteName;

                                    //application.AveAnnualSales = 0;
                                    //application.IsGuarantorSecurity = 0;
                                    //application.IsGuarantorPropertyOwner = 0;
                                    //application.GuarantorSecurityValue = 0;
                                    //application.GuarantorSecurityOwing = 0;
                                    application.EndCustomerYearsTrading = quote.EndCustomerYearsTrading;

                                    //application.BusinessActivity = "";
                                    application.FinanceType = quote.QuoteFinanceType;//dbo.financetype.ID
                                    application.FinanceLink = "";
                                    application.FinanceDuration = quote.FinanceDuration;//dbo.quoteduration.id
                                    application.FinanceFrequency = quote.FinanceFrequency;
                                    application.FinanceValue = quote.FinanceValue;
                                    application.FinanceTotal = quote.QuoteTotal;
                                    application.FinanceFunder = quote.FinanceFunder;//dbo.funder.id

                                    application.EndCustomerName = quote.EndCustomerName;

                                    application.EndCustomerPrimaryAddressLine1 = quote.EndCustomerPrimaryAddressLine1;
                                    application.EndCustomerPrimaryAddressLine2 = quote.EndCustomerPrimaryAddressLine2;
                                    application.EndCustomerPrimaryCity = quote.EndCustomerPrimaryCity;
                                    application.EndCustomerPrimaryState = quote.EndCustomerPrimaryState;
                                    application.EndCustomerPrimaryCountry = quote.EndCustomerPrimaryCountry;
                                    //temprary passing hardcoded letter on we will get it from mapping table
                                    application.EndCustomerPrimaryCountry = "AU";

                                    application.EndCustomerPrimaryPostcode = quote.EndCustomerPrimaryPostcode;

                                    application.EndCustomerDeliveryAddressLine1 = quote.EndCustomerDeliveryAddressLine1;
                                    application.EndCustomerDeliveryAddressLine2 = quote.EndCustomerDeliveryAddressLine2;
                                    application.EndCustomerDeliveryCity = quote.EndCustomerDeliveryCity;
                                    application.EndCustomerDeliveryState = quote.EndCustomerDeliveryState;
                                    application.EndCustomerDeliveryCountry = quote.EndCustomerDeliveryCountry;
                                    application.EndCustomerDeliveryPostcode = quote.EndCustomerDeliveryPostcode;

                                    //application.EndCustomerPostalAddressLine1 = "";
                                    //application.EndCustomerPostalAddressLine2 = "";
                                    //application.EndCustomerPostalCity = "";
                                    //application.EndCustomerPostalState = "";
                                    //application.EndCustomerPostalCountry = "";
                                    //application.EndCustomerPostalPostcode = "";

                                    application.EndCustomerType = quote.EndCustomerType;//Get from EntityType.EnityType
                                    application.EndCustomerContactName = quote.EndCustomerContactName;
                                    application.EndCustomerContactPhone = quote.EndCustomerContactPhone;
                                    application.EndCustomerContactEmail = quote.EndCustomerContactEmail;

                                    //application.EndCustomerSignatoryName = "";
                                    //application.EndCustomerSignatoryPosition = "";

                                    application.IMFSContactName = quote.QuoteCreatedBy;
                                    application.IMFSContactEmail = quote.QuoteCreatedBy;// extracted from dbo.quotes.quotecreatedby
                                    application.IMFSContactPhone = quote.QuoteCreatedBy;//extracted from dbo.quotes.quotecreatedby

                                    application.ResellerID = quote.ResellerAccount;// quote.ResellerID; TBD
                                    application.ResellerName = quote.ResellerName;
                                    application.ResellerContactName = quote.ResellerContactName;

                                    application.ResellerContactEmail = quote.ResellerContactEmail;//extracted from dbo.quotes.
                                    application.ResellerContactPhone = quote.ResellerContactPhone;//extracted from dbo.quotes.

                                    //application.IsApplicationSigned = ;


                                    _applicationsRepository.Insert(application);

                                    //string str = JsonNew(response.ApplicationId);

                                    //call combo request
                                    var apiResponse = CallCCRAPI(application.Id, quoteId);
                                    response.ACAPIModel = apiResponse.ACAPIModel;
                                    response.GCRSAPIModel = apiResponse.GCRSAPIModel;
                                    response.GDSAPIModel = apiResponse.GDSAPIModel;
                                    response.ErrorMessage = apiResponse.Message;
                                    response.apiErrorLogs = apiResponse.apiErrorLog;
                                    if (apiResponse.Error)
                                    {
                                        UpdateQuoteStatus(quoteId, quoteStatus);
                                        response.ErrorMessage = apiResponse.Message;
                                    }
                                    else
                                    {
                                        //bind response 
                                        response.HasError = false;
                                        response.ApplicationId = application.Id;
                                        response.Result = "Succeed";
                                        if (string.IsNullOrEmpty(response.ErrorMessage))
                                        {
                                            response.ErrorMessage = "";
                                        }
                                    }
                                }
                                catch
                                {
                                    //role back qute status
                                    UpdateQuoteStatus(quoteId, quoteStatus);
                                    response.ErrorMessage = "Error in create application entity.";
                                }
                            }
                            else
                            {
                                response.ErrorMessage = "Error in update quote entity.";
                            }
                        }
                        else
                        {
                            response.ErrorMessage = "The Reseller is not accredited.";
                        }
                    }
                    else
                    {
                        response.ErrorMessage = "The quote total is not within the limit.";
                    }
                }
                else
                {
                    response.ErrorMessage = "This quote is  not belong to HPEFS Funder.";
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return response;
        }

        #endregion

        #region Create Combo Request

        /// <summary>
        /// Update DBO.Quotes.Status = 7 (“End Customer Accepted”)
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>

        /// <summary>
        /// Get Funder Plan Data
        /// </summary>
        /// <param name="funderPlanId"></param>
        /// <returns></returns>
        private FunderPlanDataRequest GetFunderPlanData(int? funderPlanId)
        {
            FunderPlanDataRequest _plan = new FunderPlanDataRequest();
            try
            {
                var plan = _funderPlanRepository.GetById(funderPlanId);
                if (plan != null)
                {
                    _plan.FunderPlan = plan.PlanDescription;

                }
            }
            catch
            {

            }
            return _plan;
        }

        /// <summary>
        /// Get Application Data
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        private ApplicationDataRequest GetApplicationData(int applicationId)
        {
            ApplicationDataRequest _application = new ApplicationDataRequest();
            try
            {
                var application = _applicationsRepository.GetById(applicationId);
                if (application != null)
                {
                    //map model to entity
                    _application.ApplicationId = application.Id;
                    _application.EndCustomerContactName = application.EndCustomerContactName;
                    _application.EndCustomerContactEmail = application.EndCustomerContactEmail;
                    _application.EndCustomerName = application.EndCustomerName;
                    _application.EndCustomerPrimaryAddressLine1 = application.EndCustomerPrimaryAddressLine1;
                    _application.EndCustomerPrimaryCity = application.EndCustomerPrimaryCity;
                    _application.EndCustomerPrimaryState = application.EndCustomerPrimaryState;
                    _application.EndCustomerPrimaryPostcode = application.EndCustomerPrimaryPostcode;
                    _application.EndCustomerPrimaryCountry = application.EndCustomerPrimaryCountry;
                    _application.EndCustomerABN = application.EndCustomerABN;
                    _application.FinanceDuration = application.FinanceDuration;
                    _application.DirectCustomer = true;
                    _application.EndCustomerDeliveryAddressLine1 = application.EndCustomerDeliveryAddressLine1;
                    _application.EndCustomerDeliveryAddressLine2 = application.EndCustomerDeliveryAddressLine2;
                    _application.EndCustomerDeliveryCity = application.EndCustomerDeliveryCity;
                    _application.EndCustomerDeliveryState = application.EndCustomerDeliveryState;
                    _application.EndCustomerDeliveryPostcode = application.EndCustomerDeliveryPostcode;
                    _application.EndCustomerDeliveryCountry = application.EndCustomerDeliveryCountry;
                    _application.PublicSectorIndicator = false;
                    _application.FinanceType = application.FinanceType;
                    _application.FinanceFrequency = application.FinanceFrequency;
                    _application.FunderPlan = application.FunderPlan;
                    _application.QuoteID = application.QuoteId;
                    _application.ResellerID = application.ResellerID;

                }
            }
            catch
            {

            }
            return _application;
        }

        /// <summary>
        /// Get Config Data Request
        /// </summary>
        /// <returns></returns>
        private ConfigDataRequest GetConfigDataRequest()
        {
            ConfigDataRequest config = null;
            try
            {
                config = new ConfigDataRequest();
                config.CurrencyCode = Configuration.GetValue<string>("CurrencyCode");
                config.CountryCode = Configuration.GetValue<string>("CountryCode");

            }
            catch (Exception)
            {

                throw;
            }
            return config;
        }

        /// <summary>
        /// Get Quote Data Request
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        private QuoteDataRequest GetQuoteDataRequest(int quoteId)
        {
            QuoteDataRequest _quote = new QuoteDataRequest();
            try
            {
                Quotes quote = _quotesRepository.GetById(quoteId);
                if (quote != null)
                {
                    _quote.QuoteLine = new List<QuoteLineDataRequest>();

                    var _quoteLines = _quoteLinesRepository.Table.Where(x => x.QuoteId == quoteId).ToList();

                    if (_quoteLines != null && _quoteLines.Count > 0)
                    {
                        var quoteLines = new List<QuoteLineDataRequest>();
                        foreach (var _quoteLine in _quoteLines)
                        {
                            var quoteLine = new QuoteLineDataRequest();
                            // get category from productXref table based on vpn
                            var pctCategory = _productXrefRepository.Table.Where(x => x.InternalSKUID == _quoteLine.SKU).FirstOrDefault()?.HPFSCategory;

                            //bind
                            quoteLine.ManufacturerPartNumber = _quoteLine.VPN;
                            quoteLine.UnitPrice = _quoteLine.ResellerSellPrice;
                            quoteLine.Quantity = _quoteLine.Qty ?? 0;
                            quoteLine.PCTCategory = pctCategory;

                            quoteLines.Add(quoteLine);
                        }
                        _quote.QuoteLine = quoteLines;
                    }
                }

            }
            catch (Exception ex)
            {
                doLog("GetQuoteDataRequest", "Error", ex.Message);
            }
            return _quote;
        }

        /// <summary>
        /// Get Quote Data Request
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        private CustomerAuxDataRequest GetCustomerDataRequest(string customerID)
        {
            CustomerAuxDataRequest _customerAux = new CustomerAuxDataRequest();
            try
            {
                var customerAux = _customerAuxRepository.Table.Where(find => find.CustomerID == customerID).FirstOrDefault();
                if (customerAux != null)
                {
                    _customerAux.PartnerID = customerAux.PartnerID;
                }

            }
            catch (Exception)
            {

                throw;
            }
            return _customerAux;
        }

        /// <summary>
        /// Call HP API CreateComboRequest
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// Call HP API CreateComboRequest
        /// </summary>
        /// <returns></returns>
        private GetCCRequest BindCCRRequest(Applications application, Quotes quotes)
        {
            var request = new GetCCRequest();
            try
            {
                string resellerAccount = quotes?.ResellerAccount;
                string strPartnerSupplierId = _customerAuxRepository.Table.Where(x => x.CustomerID == resellerAccount).FirstOrDefault()?.PartnerID;
                long parnerSupplierId;
                long.TryParse(strPartnerSupplierId, out parnerSupplierId);

                var config = GetConfigDataRequest();
                var quote = GetQuoteDataRequest(application.QuoteId);
                var funder = GetFunderPlanData(application.FunderPlan);
                var customerAux = GetCustomerDataRequest(application.ResellerID);
                #region Transaction Detail

                var transactionDetail = new GetCCRTransactionDetail();
                //map
                transactionDetail.TransactionId = "TEST ABC FOR Suppliers";
                transactionDetail.ClientProgramId = "IngramTest";
                transactionDetail.PartnerSupplierId = parnerSupplierId;// 5142739112;
                transactionDetail.PartnerSupplierName = quotes?.ResellerName;//"ARTREF PTY LTD";
                transactionDetail.DistributorId = 5423912845;
                transactionDetail.DistributorName = "Ingrammicro";
                transactionDetail.RelationshipCode = "HPE";
                transactionDetail.LanguageCode = "EN";
                //bind
                request.TransactionDetail = transactionDetail;

                #endregion

                #region SourceEntryPerson

                var sourceEntryPerson = new GetCCRSourceEntryPerson();
                //map
                sourceEntryPerson.FirstName = "IMFS";
                sourceEntryPerson.LastName = "Portal";
                sourceEntryPerson.Email = "AU-IMFSPortal@ingrammicro.com";
                //sourceEntryPerson.Phone = "9999999999";
                //bind
                request.SourceEntryPerson = sourceEntryPerson;

                #endregion

                #region PartnerRep

                var partnerRep = new GetCCRPartnerRep();
                //map
                partnerRep.ExistingFamID = "0";
                partnerRep.PartnerRepID = 0;
                partnerRep.PartnerRepFirstName = "IMFS";
                partnerRep.PartnerRepLastName = "Portal";
                partnerRep.PartnerRepPhone = "9999999999";
                partnerRep.PartnerRepEmail = "AU-IMFSPortal@ingrammicro.com";
                //bind
                request.PartnerRep = partnerRep;

                #endregion

                #region CCR API Request details
                GetCCRAPIRequestDetails getCCRAPIRequestDetails = new GetCCRAPIRequestDetails();

                #region CreateCCRequest

                CreateCCRequest cCRequest = new CreateCCRequest();

                cCRequest.CustomerContactFirstName = application.EndCustomerContactName;
                cCRequest.CustomerContactemail = application.EndCustomerContactEmail;
                cCRequest.CustomerLegalName = application.EndCustomerName;
                #region CCRCustomerAddress
                CCRCustomerAddress cCRCustomerAddress = new CCRCustomerAddress();
                //map fields
                cCRCustomerAddress.AddressLine1 = application.EndCustomerPrimaryAddressLine1;
                cCRCustomerAddress.City = application.EndCustomerPrimaryCity;
                cCRCustomerAddress.StateCode = application.EndCustomerPrimaryState;
                cCRCustomerAddress.PostalCode = application.EndCustomerPrimaryPostcode;
                cCRCustomerAddress.CountryCodeIso2 = application.EndCustomerPrimaryCountry;
                #endregion
                cCRequest.CustomerAddress = cCRCustomerAddress;
                cCRequest.CustomerTaxOrRegistrationId = application.EndCustomerABN;
                cCRequest.DNBNumber = "";
                cCRequest.FinaceTerm = application.FinanceDuration;
                cCRequest.DirectCustomer = true;

                getCCRAPIRequestDetails.CreateCreditRequestRequest = cCRequest;
                #endregion

                #region Equipment
                CCREquipment cCREquipment = new CCREquipment();

                #region Installtion list
                List<CCRInstallLocation> installLocations = new List<CCRInstallLocation>();

                CCRInstallLocation cCRInstall = new CCRInstallLocation();
                #region Address

                var address = new CCRAddress();
                address.AddressLine1 = !string.IsNullOrEmpty(application.EndCustomerDeliveryAddressLine1) ? application.EndCustomerDeliveryAddressLine1 : "";
                address.AddressLine2 = !string.IsNullOrEmpty(application.EndCustomerDeliveryAddressLine2) ? application.EndCustomerDeliveryAddressLine2 : "";
                address.City = !string.IsNullOrEmpty(application.EndCustomerDeliveryCity) ? application.EndCustomerDeliveryCity : "";
                address.StateCode = !string.IsNullOrEmpty(application.EndCustomerDeliveryState) ? application.EndCustomerDeliveryState : "";
                address.PostalCode = !string.IsNullOrEmpty(application.EndCustomerDeliveryPostcode) ? application.EndCustomerDeliveryPostcode : "";
                address.CountryCodeIso2 = !string.IsNullOrEmpty(application.EndCustomerDeliveryCountry) ? application.EndCustomerDeliveryCountry : "";

                cCRInstall.Address = address;
                #endregion

                installLocations.Add(cCRInstall);
                #endregion

                cCREquipment.InstallLocations = installLocations;
                #endregion
                getCCRAPIRequestDetails.Equipment = cCREquipment;

                getCCRAPIRequestDetails.CurrencyCode = config.CurrencyCode;
                getCCRAPIRequestDetails.CountryCode = config.CountryCode;

                #region GetFirmQuoteRequest
                GetCCRFirmQuoteRequest getCCRFirmQuoteRequest = new GetCCRFirmQuoteRequest();
                getCCRFirmQuoteRequest.PublicSectorIndicator = false;
                getCCRFirmQuoteRequest.LeaseTerm = application.FinanceDuration;

                string leaseType = "FMV";
                if (application.FinanceType == "1")
                {
                    leaseType = "FL";
                }
                else if (application.FinanceType == "2")
                {
                    leaseType = "FMV";
                }
                else if (application.FinanceType == "4")
                {
                    leaseType = "LN";
                }
                getCCRFirmQuoteRequest.LeaseType = leaseType;

                string finDuration = string.Empty;
                if (application.FinanceFrequency == "Monthly")
                {
                    finDuration = "MON";
                }
                else if (application.FinanceFrequency == "Quarterly")
                {
                    finDuration = "QUA";
                }
                else if (application.FinanceFrequency == "Yearly")
                {
                    finDuration = "ANU";
                }
                else
                {
                    finDuration = "ALL";
                }
                getCCRFirmQuoteRequest.PaymentFrequency = finDuration;
                getCCRFirmQuoteRequest.ProgramType = "ADV";
                #region Equipment
                //map
                List<CCREquipment> equipments = new List<CCREquipment>();

                //quote line
                foreach (var quoteLine in quote.QuoteLine)
                {
                    CCREquipment firmEquipment = new CCREquipment();
                    //map
                    firmEquipment.PCTCategory = quoteLine.PCTCategory;
                    firmEquipment.ProductDescription = quoteLine.ProductDescription;
                    firmEquipment.BundleId = quoteLine.BundleId;
                    firmEquipment.ProductManufacturer = quoteLine.ProductManufacturer;
                    firmEquipment.ManufacturerProductLine = quoteLine.ManufacturerProductLine;
                    firmEquipment.ManufacturerPartNumber = quoteLine.ManufacturerPartNumber;
                    firmEquipment.UnitPrice = quoteLine?.UnitPrice ?? 0;
                    firmEquipment.Quantity = quoteLine.Quantity;

                    equipments.Add(firmEquipment);
                }
                //bind
                //getCCRFirmQuoteRequest.Equipment = new List<FirmQuoteEquipment>();
                getCCRFirmQuoteRequest.Equipment = equipments;

                #endregion

                getCCRFirmQuoteRequest.Comments = "Whatever we put in for reference in case of";

                getCCRAPIRequestDetails.GetFirmQuoteRequest = getCCRFirmQuoteRequest;
                #endregion

                request.GetComboAPIRequestDetails = getCCRAPIRequestDetails;
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return request;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CreateComboResponse CallCCRAPI(int applicationId, int quoteId)
        {

            var response = new CreateComboResponse();
            response.Error = true;
            response.apiErrorLog = new List<string>();
            var ccrAPIModel = new HPEAPIRequestResponseObjects();
            try
            {
                string webAddr = "https://api-csc-stg-sgw.ext.hpe.com/gw/gfit/hpfs/pricingservice.rest.stg/3.0/CreateComboRequest";
                //string webAddr = "http://localhost:81/QuoteAcceptance/GetComboRequest";
                var httpWebRequest = System.Net.WebRequest.CreateHttp(webAddr);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";

                //Certificate 
                httpWebRequest.ClientCertificates.Add(GetCertificate());

                Applications applications = _applicationsRepository.GetById(applicationId);
                Quotes quote = _quotesRepository.GetById(quoteId);
                //bind data
                GetCCRequest requestData = BindCCRRequest(applications, quote);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(requestData);
                    //JsonNew(requestData);
                    //Log Request  - QuoteLogs
                    ccrAPIModel.Request = json;
                    //_quoteManager.InsertQuoteLog(Convert.ToInt32(quoteId), "API-CreateComboRequest-Request", json);
                    doLog("CCRAPI app num - " + applications.ApplicationNumber, "API-CreateComboRequest-Request", json);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
                try
                {
                    var httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string comboAPIResponse = streamReader.ReadToEnd();
                        //Log Response -QuoteLogs
                        //_quoteManager.InsertQuoteLog(Convert.ToInt32(quoteId), "API-CreateComboRequest-Response", comboAPIResponse);
                        doLog("CCRAPI app num - " + applications.ApplicationNumber, "API-CreateComboRequest-Response", comboAPIResponse);
                        ccrAPIModel.Response = comboAPIResponse;
                        var resultJson = JsonConvert.DeserializeObject<CreateComboResponse>(comboAPIResponse);

                        if (resultJson != null)
                        {
                            var resultStatus = resultJson.TransactionResult?.ResultSuccess ?? false;

                            if (resultStatus)
                            {

                                ////Update HPE Data
                                //ApplicationHPEUpdateRequest hpData = new ApplicationHPEUpdateRequest();
                                //hpData.ApplicationID = applicationId;
                                //hpData.QuoteId = quoteId;
                                //hpData.CreditApplicationID = resultJson.CreditApplicationID;
                                //hpData.RequestID = resultJson.RequestID;

                                applications.HPECreditApplicationID = Convert.ToInt32(resultJson.CreditApplicationID);
                                applications.HPERequestID = Convert.ToInt32(resultJson.RequestID);

                                _applicationsRepository.Update(applications);
                                bool hpDataResult = true;
                                //bool hpDataResult = UpdateApplicationHPEData(hpData);
                                response.Error = !hpDataResult;

                                //Call combo request status api
                                if (hpDataResult)
                                {
                                    //map new request 
                                    //ComboRequestForStatus nextRequest = new ComboRequestForStatus();

                                    //call deal status API with application Id
                                    var responseGDS = CallGDSAPI(applications);
                                    response.GDSAPIModel = responseGDS.GDSAPIModel;
                                    response.ACAPIModel = responseGDS.ACAPIModel;
                                    response.GCRSAPIModel = responseGDS.GCRSAPIModel;
                                    response.CCRAPIModel = ccrAPIModel;
                                    response.apiErrorLog = responseGDS.apiErrorLogs;


                                    response.Error = responseGDS.Error;
                                    response.Message = responseGDS.Message;
                                    //var nextResult = CreateGetComboRequestData(requestData, applications);
                                }
                            }
                            else
                            {
                                response.Error = true;
                                response.Message = resultJson?.TransactionResult?.Validation?.FirstOrDefault()?.ValidationText;
                            }
                        }
                        else
                        {
                            response.Error = true;
                            response.Message = resultJson?.TransactionResult?.Validation?.FirstOrDefault()?.ValidationText;
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "");
                    Console.WriteLine(response.Message);
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "");
                Console.WriteLine(ex.Message);
            }

            return response;
        }


        #endregion

        #region Get Combo Request

        private GetComboRequest BindGCRSRequestModel(GetDealStatusResponse dsresponse, Applications response, GetDealStatusRequest dealStatusRequestModel)
        {
            var getComboRequestStatus = new GetComboRequest();

            try
            {
                getComboRequestStatus.QuoteId = response.QuoteId;
                getComboRequestStatus.ApplicationId = response.Id;

                GetDealStatusList dsl = new GetDealStatusList();
                if (dsresponse != null && dsresponse.GetDealStatusList != null)
                {
                    dsl = dsresponse.GetDealStatusList.Where(x => x.ApplicationRef == (response.HPECreditApplicationID + ""))?.FirstOrDefault();
                }

                getComboRequestStatus.TransactionDetail = new TransactionDetailGCRS();

                var td = new TransactionDetailGCRS();
                td.TransactionId = dsresponse?.TransactionDetail?.TransactionId;
                td.ClientProgramId = dsresponse?.TransactionDetail?.ClientProgramId;
                //td.PartnerSupplierId = dsresponse?.TransactionDetail?.PartnerSupplierId ?? 0;
                td.PartnerSupplierId = dsl?.PartnerID ?? 0;
                td.PartnerSupplierName = dsl?.PartnerName;
                td.DistributorId = dsresponse?.TransactionDetail?.DistributorId ?? 0;
                td.DistributorName = dealStatusRequestModel?.TransactionDetail?.DistributorName;// "Ingrammicro";
                td.RelationshipCode = dsresponse?.TransactionDetail?.RelationshipCode;
                td.LanguageCode = dsresponse?.TransactionDetail?.LanguageCode;
                getComboRequestStatus.TransactionDetail = td;

                var cif = new ComboInformation();

                cif.CreditApplicationID = response.HPECreditApplicationID ?? 0;
                cif.RequestID = response.HPERequestID ?? 0;
                cif.SourceEntryPerson = new CIFSourceEntryPerson();

                var sep = new CIFSourceEntryPerson();
                sep.FirstName = dealStatusRequestModel?.SourceEntryPerson?.FirstName;// "IMFS";
                sep.LastName = dealStatusRequestModel?.SourceEntryPerson?.LastName; //"Portal";
                sep.Email = dealStatusRequestModel?.SourceEntryPerson?.Email;// "AU-IMFSPortal@ingrammicro.com";
                cif.SourceEntryPerson = sep;

                getComboRequestStatus.ComboInformation = cif;


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return getComboRequestStatus;
        }

        public GetComboResponse CallGCRSAPI(GetComboRequest request)
        {
            var response = new GetComboResponse();
            response.Error = true;
            var GCRSApiModel = new HPEAPIRequestResponseObjects();
            try
            {
                string webAddr = "https://api-csc-stg-sgw.ext.hpe.com/gw/gfit/hpfs/pricingservice.rest.stg/3.0/GetComboRequestStatus";
                //string webAddr = "http://localhost:81/QuoteAcceptance/GetComboRequestStatus";
                var _response = new GetComboAPIResponseType();
                var httpWebRequest = System.Net.WebRequest.CreateHttp(webAddr);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";

                //Certificate 
                httpWebRequest.ClientCertificates.Add(GetCertificate());

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string nextRequest = JsonConvert.SerializeObject(request); //BindPostData(request);
                    GCRSApiModel.Request = nextRequest;
                    //Log
                    //_quoteManager.InsertQuoteLog(Convert.ToInt32(request.QuoteId), "API-GetComboRequestStatus-Request", nextRequest);
                    doLog("For app number : " + request.QuoteId, "API-GetComboRequestStatus-Request", nextRequest);
                    streamWriter.Write(nextRequest);
                    streamWriter.Flush();
                }
                try
                {
                    var httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string responseData = streamReader.ReadToEnd();
                        GCRSApiModel.Response = responseData;
                        //Log
                        //_quoteManager.InsertQuoteLog(Convert.ToInt32(request.QuoteId), "API-GetComboRequestStatus-Response", responseData);
                        //responseData = "{\"GetFirmQuoteResponse\":{\"TransactionDetail\":{\"TransactionId\":\"Design\",\"ClientProgramId\":\"IngramTest\",\"HPEPartyId\":null,\"PartnerSupplierId\":5142739112,\"PartnerSupplierName\":\"ARTREF PTY LTD\",\"DistributorId\":5423912845,\"DistributorName\":\"Ingrammicro\",\"RelationshipCode\":\"HPE\",\"ClientAuthenticationKey\":\"\",\"LanguageCode\":\"en\"},\"TransactionResult\":{\"ResultSuccess\":true,\"Validation\":[],\"ValidationEquipmentMapping\":[]},\"AlternateDocumentList\":[{\"AlternateDocumentID\":\"5280416\",\"AlternateDocumentDesc\":\"Partner Connection BLA Lite\"}],\"DocumentSignatureRequiredActions\":{\"LeaseAgreementSignorEmail\":\"Required\",\"CertificateOfAcceptanceSignorEmail\":\"Optional\",\"AntiMoneyLaunderingSignorEmail\":\"Not Required\",\"DirectDebitACHSignorEmail\":\"Required\"},\"GetDetailedPricingResponseDetail\":{\"PricingRequestId\":6254965,\"LeasePaymentOptions\":[{\"QuoteNumber\":18414099,\"CreditApplicationNumber\":302878,\"Type\":\"FMV\",\"Program\":\"ADV\",\"PaymentTerm\":\"MON\",\"LeaseTerm\":\"36\",\"PaymentWithoutTax\":26.55,\"EstimatedTaxAmount\":0,\"PaymentAmount\":26.55,\"DownPaymentAmount\":0,\"PassThrough\":\"N\",\"QuotedStatus\":\"SUB\",\"QuoteEffectiveDate\":\"1\\/27\\/2023\",\"QuoteExpirationDate\":\"3\\/28\\/2023\",\"MarginUpliftPercentage\":0,\"Comments\":null}]}},\"ComboInformationResponse\":{\"SourceEntryPerson\":{\"FirstName\":\"IMFS\",\"LastName\":\"Portal\",\"Email\":\"AU-IMFSPortal@ingrammicro.com\",\"Phone\":0},\"RequestID\":6254950,\"CreditApplicationID\":302878,\"CreditDecisionCode\":\"APPVD\",\"CreditDecisionDescription\":\"Approved\"},\"CreditApplDocDetailType\":[],\"ConditionDetails\":{\"UpFrontPayment\":0,\"BankGuarnteeLetterOfCreditPcr\":0,\"TermUpperLimit\":null,\"ArrearsMonthlyNotPermitted\":false,\"ArrearsQuarterlyNotPermitted\":false,\"LeaseStructureNotPermitted\":null,\"Dys6090DfelInd\":false,\"Guarantors\":null,\"OtherCondition\":null,\"RevisedCreditAmount\":false},\"RevisedApprovedAmount\":\"1050.00\",\"RequestAmountCurrency\":\"AUD\"}";
                        doLog("For app number : " + request.QuoteId, "API-GetComboRequestStatus-Response", responseData);
                        response = JsonConvert.DeserializeObject<GetComboResponse>(responseData);
                        response.Error = false;
                        response.GCRSApiModel = GCRSApiModel;
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : ""); 
                    GCRSApiModel.Response = ex.Message + ((ex.InnerException != null) ? " InnerException - " + ex.InnerException.Message : "") + " " + ex.StackTrace;
                    response.GCRSApiModel = GCRSApiModel;
                    Console.WriteLine(ex.Message);
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "");
                Console.WriteLine(ex.Message);
            }

            return response;
        }


        #endregion

        #region Accept Quote
        private AcceptQuoteRequest BindACRequestModel(int quoteNumber, int creditApplicationID, long partnerSuppierId, string PartnerSupplierName)
        {
            AcceptQuoteRequest _request = new AcceptQuoteRequest();

            var td = new AcceptQuoteTransactionDetail();
            //_request.ApplicationId = creditApplicationID;
            //_request.QuoteId = quoteNumber;

            var TrnDetail = new AcceptQuoteTransactionDetail();
            TrnDetail.TransactionId = "TEST ABC FOR Suppliers";
            TrnDetail.ClientProgramId = "IngramTest";
            TrnDetail.PartnerSupplierId = partnerSuppierId; //5142739112;
            TrnDetail.PartnerSupplierName = PartnerSupplierName;//"ARTREF PTY LTD";
            TrnDetail.DistributorId = 5423912845;
            TrnDetail.DistributorName = "Ingrammicro";
            TrnDetail.RelationshipCode = "HPE";
            TrnDetail.LanguageCode = "EN";
            //bind
            _request.TransactionDetail = TrnDetail;


            _request.SourceEntryPerson = new AcceptQuoteSourceEntryPerson();

            var sep = new AcceptQuoteSourceEntryPerson();
            sep.FirstName = "IMFS";
            sep.LastName = "Portal";
            sep.Email = "AU-IMFSPortal@ingrammicro.com";
            _request.SourceEntryPerson = sep;

            _request.CreditApplicationID = creditApplicationID;
            _request.FamID = 0;
            _request.QuoteNumber = quoteNumber;
            _request.DocumenttoCustomer = true;
            _request.PaymentDelegationFlag = true;
            _request.UseAdobeEsign = true;

            _request.CustomerSignatoryApprover = new CustomerSignatoryApprover();
            var csa = new CustomerSignatoryApprover();
            csa.FirstName = "IMFS";
            csa.LastName = "Portal";
            csa.Email = "AU-IMFSPortal@ingrammicro.com";
            csa.Country = "AU";

            _request.CustomerSignatoryApprover = csa;
            _request.DocumentSignatureRequiredActions = new DocumentSignatureRequiredActions();
            var dsra = new DocumentSignatureRequiredActions();
            dsra.LeaseAgreementSignorEmail = "AU-IMFSPortal@ingrammicro.com";
            dsra.CertificateOfAcceptanceSignorEmail = "AU-IMFSPortal@ingrammicro.com";
            dsra.AntiMoneyLaunderingSignorEmail = "AU-IMFSPortal@ingrammicro.com";
            dsra.DirectDebitACHSignorEmail = "AU-IMFSPortal@ingrammicro.com";
            _request.DocumentSignatureRequiredActions = dsra;
            return _request;
        }

        private GetDealStatusRequest BindGDSRequestModel(string supplierId, string PartnerSupplierName, string CreditApplicationID)
        {
            //db call to get the PartnerSupplierId and PartnerSupplierName
            string PartnerSupplierId = _customerAuxRepository.Table.Where(x => x.CustomerID == supplierId).FirstOrDefault()?.PartnerID;
            long Supplierid;
            long.TryParse(PartnerSupplierId, out Supplierid);

            var getDealStatusRequest = new GetDealStatusRequest();
            var transactionDetail = new GetDealStatusTransactionDetail();
            transactionDetail.PartnerSupplierId = Supplierid;
            transactionDetail.ClientProgramId = "IngramTest";
            transactionDetail.TransactionId = "Design";
            transactionDetail.PartnerSupplierName = PartnerSupplierName;
            transactionDetail.DistributorId = 5423912845;
            transactionDetail.DistributorName = "Ingrammicro";
            transactionDetail.RelationshipCode = "HPE";
            transactionDetail.ClientAuthenticationKey = "";
            transactionDetail.LanguageCode = "en";

            getDealStatusRequest.TransactionDetail = transactionDetail;

            var sourceEntryPerson = new GetDealStatusSourceEntryPerson();
            sourceEntryPerson.FirstName = "IMFS";
            sourceEntryPerson.LastName = "Portal";
            sourceEntryPerson.Email = "AU-IMFSPortal@ingrammicro.com";
            sourceEntryPerson.Phone = "9086651054";

            getDealStatusRequest.SourceEntryPerson = sourceEntryPerson;

            var creditApllicationInfo = new GetDealStatusCreditApplicationInfo();
            creditApllicationInfo.CreditApplicationID = string.IsNullOrEmpty(CreditApplicationID) ? 0 : Convert.ToInt32(CreditApplicationID);

            getDealStatusRequest.CreditApplicationInfo = creditApllicationInfo;

            getDealStatusRequest.FromDate = "";
            getDealStatusRequest.ToDate = "";

            return getDealStatusRequest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public AcceptQuoteResponse CallACAPI(AcceptQuoteRequest model)
        {
            var response = new AcceptQuoteResponse();
            response.Error = true;

            var ACAPIModel = new HPEAPIRequestResponseObjects();
            try
            {
                string webAddr = "https://api-csc-stg-sgw.ext.hpe.com/gw/gfit/hpfs/pricingservice.rest.stg/3.0/AcceptQuote";
                //string webAddr = "http://localhost:81/QuoteAcceptance/GetAcceptQuoteComboRequest";
                //var _response = new GetComboAPIResponseType();
                var httpWebRequest = System.Net.WebRequest.CreateHttp(webAddr);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";

                //Certificate 
                httpWebRequest.ClientCertificates.Add(GetCertificate());

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string request = JsonConvert.SerializeObject(model);
                    ACAPIModel.Request = request;
                    //Log
                    //_quoteManager.InsertQuoteLog(Convert.ToInt32(model.QuoteId), "API-AcceptQuoteAPI-Request", request);
                    doLog("ACAPI app number - " + model.QuoteId, "API-AcceptQuoteAPI-Request", request);
                    streamWriter.Write(request);
                    streamWriter.Flush();
                }
                try
                {
                    var httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string responseData = streamReader.ReadToEnd();
                        ACAPIModel.Response = responseData;
                        //Log
                        //_quoteManager.InsertQuoteLog(Convert.ToInt32(model.QuoteId), "API-AcceptQuoteAPI-Response", responseData);
                        doLog("ACAPI app number - " + model.QuoteId, "API-AcceptQuoteAPI-Response", responseData);
                        var resultJson = JsonConvert.DeserializeObject<AcceptQuoteResponse>(responseData);
                        if (resultJson != null)
                        {
                            var trnResult = resultJson.AcceptQuoteResponseType?.TransactionResult;
                            if (trnResult != null)
                            {
                                if (trnResult.ResultSuccess)
                                {
                                    response.Error = false;
                                    //resultJson.AcceptQuoteResponseType.DocumentSignatureActionsProcessed.
                                    response = resultJson;
                                }
                                else
                                {
                                    response.Error = true;
                                    response.Message = "GetAcceptQuoteAPI - " + trnResult?.Validation?.FirstOrDefault()?.ValidationText;
                                }
                            }
                        }
                        else
                        {
                            response.Error = true;
                            response.Message = "GetAcceptQuoteAPI - " + resultJson?.AcceptQuoteResponseType?.TransactionResult?.Validation?.FirstOrDefault()?.ValidationText;
                        }
                        response.ACAPIModel = ACAPIModel;
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "");
                    Console.WriteLine(ex.Message);
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "");
                Console.WriteLine(ex.Message);
            }

            return response;
        }


        #endregion

        #region Get Deal Status
        public GetDealStatusAPIResponse GetDealStatusAPI()
        {
            var apiResponse = new GetDealStatusAPIResponse();
            apiResponse.Error = true;

            List<GDSResponseForFE> gDSResponseForFE = new List<GDSResponseForFE>();

            //get all pending applications
            List<int> statusIds = _statusRepository.Table
                .Where(x => x.Description == "Application Pending" || x.Description == "Application Created" || x.Description.Contains("Submitting Credit Request"))
                .Select(x => x.Id).ToList();

            //for loop for all the applications whose applicationStatus=11
            var pendingApplications = _applicationsRepository.Table.Where(x => statusIds.Contains(x.ApplicationStatus ?? 0) && x.HPECreditApplicationID > 0).ToList();

            foreach (var application in pendingApplications)
            {
                GDSResponseForFE gDSResponse = new GDSResponseForFE();
                apiResponse = CallGDSAPI(application);
                gDSResponse.GDSAPIModel = apiResponse.GDSAPIModel;
                gDSResponse.GCRSAPIModel = apiResponse.GCRSAPIModel;
                gDSResponse.ACAPIModel = apiResponse.ACAPIModel;
                gDSResponseForFE.Add(gDSResponse);
            }
            apiResponse.apiResponseLogs = gDSResponseForFE;
            //API response
            apiResponse.Message = $"{apiResponse.UpdatedApplicationCount} out of {pendingApplications.Count} applications updated";

            return apiResponse;
        }
        private string GetGCRSMailBody(Applications application, string emailbody)
        {
            try
            {
                emailbody = emailbody.Replace("{{AppNumber}}", application.ApplicationNumber + "");
                emailbody = emailbody.Replace("{{ResellerName}}", application.ResellerName);
                emailbody = emailbody.Replace("{{EndCustomerName}}", application.EndCustomerName);
                emailbody = emailbody.Replace("{{FinanceTotal}}", application.FinanceTotal + "");
                if (!string.IsNullOrEmpty(application.FinanceType))
                {
                    int financeTypeId;
                    int.TryParse(application.FinanceType, out financeTypeId);
                    var financeTypeDesc = _financeTypesRepository.Table.Where(x => x.Id == financeTypeId).FirstOrDefault()?.Description;
                    emailbody = emailbody.Replace("{{FinanceType}}", financeTypeDesc);
                }
                if (application.FunderPlan > 0)
                {
                    var funderPlanDesc = _funderPlanRepository.Table.Where(x => x.PlanId == application.FunderPlan).FirstOrDefault()?.PlanDescription;
                    emailbody = emailbody.Replace("{{FunderPlan}}", funderPlanDesc);
                }
                if (!string.IsNullOrEmpty(application.FinanceFunder))
                {
                    int financeFunderId;
                    int.TryParse(application.FinanceFunder, out financeFunderId);
                    var financeFunder = _fundersRepository.Table.Where(x => x.Id == financeFunderId).FirstOrDefault()?.FunderName;
                    emailbody = emailbody.Replace("{{FinanceFunder}}", financeFunder);
                }
                var url = Configuration.GetValue<string>("IMFSClientAppHost") + "/application/application-details?id=" + application.Id;
                emailbody = emailbody.Replace("{{URLWithText}}", "<a href='" + url + "&appNo=" + application.ApplicationNumber + "&mode=edit'>click here</a>");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return emailbody;
        }

        public GetDealStatusAPIResponse CallGDSAPI(Applications application)
        {
            var apiResponse = new GetDealStatusAPIResponse();
            apiResponse.Error = true;
            var response = new GetDealStatusResponse();
            response.Error = true;
            apiResponse.apiErrorLogs = new List<string>();

            var gdsAPIModel = new HPEAPIRequestResponseObjects();
            var acApiModel = new HPEAPIRequestResponseObjects();
            //Create Log object
            var apiLog = new IMFSAPILog();

            //Application status success count
            int updatedApplicationCout = 0;
            try
            {
                string appNumber = Convert.ToString(application.ApplicationNumber);
                string creditApplicationId = Convert.ToString(application.HPECreditApplicationID);
                try
                {
                    string webAddr = "https://api-csc-stg-sgw.ext.hpe.com/gw/gfit/hpfs/creditservice.rest.stg/3.0/GetDealStatus";
                    var httpWebRequest = System.Net.WebRequest.CreateHttp(webAddr);
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    httpWebRequest.Method = "POST";

                    //Certificate 
                    httpWebRequest.ClientCertificates.Add(GetCertificate());


                    var quoteData = _quotesRepository.GetById(application.QuoteId);
                    var dealStatusRequestModel = new GetDealStatusRequest();
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        dealStatusRequestModel = BindGDSRequestModel(quoteData?.ResellerAccount, quoteData?.ResellerName, creditApplicationId); //pass in the param applicationId/CustomerID/QuoteId
                        string requestBody = JsonConvert.SerializeObject(dealStatusRequestModel);
                        gdsAPIModel.Request = requestBody;
                        doLog("App No: " + appNumber, "GetDealStatusAPI - request data", requestBody);
                        //Request Log
                        apiLog.Url = "GetDealStatus Api Call";
                        apiLog.RequestBody = requestBody;
                        streamWriter.Write(requestBody);
                        streamWriter.Flush();

                    }
                    try
                    {
                        var httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            string responseData = streamReader.ReadToEnd();
                            //Response Log
                            apiLog.ResponseBody = responseData;
                            gdsAPIModel.Response = responseData;
                            doLog("App No: " + appNumber, "GetDealStatusAPI - response data", responseData);
                            apiResponse.GDSAPIModel = gdsAPIModel;
                            //responseData= "{'TransactionDetail':{'TransactionId':'Design','ClientProgramId':'IngramTest','PartnerSupplierId':0,'DistributorId':5423912845,'ApplicationID':302636,'ClientID':'IngramTest','CreditID':'741048','RelationshipCode':'HPE','ClientAuthenticationKey':'','LanguageCode':'en'},'TransactionResult':{'ResultSuccess':true,'Validation':[]},'GetDealStatusList':[{'ApplicationRef':'302636','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC PTY LTD','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'12\\/6\\/2022 1:42:47 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':6011399,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302594','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/30\\/2022 3:44:14 PM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5983084,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302591','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/30\\/2022 2:53:59 PM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5982766,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302568','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/30\\/2022 2:36:48 PM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5976893,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302575','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'PAY YOU BACK PTY LTD','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/30\\/2022 6:46:04 AM','NextAction':'','RequestedDealAmount':4200.00,'Currency':'AUD','WebServiceID':5979675,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302574','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'PAY YOU BACK PTY LTD','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/30\\/2022 6:45:16 AM','NextAction':'','RequestedDealAmount':4200.00,'Currency':'AUD','WebServiceID':5979669,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302573','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'LA LA CORP PTY LTD','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/30\\/2022 6:35:40 AM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5979662,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302569','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/29\\/2022 2:00:55 PM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5977077,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302567','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'E-SIGN DOCS IN PROGRESS','StatusUpdateTimestamp':'11\\/29\\/2022 1:03:41 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5976821,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302566','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/29\\/2022 12:52:11 PM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5976737,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302565','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/29\\/2022 12:43:37 PM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5976730,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302564','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'E-SIGN DOCS IN PROGRESS','StatusUpdateTimestamp':'11\\/29\\/2022 12:36:42 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5976669,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302563','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/29\\/2022 12:32:23 PM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5976637,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302561','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/29\\/2022 5:08:30 AM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5974564,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302556','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/28\\/2022 12:25:59 PM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5970757,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302545','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/28\\/2022 6:17:00 AM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5969498,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302544','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/25\\/2022 3:45:03 PM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5957638,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302535','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/24\\/2022 1:47:19 PM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5951564,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302534','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/24\\/2022 1:08:09 PM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5951303,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302527','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/24\\/2022 4:08:04 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5948929,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302514','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/23\\/2022 3:05:10 PM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5946357,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302503','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/23\\/2022 9:03:51 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5944164,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302437','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC PTY LTD','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/16\\/2022 5:42:23 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5910673,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302368','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/15\\/2022 12:42:32 PM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5878111,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302369','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/15\\/2022 12:35:40 PM','NextAction':'Generate Firm Quote','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5878129,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302379','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'PACIFIC & SONS PTY LTD','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/14\\/2022 4:27:12 AM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5895214,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302364','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/10\\/2022 12:33:58 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5877831,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302363','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'CLOUD PLUS PTY LTD','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/10\\/2022 12:22:21 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5877778,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302360','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/10\\/2022 10:50:56 AM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5877435,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302356','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/10\\/2022 3:12:01 AM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5876123,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302349','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/9\\/2022 2:54:08 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5874319,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302348','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/9\\/2022 2:43:51 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5874267,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302347','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/9\\/2022 2:42:00 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5874261,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302345','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/9\\/2022 2:29:28 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5874187,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302344','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/9\\/2022 2:27:16 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5874181,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302092','PartnerName':'Trident Computer Services Pty Ltd','PartnerId':'2367970438','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/9\\/2022 9:20:27 AM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5665243,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302280','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'11\\/9\\/2022 8:03:47 AM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5850466,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302331','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/9\\/2022 7:32:24 AM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5872481,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302330','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/9\\/2022 7:27:58 AM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5872476,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302329','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/9\\/2022 7:26:02 AM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5872473,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302328','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/8\\/2022 3:01:38 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5870823,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302327','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/8\\/2022 3:00:49 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5870818,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302326','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/8\\/2022 2:57:14 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5870790,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302325','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/8\\/2022 2:54:11 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5870762,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302324','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/8\\/2022 2:50:04 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5870734,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302323','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/8\\/2022 2:45:49 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5870724,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302321','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/8\\/2022 12:28:51 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5870087,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302316','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Firm Quote in Progress','StatusUpdateTimestamp':'11\\/8\\/2022 6:40:14 AM','NextAction':'Quote List','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5869157,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302315','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/8\\/2022 6:39:30 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5869154,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302313','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/7\\/2022 2:18:51 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5867280,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302312','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/7\\/2022 2:12:02 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5867252,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302311','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/7\\/2022 2:10:03 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5867224,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302310','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/7\\/2022 2:03:44 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5867196,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302309','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/7\\/2022 1:57:21 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5867168,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302308','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/7\\/2022 1:49:31 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5867117,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302307','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/7\\/2022 1:44:17 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5867089,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302306','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/7\\/2022 1:43:54 PM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5867086,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302305','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/7\\/2022 11:18:44 AM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5866452,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302304','PartnerName':'Winc Australia Pty Limited','PartnerId':'4951675468','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/7\\/2022 8:15:57 AM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5865949,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302303','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'Remco','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'11\\/7\\/2022 8:09:16 AM','NextAction':'','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5865898,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302302','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 8:04:05 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5865850,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302301','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 8:03:46 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5865847,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302300','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 7:55:04 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5865805,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302299','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 7:49:16 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5865802,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302298','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 7:47:52 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5865797,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302297','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 7:43:34 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5865794,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302295','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 7:43:34 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5865790,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302296','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 7:43:34 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5865791,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302294','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Firm Quote in Progress','StatusUpdateTimestamp':'11\\/7\\/2022 6:51:40 AM','NextAction':'Quote List','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5865768,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302293','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 3:00:16 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1050.00,'Currency':'AUD','WebServiceID':5865736,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302292','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 2:38:46 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5865727,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302291','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 2:35:45 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5865717,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302290','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 2:31:05 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5865713,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302289','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 2:30:29 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5865706,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302288','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 2:29:13 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5865699,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302287','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/7\\/2022 2:28:01 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5865694,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302285','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/4\\/2022 5:38:33 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5853204,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302284','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/4\\/2022 5:26:25 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5853193,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302283','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/4\\/2022 5:24:09 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5853190,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302282','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/4\\/2022 5:22:37 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5853183,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302281','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/4\\/2022 5:08:16 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5853174,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302279','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/3\\/2022 6:24:35 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5849536,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302278','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/2\\/2022 2:53:33 PM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5847829,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302277','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'11\\/2\\/2022 7:20:52 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5846396,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302276','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'10\\/28\\/2022 10:50:50 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5835502,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302273','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'10\\/27\\/2022 8:59:42 AM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5830630,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302274','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Credit Request Approved','StatusUpdateTimestamp':'10\\/27\\/2022 8:52:33 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5830638,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302272','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'10\\/25\\/2022 11:58:56 AM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5822843,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302271','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Firm Quote in Progress','StatusUpdateTimestamp':'10\\/25\\/2022 11:32:57 AM','NextAction':'Quote List','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5822730,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302270','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Awaiting Lease Doc Prep from HPFS','StatusUpdateTimestamp':'10\\/25\\/2022 11:20:27 AM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5822672,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302269','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'10\\/25\\/2022 10:46:44 AM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5822611,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302267','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'10\\/25\\/2022 9:20:01 AM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5821999,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302268','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Credit Request Approved','StatusUpdateTimestamp':'10\\/25\\/2022 8:46:59 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5822146,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302265','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/21\\/2022 6:23:24 AM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5795989,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302263','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/20\\/2022 12:25:32 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5791481,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302260','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/19\\/2022 1:40:27 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5785529,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302259','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/19\\/2022 1:38:57 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5785502,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302253','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/19\\/2022 10:38:47 AM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5784280,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302249','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/19\\/2022 9:28:10 AM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5784059,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302247','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/18\\/2022 3:25:35 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5780424,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302245','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/18\\/2022 1:47:53 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5780027,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302244','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/18\\/2022 1:26:51 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5779879,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302242','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Credit Request Approved','StatusUpdateTimestamp':'10\\/18\\/2022 12:00:24 PM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5778727,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302240','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'10\\/18\\/2022 9:55:27 AM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5778330,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302241','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Credit Request Approved','StatusUpdateTimestamp':'10\\/18\\/2022 9:45:19 AM','NextAction':'Generate Firm Quote','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5778383,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302239','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/18\\/2022 9:31:29 AM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5778295,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302238','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/18\\/2022 7:40:40 AM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5777836,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302237','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/18\\/2022 7:39:07 AM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5777820,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302236','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/18\\/2022 7:28:30 AM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5777799,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302234','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/18\\/2022 4:48:45 AM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5777533,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302233','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/18\\/2022 4:42:39 AM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5777518,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302181','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'10\\/17\\/2022 2:45:39 PM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5750962,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302232','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/17\\/2022 2:08:12 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5774200,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302231','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/17\\/2022 2:07:48 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5774197,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302230','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/17\\/2022 1:59:31 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5774132,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302229','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/17\\/2022 1:58:49 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5774129,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302228','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/17\\/2022 1:56:44 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5774126,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302227','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/17\\/2022 1:55:49 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5774123,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302226','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/17\\/2022 1:52:46 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5774089,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302225','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/17\\/2022 1:52:15 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5774086,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302224','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/17\\/2022 1:52:01 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5774083,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302223','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/17\\/2022 1:47:26 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5774049,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302222','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/17\\/2022 1:34:32 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5773953,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302180','PartnerName':'Blue Connections  Pty Ltd atf Blue Connections Unit Trust','PartnerId':'2628558614','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'GOUGH RECRUITMENT (NSW) PTY LTD','Status':'Lease Docs Available for Execution','StatusUpdateTimestamp':'10\\/17\\/2022 12:46:48 PM','NextAction':'Print Lease Documents','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5750880,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302220','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/17\\/2022 12:46:12 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5773676,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302219','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail':'AU-IMFSPortal@ingrammicro.com','CustomerName':'ABC Pty Ltd','Status':'Under Manual Credit Review','StatusUpdateTimestamp':'10\\/17\\/2022 12:35:00 PM','NextAction':'','RequestedDealAmount':1102.50,'Currency':'AUD','WebServiceID':5773582,'SpecialFirmQuote':null,'HPEFSPONumber':null},{'ApplicationRef':'302218','PartnerName':'ARTREF PTY LTD','PartnerId':'5142739112','DistiOrCallCentreName':'INGRAM MICRO PTY LTD','DistiOrCallCentreID':'5423912845','CreatedBy':'IMFS \\/ Portal','CreatedByEmail': 'AU-IMFSPortal@ingrammicro.com','CustomerName': 'GOUGH RECRUITMENT (NSW) PTY LTD', 'Status': 'Credit Request Approved','StatusUpdateTimestamp': '9\\/26\\/2022 9:26:03 AM','NextAction': 'Generate Firm Quote','RequestedDealAmount': 1102.5,'Currency': 'AUD','WebServiceID': 5665243,'SpecialFirmQuote':null,'HPEFSPONumber':null}]}";
                            response = JsonConvert.DeserializeObject<GetDealStatusResponse>(responseData);
                            if (response != null)
                            {
                                var resultStatus = response.TransactionResult.ResultSuccess;
                                if (resultStatus)
                                {
                                    response.Error = false;
                                    //API response
                                    apiResponse.Error = false;

                                    if (response.GetDealStatusList != null)
                                    {
                                        //Get status from the response
                                        var strStatus = response.GetDealStatusList.Where(w => w.ApplicationRef == creditApplicationId).FirstOrDefault()?.Status;

                                        //check if status exist in the db status table
                                        if (!string.IsNullOrEmpty(strStatus))
                                        {
                                            var applicationStatus = _statusRepository.Table.Where(x => x.HPDescription == strStatus).FirstOrDefault();

                                            if (applicationStatus != null)
                                            {
                                                var statusId = applicationStatus.Id;
                                                if (statusId > 0)
                                                {
                                                    apiResponse.apiErrorLogs.Add("API Status Description is - " + applicationStatus.Description);
                                                    //application table updated with api response status
                                                    application.ApplicationStatus = statusId;
                                                    response.Error = false;
                                                    response.Message = "Application status updated";

                                                    //API response
                                                    updatedApplicationCout++;
                                                    doLog("App No: " + appNumber, "App Status", application.ApplicationStatus.ToString());

                                                    if (applicationStatus.HPCode == "SCR" || applicationStatus.HPCode == "CRS")
                                                    {
                                                        CallGDSAPI(application);
                                                        apiResponse.apiErrorLogs.Add("Called CallGDSAPI as HPECode is - " + applicationStatus.HPCode);
                                                    }

                                                    //Sent mail when application status is declined 
                                                    var emailbody = Configuration.GetValue<string>("ApplicationGCRSResponseEmail");
                                                    string fromMail, toMail, subject;
                                                    fromMail = Configuration.GetValue<string>("NoReplyEmail");
                                                    toMail = Configuration.GetValue<string>("CreditApprovalEmailAddress");
                                                    if (applicationStatus.HPCode == "CRD")
                                                    {
                                                        subject = "Application (" + application.ApplicationNumber + ") has been declined";
                                                        emailbody = GetGCRSMailBody(application, emailbody);
                                                        _emailService.Send(Configuration.GetValue<string>("NoReplyEmail"),
                                                                           Configuration.GetValue<string>("CreditApprovalEmailAddress"),
                                                                           string.Empty, string.Empty, subject,
                                                                           emailbody, null, null, null);
                                                        doLog("App No: " + appNumber, "GCRS - response - CRD mail sent", "");
                                                        SaveSendEmail(application.ApplicationNumber, fromMail, toMail, null, null, subject, emailbody);
                                                        application = SetApplicationStatus("Application Declined", application);
                                                        apiResponse.Message = "App Status declined updated successfully";
                                                    }

                                                    //check if application HPcode is "CRA" then call accept quoate API
                                                    if (applicationStatus.HPCode == "CRA" || applicationStatus.HPCode == "FQIP" || applicationStatus.HPCode == "CRAC")
                                                    {
                                                        //Sent mail when application status is conditionally approved 
                                                        if (applicationStatus.HPCode == "CRA")
                                                        {
                                                            emailbody = GetGCRSMailBody(application, emailbody);
                                                            subject = "Application (" + application.ApplicationNumber + ") has been  approved";
                                                            _emailService.Send(Configuration.GetValue<string>("NoReplyEmail"),
                                                                               Configuration.GetValue<string>("CreditApprovalEmailAddress"),
                                                                               string.Empty, string.Empty, subject,
                                                                               emailbody, null, null, null);
                                                            doLog("App No: " + appNumber, "GCRS - response - CRA mail sent", "");
                                                            SaveSendEmail(application.ApplicationNumber, fromMail, toMail, null, null, subject, emailbody);
                                                            application = SetApplicationStatus("Application Approved", application);
                                                            apiResponse.Message = "App Status approved updated successfully";
                                                        }

                                                        //Sent mail when application status is conditionally approved 
                                                        if (applicationStatus.HPCode == "CRAC")
                                                        {
                                                            emailbody = GetGCRSMailBody(application, emailbody);
                                                            subject = "Application (" + application.ApplicationNumber + ") has been conditionally approved";
                                                            _emailService.Send(Configuration.GetValue<string>("NoReplyEmail"),
                                                                               Configuration.GetValue<string>("CreditApprovalEmailAddress"),
                                                                               string.Empty, string.Empty, subject,
                                                                               emailbody, null, null, null);
                                                            doLog("App No: " + appNumber, "GCRS - response - CRA mail sent", "");
                                                            SaveSendEmail(application.ApplicationNumber, fromMail, toMail, null, null, subject, emailbody);
                                                            application = SetApplicationStatus("Application Conditionally Approved", application);
                                                            apiResponse.Message = "App Status conditionally approved updated successfully";
                                                        }

                                                        //New method to bind combo request
                                                        GetComboRequest cr = BindGCRSRequestModel(response, application, dealStatusRequestModel);

                                                        ////Update HPE Data
                                                        GetComboResponse gcrs = CallGCRSAPI(cr);
                                                        apiResponse.GCRSAPIModel = gcrs.GCRSApiModel;
                                                        if (gcrs != null)
                                                        {
                                                            if (!gcrs.Error)
                                                            {
                                                                if (gcrs.GetFirmQuoteResponse != null)
                                                                {
                                                                    var trnResult = gcrs.GetFirmQuoteResponse?.TransactionResult?.ResultSuccess ?? false;
                                                                    if (trnResult)
                                                                    {

                                                                        if (gcrs.GetFirmQuoteResponse.GetDetailedPricingResponseDetail != null)
                                                                        {
                                                                            if (gcrs.GetFirmQuoteResponse.GetDetailedPricingResponseDetail.LeasePaymentOptions != null)
                                                                            {
                                                                                int intCreditApplicationId;
                                                                                int.TryParse(creditApplicationId, out intCreditApplicationId);
                                                                                var firmLeaseOptions = gcrs.GetFirmQuoteResponse.GetDetailedPricingResponseDetail.LeasePaymentOptions;
                                                                                if (firmLeaseOptions != null)
                                                                                {
                                                                                    foreach (var leasePaymentOption in firmLeaseOptions)
                                                                                    {
                                                                                        if (leasePaymentOption != null && (leasePaymentOption.CreditApplicationNumber == intCreditApplicationId))
                                                                                        {
                                                                                            var fqlpotype = leasePaymentOption;
                                                                                            if (fqlpotype != null)
                                                                                            {
                                                                                                if (gcrs.ComboInformationResponse != null &&
                                                                                                    gcrs.ComboInformationResponse.CreditDecisionCode == "APPVD" &&
                                                                                                    fqlpotype.QuoteNumber > 0)
                                                                                                {
                                                                                                    //var approveStatus = _statusRepository.Table.Where(x => x.Description == "Application Approved")?.FirstOrDefault();
                                                                                                    //if (approveStatus != null && approveStatus.Id > 0)
                                                                                                    //{
                                                                                                    //application.ApplicationStatus = approveStatus.Id;
                                                                                                    application.FunderQuote = fqlpotype.QuoteNumber + "";
                                                                                                    decimal dRevisedAprAmout, dPaymentWithoutTax, dPaymentAmount;
                                                                                                    decimal.TryParse(gcrs.RevisedApprovedAmount + "", out dRevisedAprAmout);
                                                                                                    application.RevisedApprovedAmount = dRevisedAprAmout;

                                                                                                    application.DownPaymentAmount = fqlpotype.DownPaymentAmount;
                                                                                                    decimal.TryParse(fqlpotype.PaymentWithoutTax + "", out dPaymentWithoutTax);
                                                                                                    application.PaymentWithoutTax = dPaymentWithoutTax;
                                                                                                    application.EstimatedTaxAmount = fqlpotype.EstimatedTaxAmount;
                                                                                                    decimal.TryParse(fqlpotype.PaymentAmount + "", out dPaymentAmount);
                                                                                                    application.PaymentAmount = dPaymentAmount;

                                                                                                    var documentSignatureRequiredActions = gcrs?.GetFirmQuoteResponse?.DocumentSignatureRequiredActions;
                                                                                                    if (documentSignatureRequiredActions != null)
                                                                                                    {
                                                                                                        application.AntiMoneyLaunderingSignorEmail = documentSignatureRequiredActions.AntiMoneyLaunderingSignorEmail;
                                                                                                        application.LeaseAgreementSignorEmail = documentSignatureRequiredActions.LeaseAgreementSignorEmail;
                                                                                                        application.CertificateOfAcceptanceSignorEmail = documentSignatureRequiredActions.CertificateOfAcceptanceSignorEmail;
                                                                                                        application.DirectDebitACHSignorEmail = documentSignatureRequiredActions.DirectDebitACHSignorEmail;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        apiResponse.Message = "Response DocumentSignatureRequiredActions object is null";
                                                                                                        apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                                                        doLog("App No: " + appNumber, "GCRS - response - null object", "Response DocumentSignatureRequiredActions object is null");
                                                                                                    }

                                                                                                    var gcrsConditionDetails = gcrs.ConditionDetails;
                                                                                                    if (gcrsConditionDetails != null)
                                                                                                    {
                                                                                                        application.UpFrontPayment = gcrsConditionDetails.UpFrontPayment;
                                                                                                        application.BankGuaranteeLetterOfCreditPcr = gcrsConditionDetails.BankGuaranteeLetterOfCreditPcr;
                                                                                                        application.TermUpperLimit = gcrsConditionDetails.TermUpperLimit;
                                                                                                        application.ArrearsMonthlyNotPermitted = gcrsConditionDetails.ArrearsMonthlyNotPermitted;
                                                                                                        application.ArrearsQuarterlyNotPermitted = gcrsConditionDetails.ArrearsQuarterlyNotPermitted;
                                                                                                        application.LeaseStructureNotPermitted = gcrsConditionDetails.LeaseStructureNotPermitted;

                                                                                                        application.Dys6090DfelInd = gcrsConditionDetails.Dys6090DfelInd;
                                                                                                        application.RevisedCreditAmount = gcrsConditionDetails.RevisedCreditAmount;
                                                                                                        application.Guarantors = gcrsConditionDetails.Guarantors;
                                                                                                        application.OtherCondition = gcrsConditionDetails.OtherCondition;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        apiResponse.Message = "Response ConditionDetails object is null";
                                                                                                        apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                                                        doLog("App No: " + appNumber, "GCRS - response - null object", "Response ConditionDetails object is null");
                                                                                                    }

                                                                                                    //update application details
                                                                                                    _applicationsRepository.Update(application);
                                                                                                    //apiResponse.Message = "App Status " + approveStatus.Description + " updated successfully";
                                                                                                    //}
                                                                                                    //else
                                                                                                    //{
                                                                                                    //    apiResponse.Message = "Status - Application Approved - not found in database ";
                                                                                                    //    apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                                                    //    doLog("App No: " + appNumber, "GCRS - response - Success", "Status - Application Approved - not found in database ");
                                                                                                    //}
                                                                                                    if (quoteData != null && quoteData.Id > 0)
                                                                                                    {
                                                                                                        var quote = _quotesRepository.GetById(quoteData.Id);
                                                                                                        quote.FunderQuote = fqlpotype.QuoteNumber + "";
                                                                                                        //fqlpotype.QuoteEffectiveDate = string.Format("{00/00/0000}", fqlpotype.QuoteEffectiveDate);
                                                                                                        //DateTime qEffectiveDate, qExpirationDate;
                                                                                                        //DateTime.TryParse(fqlpotype.QuoteEffectiveDate, out qEffectiveDate);
                                                                                                        //qExpirationDate = DateTime.ParseExact(fqlpotype.QuoteExpirationDate, "dd/MM/yyyy", null);

                                                                                                        quote.FunderQuoteEffectiveDate = fqlpotype.QuoteEffectiveDate;
                                                                                                        quote.FunderQuoteExpirationDate = fqlpotype.QuoteExpirationDate;

                                                                                                        _quotesRepository.Update(quote);
                                                                                                        apiResponse.Message = "Quote details saved for quote id - " + quote.Id;
                                                                                                        apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                                                        doLog("App No: " + appNumber, "GCRS - response - Success", "Quote details saved for quote id - " + quoteData.Id);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        apiResponse.Message = "Quote details not found for applications quote id - " + application.QuoteId;
                                                                                                        apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                                                        doLog("App No: " + appNumber, "GCRS - response - Success", "Quote details not found for applications quote id - " + application.QuoteId);
                                                                                                    }


                                                                                                    #region Accept quote (AC) request code

                                                                                                    //Call accept quote API
                                                                                                    long partnerSuppierId = 0;
                                                                                                    var trnDetails = gcrs?.GetFirmQuoteResponse?.TransactionDetail;
                                                                                                    string partnerSuppierName = string.Empty;

                                                                                                    partnerSuppierId = trnDetails?.PartnerSupplierId ?? 0;
                                                                                                    partnerSuppierName = trnDetails?.PartnerSupplierName;
                                                                                                    var model = BindACRequestModel(fqlpotype.QuoteNumber, fqlpotype.CreditApplicationNumber, partnerSuppierId, partnerSuppierName);
                                                                                                    model.QuoteId = application.QuoteId;
                                                                                                    model.ApplicationId = application.ApplicationNumber;
                                                                                                    AcceptQuoteResponse acr = CallACAPI(model);
                                                                                                    apiResponse.ACAPIModel = acr.ACAPIModel;
                                                                                                    if (acr != null && !acr.Error)
                                                                                                    {
                                                                                                        apiResponse.Message = "AC - response - Success QuoteNumber - " + fqlpotype.QuoteNumber;
                                                                                                        apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                                                        doLog("App No: " + appNumber, "AC - response - Success", "QuoteNumber - " + fqlpotype.QuoteNumber);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        apiResponse.Message = "AC - response - fail, null response model" + acr?.Message;
                                                                                                        apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                                                        doLog("App No: " + appNumber, "AC - response - fail, null response model", acr?.Message);
                                                                                                    }
                                                                                                    #endregion

                                                                                                }
                                                                                                apiResponse.Message = "GCRS - response - Success QuoteNumber - " + fqlpotype.QuoteNumber;
                                                                                                apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                                                doLog("App No: " + appNumber, "GCRS - response - Success", "QuoteNumber - " + fqlpotype.QuoteNumber);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                apiResponse.Message = "Response FirmQuoteLeasePaymentOptionType object is null";
                                                                                                apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                                                doLog("App No: " + appNumber, "GCRS - response - null object", "Response FirmQuoteLeasePaymentOptionType object is null");
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            var message = string.Empty;
                                                                                            if (leasePaymentOption == null)
                                                                                            {
                                                                                                message = $"{message + " FirmQuoteLeasePaymentOptionType is null "}";
                                                                                            }
                                                                                            if (leasePaymentOption != null && leasePaymentOption.CreditApplicationNumber != application.HPECreditApplicationID)
                                                                                            {
                                                                                                message = $"{message + " FirmQuote Lease Payment Option Type have other application number " + leasePaymentOption.CreditApplicationNumber}";
                                                                                            }
                                                                                            apiResponse.apiErrorLogs.Add(message);
                                                                                            doLog("App No: " + appNumber, "GCRS - response - null object", "Response FirmQuoteLeasePaymentOptionType object is null " + message);
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    apiResponse.Message = "GCRS - response - null object Response LeasePaymentOptions object is null ";
                                                                                    apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                                    doLog("App No: " + appNumber, "GCRS - response - null object", "Response LeasePaymentOptions object is null ");
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                var message = "Response LeasePaymentOptions object is null,";
                                                                                if (gcrs.GetFirmQuoteResponse.TransactionResult == null)
                                                                                {
                                                                                    message = "Response TransactionResult object is null.";
                                                                                }
                                                                                if (gcrs.GetFirmQuoteResponse.TransactionResult != null && gcrs.GetFirmQuoteResponse.TransactionResult.ValidationEquipmentMapping != null && gcrs.GetFirmQuoteResponse.TransactionResult.ValidationEquipmentMapping.Count > 0
                                                                                    && gcrs.GetFirmQuoteResponse?.TransactionResult?.ValidationEquipmentMapping != null)
                                                                                {
                                                                                    var errorsModel = gcrs.GetFirmQuoteResponse?.TransactionResult?.ValidationEquipmentMapping ?? new List<ValidationEquipmentMapping>();

                                                                                    foreach (var error in errorsModel)
                                                                                    {
                                                                                        message = $"{message + " " + error?.ValidationEquipmentMessageType?.ValidationText}";
                                                                                    }
                                                                                    apiResponse.Message = "GCRS - response - HPE API error : " + message;

                                                                                }
                                                                                apiResponse.Message = "Response LeasePaymentOptions object is null " + message;
                                                                                apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                                doLog("App No: " + appNumber, "GCRS - response - null object", "Response LeasePaymentOptions object is null " + message);
                                                                            }

                                                                        }
                                                                        else
                                                                        {
                                                                            apiResponse.Message = "Response GetDetailedPricingResponseDetail object is null";
                                                                            apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                            doLog("App No: " + appNumber, "GCRS - response - Null object", "Response GetDetailedPricingResponseDetail object is null");
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        string message = "TransactionResult- " + trnResult;
                                                                        if (gcrs.GetFirmQuoteResponse.TransactionResult != null && gcrs.GetFirmQuoteResponse.TransactionResult.Validation.Count > 0)
                                                                        {
                                                                            foreach (Validation error in gcrs.GetFirmQuoteResponse.TransactionResult.Validation)
                                                                            {
                                                                                message = $"{message + " | " + error?.ValidationText}";
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            message = $"{message + "  null object - gcrs.GetFirmQuoteResponse.TransactionResult"}";
                                                                        }
                                                                        apiResponse.Message = message;
                                                                        apiResponse.apiErrorLogs.Add(message);
                                                                        doLog("App No: " + appNumber, "GCRS - (GetFirmQuoteResponse.TransactionResult.ResultSuccess) trn result - false", message);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    apiResponse.Message = "Response GetFirmQuoteResponse object is null";
                                                                    apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                    doLog("App No: " + appNumber, "GCRS - response - Null object", "Response GetFirmQuoteResponse object is null");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                apiResponse.Message = "GCRS - response - Error message - " + gcrs.Message;
                                                                apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                                doLog("App No: " + appNumber, "GCRS - response - Error", "Error message - " + gcrs.Message);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            apiResponse.Message = "GCRS Response object is null";
                                                            apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                            doLog("App No: " + appNumber, "GCRS - response - Null", "GCRS Response object is null");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        apiResponse.Message = "GetAcceptQuoteAPI skipped, HPCode - " + applicationStatus.HPCode;
                                                        apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                        doLog("App No: " + appNumber, "GetAcceptQuoteAPI skipped, HPCode - " + applicationStatus.HPCode, "");
                                                    }
                                                    _applicationsRepository.Update(application);
                                                    apiResponse.Message = "App Status " + applicationStatus.Description + " updated successfully";
                                                    apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                    doLog("App No: " + appNumber, apiResponse?.Message, "");
                                                }
                                                else
                                                {
                                                    apiResponse.Message = "App Status: " + statusId;
                                                    apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                    doLog("App No : " + appNumber, "App Status", Convert.ToString(statusId));
                                                }
                                            }
                                            else
                                            {
                                                apiResponse.Message = "App status not found in db - " + strStatus;
                                                apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                                doLog("App No : " + appNumber, "App status", "App status not found in db - " + strStatus);
                                            }
                                        }
                                        else
                                        {
                                            apiResponse.Message = "API response NULL App Status: " + strStatus;
                                            apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                            doLog("App No : " + appNumber, "API response NULL App Status: " + strStatus, "");
                                        }

                                    }
                                    else
                                    {
                                        apiResponse.Message = "Deal Status list not found in the api response ";
                                        apiResponse.apiErrorLogs.Add(apiResponse.Message);
                                        doLog("App No: : " + appNumber, "Deal Status list not found in the api response ", "");
                                    }
                                }
                                else
                                {
                                    apiResponse.Error = true;
                                    response.Error = true;
                                    apiResponse.apiErrorLogs.Add("validation error " + response?.TransactionResult?.Validation?.FirstOrDefault()?.ValidationText);
                                    doLog("App No : " + appNumber, "validation error ", response?.TransactionResult?.Validation?.FirstOrDefault()?.ValidationText);
                                }
                            }
                            else
                            {
                                apiResponse.Error = true;
                                response.Error = true;
                                response.Message = response?.TransactionResult?.Validation?.FirstOrDefault()?.ValidationText;
                                doLog("App No : " + appNumber, "validation error null API response ", response.Message);
                                apiResponse.apiErrorLogs.Add(response.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        apiResponse.Error = true;
                        response.Message = ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "");
                        doLog("Error", "App Id :" + appNumber, ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : ""));
                        apiResponse.apiErrorLogs.Add(response.Message + " " + ex.StackTrace);
                    }

                    _imfsLogManager.SaveAPIRequest(apiLog);
                }
                catch (Exception ex)
                {
                    apiResponse.Error = true;
                    response.Message = ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : ""); 
                    doLog("App No : " + appNumber, "Exception ", ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : ""));
                    apiResponse.apiErrorLogs.Add(response.Message);
                }
            }
            catch (Exception ex)
            {
                apiResponse.Error = true;
                response.Message= ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "");
                apiResponse.apiErrorLogs.Add(ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : ""));
            }
            apiResponse.UpdatedApplicationCount = updatedApplicationCout;
            return apiResponse;
        }

        public void SaveSendEmail(int ApplicationNumber, string from, string to, string cc, string bcc, string subject, string body, List<Attachment> attachments = null, AlternateView altView = null, string messageId = "")
        {
            int emailId = 0;
            var email = new Web.Models.DBModel.Emails();
            email.FromAddress = from;
            email.ToAddress = to;
            email.CCEmail = cc;
            email.CCEmail += "," + Configuration.GetValue<string>("ApplicationDefaultCcAddress"); //Replace by QuoteCreated By

            email.Subject = subject;
            email.Body = Regex.Replace(body, @"[^\u0000-\u007F]+", string.Empty).Trim();  // remove hidden characters

            var msgId = String.Format("<{0}@{1}>", Guid.NewGuid().ToString(), "ingrammicro.com");

            email.InternetMessageId = msgId;

            email.EmailType = IMFSEnums.EmailType.Sent.ToString();
            email.BodyType = IMFSEnums.EmailBodyType.HTML.ToString();
            email.Status = IMFSEnums.EmailStatus.Completed.ToString();
            email.Importance = IMFSEnums.EmailImportance.Normal.ToString();
            email.DateTimeReceived = DateTime.Now;
            email.Notes = DateTime.Now.ToString() + " - " + "LoggedInUser" + " - " + "Email sent by " + "LoggedInUser";
            email.Status = IMFSEnums.EmailStatus.Outbound.ToString();

            _emailManager.SaveRepliedEmails(email);
            emailId = email.Id;

            #region Attachments

            string physicalPath = Configuration.GetValue<string>("EmailAttachmentRootDirectory");
            var foldername = DateTime.Today.ToString("dd'.'MM'.'yyyy");
            Tools.CreateFolder(physicalPath, foldername);

            foreach (var attachment in attachments)
            {
                var attachmentobj = new EmailAttachment();

                attachmentobj.EmailId = emailId;
                attachmentobj.FileName = attachment.Name.Split("\\").Last();
                attachmentobj.PhysicalPath = physicalPath + "\\" + foldername;
                attachmentobj.ContentId = "pdf";
                _emailManager.SaveAttachment(attachmentobj);

                var sourceFile = Configuration.GetValue<string>("EmailAttachmentTempRootDirectory") + "\\" + "LoggedInUser" + "\\" + attachmentobj.FileName;
                var newFile = attachmentobj.PhysicalPath + "\\" + attachmentobj.Id + Path.GetExtension(attachmentobj.FileName);
                Tools.CopyFile(sourceFile, newFile);
                Tools.DeleteFile(sourceFile);

            }
            #endregion

            try
            {

                //Create an email reference with application
                _emailManager.CreateEmailXref(emailId, 0, Convert.ToInt32(ApplicationNumber));


                //Create imfs Log
                doLog(IMFSEnums.QuoteLogTypes.ApplicationFormSent.ToString(), $"Email: {emailId} Sent to End Customer for Application", "Sent");


            }
            catch (Exception ex)
            {
                doLog("in SendEmail()", "Error", ex.Message);
            }

        }

        public Applications SetApplicationStatus(string applicationStatus, Applications application)
        {
            try
            {
                var approveStatus = _statusRepository.Table.Where(x => x.Description == applicationStatus)?.FirstOrDefault();
                if (approveStatus != null && approveStatus.Id > 0)
                {
                    application.ApplicationStatus = approveStatus.Id;
                }
            }
            catch (Exception ex)
            {
                doLog("Error", "SetApplicationStatus", ex.Message + ex.InnerException != null ? " " + ex.InnerException.Message : "");
            }
            return application;
        }
        #endregion

        //#endregion

        #region Get Certificate path
        private X509Certificate2 GetCertificate()
        {
            try
            {
                string certificateName = Configuration.GetValue<string>("CertificateName");

                string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string sFile = sCurrentDirectory + "Resources\\" + certificateName;

                string hpDepCertifPath = Path.GetFullPath(sFile);

                return new X509Certificate2(hpDepCertifPath, "Ingram@123", X509KeyStorageFlags.MachineKeySet);
            }
            catch (Exception ex)
            {
                doLog("Certificate Exception", "exception", ex.Message);
                return null;
            }
        }
        #endregion

        private void doLog(string msg, string req, string res)
        {
            var apiLog = new IMFSAPILog();
            apiLog.Url = msg;
            apiLog.RequestBody = req;
            apiLog.ResponseBody = res;
            _imfsLogManager.SaveAPIRequest(apiLog);
        }
    }
}


