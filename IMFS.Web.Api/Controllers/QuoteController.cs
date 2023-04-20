using IMFS.BusinessLogic.Quote;
using IMFS.BusinessLogic.RoleManagement;
using IMFS.Web.Api.Helper;
using IMFS.Web.Models.Quote;
using IMFS.Web.Models.QuoteRateCalculation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using IMFS.Web.Models.Misc;
using System.IO;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class QuoteController : BaseController
    {
        private readonly IQuoteManager _quoteManager;
        private readonly IQuoteDownloadManager _quoteDownloadManager;
        private readonly IConfiguration _configuration;
        private readonly IRoleManager _roleManager;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public QuoteController(IQuoteManager quoteManager, IConfiguration configuration,
            IQuoteDownloadManager quoteDownloadManager, IRoleManager roleManager)
        {
            _quoteManager = quoteManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _quoteDownloadManager = quoteDownloadManager;
        }


        [Route("SaveQuote")]
        [HttpPost]
        public IActionResult SaveQuote(Web.Models.Quote.QuoteDetailsModel quoteDetailsModel)
        {
            try
            {
                _logger.Info(JsonConvert.SerializeObject(quoteDetailsModel));
                var claims = HttpContext.User.Claims;
                var userId = claims.FirstOrDefault(x => x.Type == "UserId")?.Value.ToLower();
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { status = "Failed", error = "User token not found" });
                }
                else
                {
                    quoteDetailsModel.QuoteHeader.QuoteCreatedBy = userId;
                    var response = _quoteManager.SaveQuote(quoteDetailsModel);
                    if (response.HasError)
                    {
                        return Ok(new { status = "Error", message = response.ErrorMessage });
                    }
                    else
                    {
                        return Ok(new { status = "Success", message = "Quote information updated successfully", quoteId = response.QuoteId });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }

        }

        [Route("GetQuoteDetails")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetQuoteDetails(string quoteId)
        {
            try
            {
                var response = _quoteManager.GetQuoteDetails(quoteId);
                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Quote information updated successfully", quoteDetails = response.QuoteDetails });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }

        }

        [Route("CalculateRate")]
        [HttpPost]
        public IActionResult CalculateRate(QuoteRateCalculationModel quoteModel)
        {
            try
            {
                var defaultFinanceType = _configuration.GetValue<string>("DefaultFinanceType");
                var defaultFrequency = _configuration.GetValue<string>("DefaultFrequency");
                var defaultDuration = _configuration.GetValue<string>("DefaultDuration");

                var response = _quoteManager.CalculateRate(quoteModel, defaultFrequency, defaultDuration, defaultFinanceType);
                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Quote Rate Calculate Successful", financeDetails = response.QuoteResponse });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }


        [Route("SearchQuote")]
        [HttpPost]
        public IActionResult SearchQuote(QuoteSearchModel quoteSearchModel)
        {
            try
            {
                QuoteSearchResponseModel response;
                var claims = HttpContext.User.Claims;
                var userId = claims.FirstOrDefault(x => x.Type == "UserId")?.Value.ToLower();
                var role = _roleManager.GetUserRole(userId);
                var resellerId = claims.FirstOrDefault(x => x.Type == "imResellerId")?.Value.ToLower();
                if (role != null && (role.Name == "ResellerStandard" || role.Name == "ResellerAdmin"))
                {
                    _logger.Info("inside user search: " + userId);
                    response = _quoteManager.SearchQuote(quoteSearchModel, resellerId);
                }
                else
                {
                    response = _quoteManager.SearchQuote(quoteSearchModel, string.Empty);
                }

                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Quote Search successfully", searchResult = response.SearchResult.OrderByDescending(s => s.CreatedDate) });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        [Route("LookupQuoteNumber")]
        [HttpPost]
        public IActionResult LookupQuoteNumber(QuoteSearchModel quoteSearchModel)
        {
            try
            {
                QuoteSearchResponseModel response;
                var claims = HttpContext.User.Claims;
                var userId = claims.FirstOrDefault(x => x.Type == "UserId")?.Value.ToLower();
                var role = _roleManager.GetUserRole(userId);
                var resellerId = claims.FirstOrDefault(x => x.Type == "imResellerId")?.Value.ToLower();
                if (role != null && (role.Name == "ResellerStandard" || role.Name == "ResellerAdmin"))
                {
                    response = _quoteManager.LookupQuoteNumber(quoteSearchModel, resellerId);
                }
                else
                {
                    response = _quoteManager.LookupQuoteNumber(quoteSearchModel, string.Empty);
                }

                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Quote Search successfully", searchResult = response.SearchResult.OrderByDescending(s => s.CreatedDate) });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        [Route("DownloadQuote")]
        [HttpPost]
        public IActionResult DownloadQuote(QuoteDownloadInput inputModel)
        {
            try
            {
                var result = _quoteDownloadManager.DownloadQuote(inputModel);
                if (result.HasError)
                {
                    return BadRequest(new { status = "Error", message = result.ErrorMessage });
                }
                else
                {
                    IMFSGlobals.CreateDownloadResponse(Response, result);
                    return File(result.DownloadFile, MimeTypes.GetMimeType(result.FileName), result.FileName);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        /// <summary>
        /// Get the recent quotes by for the reseller account
        /// </summary>
        /// <returns></returns>
        [Route("GetRecentQuotes")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetRecentQuotes()
        {
            try
            {
                //check user claim
                var claims = HttpContext.User.Claims;
                string resellerId = claims.FirstOrDefault(x => x.Type == "imResellerId")?.Value;
                if (string.IsNullOrEmpty(resellerId)) resellerId = "153200";

                var response = _quoteManager.GetRecentQuotes(resellerId);
                if (response == null)
                    return Ok(new { status = "Error", message = "Quotes not found" });
                else
                    return Ok(new { status = "Success", message = "Recent Quotes fetched successfully", recentQuoteDetails = response });

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }

        }

        /// <summary>
        /// Get the recent quotes by for the reseller account
        /// </summary>
        /// <returns></returns>
        [Route("GetAddresses")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<AddressList>> GetAddresses([FromBody] AddressRequest request)
        {
            try
            {
                var grantType = _configuration.GetValue<string>("grant_type");
                var clientID = _configuration.GetValue<string>("client_id");
                var clientSecret = _configuration.GetValue<string>("client_secret");

                string jsonString = System.Text.Json.JsonSerializer.Serialize(request);

                var response = await _quoteManager.GetAddresses(grantType, clientID, clientSecret, jsonString);

                if (response != null)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return StatusCode(StatusCodes.Status200OK,
                        response.Content);
                    else
                        return StatusCode(StatusCodes.Status412PreconditionFailed,
                        "Somthing went wrong.");
                }
                else
                    return StatusCode(StatusCodes.Status412PreconditionFailed,
                    "Response not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        ex.Message);
            }
        }

        //NOTE: This API is not associated with any service manager, due to this is calling another API from third party integration.  we have don't need any extra middleware.  
        /// <summary>
        /// date:03-02-2022
        /// this api for get abn details by abn and guid.
        /// </summary>
        /// <param name="abn"></param>
        /// <param name="guid"></param>
        /// <returns>response abn details json resopnse</returns>
        [Route("GetAbnDetails")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<AbnDetailsResponseSchema>> GetAbnDetails(string abn, string guid)
        {
            try
            {
                AbnDetailsResponseSchema? abnDetails = new AbnDetailsResponseSchema();
                //end point url
                var client = new RestClient("https://abr.business.gov.au/");
                var request = new RestRequest("json/AbnDetails.aspx?callback=callback&abn=" + abn + "&guid=" + guid, Method.Get);
                request.AddHeader("Content-Type", "application/json");
                //execute method with request.
                RestResponse<AbnDetailsResponseSchema> response = await client.ExecuteAsync<AbnDetailsResponseSchema>(request);
                if (response != null)
                {
                    //replace extra content from get response.
                    string json = response.Content != null ? response.Content.Replace("callback(", "").Replace(")", "") : "";
                    //deserialized string to response object.
                    abnDetails = json != "" ? JsonConvert.DeserializeObject<AbnDetailsResponseSchema>(json) : null;
                }
                //return response
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return StatusCode(StatusCodes.Status200OK,
                        abnDetails);
                else
                    return StatusCode(StatusCodes.Status412PreconditionFailed,
                   "Somthing went wrong.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error:" + ex.Message);
            }
        }

        //NOTE: This API is not associated with any service manager, due to this is calling another API from third party integration.  we have don't need any extra middleware.  
        /// <summary>
        /// date:03-02-2022
        /// this api for get abn listing by name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="guid"></param>
        /// <returns>get response apn listing</returns>
        [Route("GetAbnInfoListingByName")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<AbnMessageResponse>> GetAbnInfoListingByName(string name, string guid)
        {
            try
            {
                AbnMessageResponse? abnInfo = new AbnMessageResponse();
                //end point url
                var client = new RestClient("https://abr.business.gov.au/");
                var request = new RestRequest("json/MatchingNames.aspx?callback=callback&name=" + name + "&guid=" + guid, Method.Get);
                request.AddHeader("Content-Type", "application/json");
                //execute method with request.
                RestResponse<AbnMessageResponse> response = await client.ExecuteAsync<AbnMessageResponse>(request);
                if (response != null)
                {
                    //replace extra content from get response.
                    string json = response.Content != null ? response.Content.Replace("callback(", "").Replace(")", "") : "";
                    //deserialized string to response object.
                    abnInfo = json != "" ? JsonConvert.DeserializeObject<AbnMessageResponse>(json) : null;

                }

                //return response
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return StatusCode(StatusCodes.Status200OK,
                        abnInfo);
                else
                    return StatusCode(StatusCodes.Status412PreconditionFailed,
                   "Somthing went wrong.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Error:" + ex.Message);
            }
        }

        #region Quote Attachment APIs

        /// <summary>
        /// save quote attachment's by quote id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Route("upload-quote-attachments")]
        [Route("UploadQuoteAttachments")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UploadAttachments([FromForm] AttachmentRequest request)
        {
            try
            {

                if (request != null)
                {
                    //get all file form object.
                    List<IFormFile> files = request.files;

                    if (files != null && files.Count > 0)
                    {
                        Guid uploadBy = Guid.Empty;
                        var physicalPath = string.Empty;
                        if (request.Source == (int)Source.Application)
                        {
                            physicalPath = _configuration.GetValue<string>("ApplicationAttachmentPath");
                        }
                        else
                        {
                            physicalPath = _configuration.GetValue<string>("QuoteAttachmentPath");
                        }

                        //create quote folder
                        if (!System.IO.Directory.Exists(physicalPath + "\\" + request.Id))
                        {
                            System.IO.Directory.CreateDirectory(physicalPath + "\\" + request.Id);
                        }


                        var uploadResponse = await _quoteManager.UploadFiles(request.Id, request.ApplicationNumber, request.Source, uploadBy, physicalPath, request.Description, files);

                        if (string.IsNullOrEmpty(uploadResponse.Message))
                        {
                            uploadResponse.Message = "The files have been uploaded successfully.";
                            return StatusCode(StatusCodes.Status200OK, new { success = uploadResponse.Message });
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status400BadRequest, new { error = uploadResponse.Message });
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status412PreconditionFailed, new { error = "File not found." });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status412PreconditionFailed, new { error = "Request is null or empty." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        ex.Message);
            }
        }


        /// <summary>
        /// remove quote attachment's by file id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("DeleteQuoteAttachments")]
        [HttpPost]
        public async Task<IActionResult> DeleteAttachments(DeleteAttachmentRequest request)
        {
            try
            {
                if (request != null)
                {
                    var response = await _quoteManager.RemoveQuoteFiles(request.FileId);

                    if (response=="Success")
                    {
                        return StatusCode(StatusCodes.Status200OK, new { success = "The file has been removed successfully." });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new { error = "File not found. " + request.FileId.ToString() + ". " + response });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status412PreconditionFailed, new { error = "Request is null or empty." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        ex.Message);
            }
        }


        /// <summary>
        /// get all files by quoteId
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        [Route("GetQuoteAttachments")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<AbnDetailsResponseSchema>> GetAttachments(int id, int source)
        {
            try
            {
                var response = await _quoteManager.GetQuoteFiles(id, source);

                //return response
                return StatusCode(StatusCodes.Status200OK,
                       response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error:" + ex.Message);
            }
        }

        /// <summary>
        /// Download files
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [Route("DownloadQuoteAttachment")]
        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(int fileId)
        {
            try
            {
                if (fileId > 0)
                {
                    DownloadResponse result = new DownloadResponse();
                    var file = await _quoteManager.DownloadFile(fileId);

                    if (file != null)
                    {
                        string fileExtension = Path.GetExtension(file.FileName);
                        if (System.IO.File.Exists(file.PhysicalPath))
                        {
                            var dataBytes = System.IO.File.ReadAllBytes(file.PhysicalPath);
                            result.DownloadFile = dataBytes;
                            result.FileName = file.FileName;
                            IMFSGlobals.CreateDownloadResponse(Response, result);
                            return File(result.DownloadFile, MimeTypes.GetMimeType(result.FileName), result.FileName);
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status412PreconditionFailed, new { error = "File not found" });
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status412PreconditionFailed,
                        "Error:" + file.Message);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status412PreconditionFailed, new { error = "Invalid fileId" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error:" + ex.Message);
            }

        }

        #endregion

        #region Get product details by sku or vpn
        /// <summary>
        /// Get product details by sku or vpn from orp db.
        /// </summary>
        /// <param name="param">sku or vpn</param>
        /// <returns></returns>
        [Route("GetProductsBySkuOrVPN")]
        [HttpGet]
        public async Task<ActionResult<ProductDetailResponse>> GetProductsBySkuOrVPN(string param)
        {
            try
            {
                string connnectionString = _configuration.GetConnectionString("AUORPDataContext");
                var response = await _quoteManager.GetProductsBySKUAndVPN(param, connnectionString);
                if (response != null)
                {
                    return Ok(new { status = response.Status, message = response.Message, products = response.Products });
                }
                return Ok(new { status = "Error" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Error", error = ex.ToString() });
            }
        }

        #endregion
    }
}
