using IMFS.Web.Models.Customer;
using IMFS.Web.Models.Misc;
using IMFS.Web.Models.OPRDBModel;
using IMFS.Web.Models.Quote;
using Microsoft.AspNetCore.Http;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMFS.BusinessLogic.Quote
{
    public interface IQuoteManager
    {
        QuoteRateCalculationResponseModel CalculateRate(Web.Models.QuoteRateCalculation.QuoteRateCalculationModel quoteModel, string defaultFrequency, string defaultDuration, string defaultFinanceType);

        QuoteSaveResponseModel SaveQuote(QuoteDetailsModel quoteDetailsModel);

        CustomerResponseModel GetCustomer(string customerName);

        QuoteDetailsResponseModel GetQuoteDetails(string quoteId);
        bool RejectQuoteDetails(RejectQuoteDetail quoteDetailsModel);

        QuoteSearchResponseModel SearchQuote(QuoteSearchModel quoteSearchModel, string resellerId);

        QuoteSearchResponseModel LookupQuoteNumber(QuoteSearchModel quoteSearchModel, string resellerId);

        void InsertQuoteLog(int quoteId, string type, string description);

        void UpdateQuoteStatus(int quoteId, int status);

        List<RecentQuotesModel> GetRecentQuotes(string resellerId);

        Task<RestResponse<AddressList>> GetAddresses(string grantType, string clientID, string clientSecret, string jsonString);

        ContactResponseModel GetCustomerContacts(string customerNumber, string connectionString);

        Task<FileUploadResponse> UploadFiles(int id, int? applicationNumber, int source, Guid? uploadBy, string physicalPath, string description, List<IFormFile> files);

        Task<string> RemoveQuoteFiles(int fileId);

        Task<List<AttachmentsResponse>> GetQuoteFiles(int id, int source);

        Task<FileDownloadResponse> DownloadFile(int id);

        Task<ProductDetailResponse> GetProductsBySKUAndVPN(string param, string connectionString);
        List<EmailAttachmentsResponse> GetQuoteAttachments(List<int> documentIDs);
    }
}
