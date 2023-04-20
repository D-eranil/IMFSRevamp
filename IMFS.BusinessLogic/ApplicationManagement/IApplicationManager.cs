using IMFS.Web.Models.Application;
using IMFS.Web.Models.Customer;
using IMFS.Web.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.BusinessLogic.ApplicationManagement
{
    public interface IApplicationManager
    {
        ApplicationDetailsResponseModel GetApplicationDetails(string applicationId);

        ApplicationSearchResponseModel SearchApplication(ApplicationSearchModel quoteSearchModel, string resellerId);

        ApplicationSearchResponseModel LookupApplicationNumber(ApplicationSearchModel quoteSearchModel, string resellerId);

        ApplicationSaveResponseModel SaveApplication(ApplicationDetailsModel applicationDetailsModel, string userId);

        List<Contacts> GetContacts(string resellerID);

        List<Contacts> GetGuarantorContacts(string resellerID);

        List<Contacts> GetAccountantContacts(string resellerID);

        List<Contacts> GetTrusteeContacts(string resellerID);

        List<Contacts> GetBeneficialOwnersContacts(string resellerID);

        ApplicationDownloadResponse DownloadApplication(ApplicationDownloadInput inputModel);

        ApplicationUpdateResponseModel RejectApplication(int applicationId);
        ApplicationUpdateResponseModel CancelApplication(int applicationId);

        List<RecentApplicationsModel> GetRecentApplications(string resellerId);

        List<AwaitingInvoiceModel> GetAwaitingInvoices(string resellerId);

        #region Signatories Tabs

        SignatoriesTabModel GetSignatories(int applicationNumber);

        ExistingContactsModel GetExistingContacts(string resellerId, int tabId);

        ContactsResponseModel SaveContacts(ContactsRequestModel request);

        ContactsResponseModel DeleteContacts(string ContactID);

        #endregion

        #region Combo APIs

        ApplicationResponse VerifyCreatedApplication(int quoteId);

        CreateComboResponse CallCCRAPI(int applicationId, int quoteId);

        //GetComboAPIResponseResult TempCreateComboRequest(int applicationId, string xml);

        //GetComboResponse GetComboRequestStatus(GetComboRequest request, CreateComboRequest requestData);
        //GetComboResponse GetComboRequestStatus(GetComboRequest request, GetCCRequest requestData);

        //ComboResponseForStatus GetComboRequestStatus(ComboRequestForStatus request);

        //string GetComboRequestStatusTest(ComboRequestForStatus request);

        //string GetAcceptQuoteComboRequestTest(AcceptQuoteRequest request);

        AcceptQuoteResponse CallACAPI(AcceptQuoteRequest model);

        //GetDealStatusResponse GetDealStatusAPI();
        GetDealStatusAPIResponse GetDealStatusAPI();

        #endregion
    }
}
