using IMFS.BusinessLogic.ApplicationManagement;
using IMFS.BusinessLogic.Emails;
using IMFS.BusinessLogic.Quote;
using IMFS.Core;
using IMFS.Services.Services;
using IMFS.Web.Api.Helper;
using IMFS.Web.Models.Application;
using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Email;
using IMFS.Web.Models.Enums;
using IMFS.Web.Models.Quote;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : BaseController
    {
        private IEmailManager _emailManager;
        private IQuoteManager _quoteManager;
        private IApplicationManager _applicationManager;
        private IQuoteDownloadManager _quoteDownloadManager;

        private IIMFSEmailService _imfsEmailService;
        private readonly IConfiguration _configuration;

        public EmailController(IEmailManager emailManager, IIMFSEmailService imfsEmailService, IConfiguration configuration, IQuoteManager quoteManager,
            IQuoteDownloadManager quoteDownloadManager, IApplicationManager applicationManager)
        {
            _emailManager = emailManager;
            _imfsEmailService = imfsEmailService;
            _configuration = configuration;
            _quoteManager = quoteManager;
            _quoteDownloadManager = quoteDownloadManager;
            _applicationManager = applicationManager;
        }

        [HttpPost]
        [Route("SendEmail")]
        public IActionResult SendEmail(SendEmailViewModel model)
        {
            try
            {

                int emailId = 0;
                int jobId = 0;

                int tempQuoteId;
                int.TryParse(model.QuoteId, out tempQuoteId);

                int tempApplicationId;
                int.TryParse(model.ApplicationId, out tempApplicationId);

                #region Attachments

                var attachmentFiles = _emailManager.GetEmailAttachmentsTemp(model.TempEmailId);
                List<Attachment> AttachmentList = new List<Attachment>();

                foreach (var attachment in attachmentFiles)
                {
                    var filePath = attachment.PhysicalPath + "\\" + attachment.Id + Path.GetExtension(attachment.FileName);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    Stream fileStream = new MemoryStream(fileBytes);
                    AttachmentList.Add(new Attachment(fileStream, attachment.FileName));

                    if (tempQuoteId > 0)
                    {
                        //Create Quote Log
                        _quoteManager.InsertQuoteLog(tempQuoteId, IMFSEnums.QuoteLogTypes.AttachFile.ToString(), string.Format("Document: {0} attached", attachment.FileName));
                    }
                }

                #endregion Attachments

                #region Quote Attachments 

                //get attachment 
                var documents = _quoteManager.GetQuoteAttachments(model.QuotesAttached);

                if (documents != null && documents.Count > 0)
                {
                    foreach (var document in documents)
                    {
                        byte[] fileBytes = System.IO.File.ReadAllBytes(document.PhysicalPath);
                        Stream fileStream = new MemoryStream(fileBytes);
                        AttachmentList.Add(new Attachment(fileStream, document.FileName));

                        if (tempQuoteId > 0)
                        {
                            //Create Quote Log
                            _quoteManager.InsertQuoteLog(tempQuoteId, IMFSEnums.QuoteLogTypes.AttachFile.ToString(), string.Format("Document: {0} attached", document.FileName));
                        }
                    }
                }

                #endregion Quote Attachments 

                #region Resource files

                string html = model.Body;
                string newHTML = "";
                Dictionary<string, string> linkedResourcesCollection = new Dictionary<string, string>();
                List<LinkedResource> linkedResources = _emailManager.ConverBase64StringsToResources(model.EmailId, html, out newHTML, out linkedResourcesCollection);
                if (linkedResources.Count > 0)
                {
                    model.Body = newHTML;
                }
                AlternateView altView = null;
                if (linkedResources != null && linkedResources.Count > 0)
                {
                    altView = AlternateView.CreateAlternateViewFromString(model.Body, null, MediaTypeNames.Text.Html);
                    foreach (LinkedResource lr in linkedResources)
                    {
                        altView.LinkedResources.Add(lr);
                    }
                }

                #endregion Resource files

                var messageId = String.Format("<{0}@{1}>", Guid.NewGuid().ToString(), "ingrammicro.com");

                var email = new IMFS.Web.Models.DBModel.Emails();
                email.FromAddress = model.FromAddress;
                email.ToAddress = model.ToAddress;
                email.CCEmail = model.CCEmail;
                email.BCCEmail = model.BCCEmail;
                email.Subject = model.Subject;
                email.EmailType = IMFSEnums.EmailType.Sent.ToString();
                email.Body = Regex.Replace(model.Body, @"[^\u0000-\u007F]+", string.Empty).Trim();  // remove hidden characters
                email.InternetMessageId = messageId;
                int result = 0;
                Int32.TryParse(model.ParentEmailId, out result);
                if (result > 0)
                {
                    email.ParentEmailId = result;
                }
                email.BodyType = IMFSEnums.EmailBodyType.HTML.ToString();
                email.Status = IMFSEnums.EmailStatus.Completed.ToString();
                email.Importance = IMFSEnums.EmailImportance.Normal.ToString();
                email.DateTimeReceived = DateTime.Now;
                //email.Notes = DateTime.Now.ToString() + " - " + currentUser.FirstName + " " + currentUser.LastName + " - " + "Email sent by " + currentUser.FirstName + " " + currentUser.LastName;

                email.Notes = DateTime.Now.ToString() + " - " + "LoggedInUser" + " - " + "Email sent by " + "LoggedInUser";


                //save email
                email.Status = IMFSEnums.EmailStatus.Outbound.ToString();
                saveSentEmail(email, messageId, attachmentFiles, model.JobId, linkedResourcesCollection, model.ParentEmailStatus);
                emailId = email.Id;

                if (tempQuoteId > 0)
                {
                    //Create an email reference with quote
                    _emailManager.CreateEmailXref(emailId, tempQuoteId);
                }

                if (tempApplicationId > 0)
                {
                    //Create an email reference with quote
                    _emailManager.CreateEmailXref(emailId, 0, tempApplicationId);
                }

                if (tempQuoteId > 0)
                {
                    //Create Quote Log
                    _quoteManager.InsertQuoteLog(tempQuoteId, IMFSEnums.QuoteLogTypes.EmailSent.ToString(), string.Format("Email: {0} Sent to End User", emailId));
                }

                //create a job and save
                string message = string.Empty;
                try
                {
                    List<string> domainList = new List<string>();
                    List<string> emailList = new List<string>();
                    if (!string.IsNullOrEmpty(email.ToAddress))
                    {
                        var to = email.ToAddress.Split(',');
                        emailList.AddRange(to);
                    }
                    if (!string.IsNullOrEmpty(email.CCEmail))
                    {
                        var cc = email.CCEmail.Split(',');
                        emailList.AddRange(cc);
                    }
                    if (!string.IsNullOrEmpty(email.BCCEmail))
                    {
                        var bcc = email.BCCEmail.Split(',');
                        emailList.AddRange(bcc);
                    }
                    foreach (var index in emailList)
                    {
                        string domain = index.Split('@')[1];
                        domainList.Add(domain);
                    }
                    domainList = domainList.Distinct().ToList();

                    _imfsEmailService.Send(model.FromAddress, model.ToAddress, model.CCEmail, model.BCCEmail, model.Subject, model.Body, AttachmentList, altView, messageId);

                    //Not required for this release as on 2 June'21
                    //_emailManager.SaveEmailQuoteReminder(model.QuotesAttached, emailId);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { status = "Failed", error = ex.ToString() });
                }


                return Ok(new { status = "Success", data = emailId, message = "Email sent successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }



        private void saveSentEmail(IMFS.Web.Models.DBModel.Emails email, string messageId, List<EmailAttachmentsTemp> attachmentFiles, string jobId,
            Dictionary<string, string> linkResources, string parentEmailStatus)
        {
            //var currentUserId = IdentityHelper.GetClaimValue(User, ClaimTypes.NameIdentifier);
            var newEmailId = 0;

            #region save sent email

            _emailManager.SaveRepliedEmails(email);

            #endregion save sent email

            #region Attachments

            string physicalPath = _configuration.GetValue<string>("EmailAttachmentRootDirectory");
            var foldername = DateTime.Today.ToString("dd'.'MM'.'yyyy");
            Tools.CreateFolder(physicalPath, foldername);

            foreach (var attachment in attachmentFiles)
            {
                var oldfilename = attachment.FileName;
                var emailId = email.Id;
                var physicalpath = physicalPath;
                var attachmentobj = new EmailAttachment();

                attachmentobj.EmailId = emailId;
                attachmentobj.FileName = oldfilename;
                attachmentobj.PhysicalPath = physicalpath + "\\" + foldername;
                attachmentobj.ContentId = attachment.ContentId;//???
                _emailManager.SaveAttachment(attachmentobj);

                var sourceFile = _configuration.GetValue<string>("EmailAttachmentTempRootDirectory") + "\\" + "LoggedInUser" + "\\" + attachment.Id + Path.GetExtension(attachment.FileName);
                var newFile = attachmentobj.PhysicalPath + "\\" + attachmentobj.Id + Path.GetExtension(attachmentobj.FileName);
                Tools.CopyFile(sourceFile, newFile);
                _emailManager.DeleteEmailAttachmentTemp(attachment.Id);
            }
            // save resources as attachments
            if (linkResources != null)
            {
                foreach (var linkResource in linkResources)
                {
                    try
                    {
                        byte[] imageBytes = Convert.FromBase64String(linkResource.Value);
                        Stream imageStream = new MemoryStream(imageBytes);
                        var emailId = email.Id;
                        var attachmentobj = new EmailAttachment();

                        attachmentobj.EmailId = emailId;
                        attachmentobj.FileName = linkResource.Key;
                        attachmentobj.PhysicalPath = physicalPath + "\\" + foldername;
                        attachmentobj.ContentId = linkResource.Key;//???
                        _emailManager.SaveAttachment(attachmentobj);
                        var newFile = attachmentobj.PhysicalPath + "\\" + attachmentobj.Id + Path.GetExtension(attachmentobj.FileName);
                        imageBytes = Convert.FromBase64String(linkResource.Value);
                        using (FileStream stream = System.IO.File.Create(newFile))
                        {
                            stream.Write(imageBytes, 0, imageBytes.Length);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            #endregion Attachments       


        }


        [HttpPost]
        [Route("GetWriteEmailModel")]
        public IActionResult GetWriteEmailModel(GetWriteEmailModel inputModel)
        {
            try
            {
                var model = new WriteEmailViewModel();
                var applicationNumber = 0;
                if (inputModel.ApplicationId.HasValue && inputModel.ApplicationId > 0)
                {
                    ApplicationDetailsResponseModel existingApplication = _applicationManager.GetApplicationDetails(Convert.ToString(inputModel.ApplicationId.Value));

                    if (existingApplication != null)
                    {
                        applicationNumber = existingApplication.ApplicationDetails.ApplicationNumber;
                    }
                }


                if (inputModel.EmailId.HasValue && inputModel.EmailId > 0)
                {
                    var email = _emailManager.GetEmailDetails(inputModel.EmailId.Value);

                    model.FromAddress = email.FromAddress;
                    model.ToAddress = email.ToAddress;
                    model.EmailMode = "Quote";
                    model.CCEmail = email.CCEmail;
                    model.Subject = email.Subject;

                    var emailBodyModel = _emailManager.GetEmailHTMLBody(inputModel.EmailId.Value);
                    model.Body = emailBodyModel.HtmlBody;

                }

                else
                {
                    string disclaimer = _configuration.GetValue<string>("EmailDisclaimer");
                    // temporary email id for attachments
                    model.TempEmailId = Guid.NewGuid();
                    var templateDetails = new EmailTemplate();
                    if (inputModel.EmailMode == "Quote")
                        templateDetails = _emailManager.GetEmailTemplateDetails(1);
                    if (inputModel.EmailMode == "Application")
                        templateDetails = _emailManager.GetEmailTemplateDetails(2);

                    model.FromAddress = templateDetails.FromAddress;
                    model.ToAddress = templateDetails.ToAddress;
                    model.CCEmail = templateDetails.CCAddress;

                    if (inputModel.EmailMode == "Quote")
                        templateDetails.Subject = templateDetails.Subject.Replace("<QuoteNumber>", "Q" + Convert.ToString(inputModel.QuoteId));
                    if (inputModel.EmailMode == "Application")
                        templateDetails.Subject = templateDetails.Subject.Replace("<ApplicationNumber>", "A" + Convert.ToString(applicationNumber));

                    model.Subject = templateDetails.Subject;
                    model.EmailMode = inputModel.EmailMode;


                    if (inputModel.QuoteId.HasValue && inputModel.QuoteId > 0)
                    {
                        #region Encrypt AES

                        var key = _configuration.GetValue<string>("CipherKey");
                        var encryptedBase64QuoteIda = EncryptDecryptHelper.EncryptString(Convert.ToString(inputModel.QuoteId.Value), key);

                        var encodedCode = System.Net.WebUtility.UrlEncode(encryptedBase64QuoteIda);

                        var encodedQuoteUrla = _configuration.GetValue<string>("QuoteAcceptance") + encodedCode;
                        var quoteLinka = string.Empty;
                        if (!inputModel.removeAcceptanceLink)
                            quoteLinka = string.Format("<p><a href='{0}' rel='noopener noreferrer' target='_blank'>Click Here to Accept Quote</a></p>", encodedQuoteUrla);

                        model.Body = string.Format("<br /><br /><br /><br />{0}<br />{1}<br />" + disclaimer, templateDetails.Body, quoteLinka);

                        #endregion

                    }
                    else
                    {
                        model.Body = string.Format("<br /><br /><br /><br />{0}<br />" + disclaimer, templateDetails.Body);
                    }

                    #region add application attachment

                    if (inputModel.ApplicationId.HasValue && inputModel.ApplicationId > 0)
                    {
                        string physicalPath = _configuration.GetValue<string>("EmailAttachmentTempRootDirectory") + "\\" + "LoggedInUser";
                        // create own folder for each user id if it does not exist
                        Tools.CreateFolder(_configuration.GetValue<string>("EmailAttachmentTempRootDirectory"), "LoggedInUser");
                        List<ApplicationDownloadResponse> applicationAttachments = new List<ApplicationDownloadResponse>();
                        var applicationDownloadInput = new ApplicationDownloadInput();
                        applicationDownloadInput.Id = inputModel.ApplicationId.Value;
                        applicationDownloadInput.ApplicationNumber = applicationNumber;
                        applicationDownloadInput.DownloadMode = "pdf";
                        var appDownloadFile = _applicationManager.DownloadApplication(applicationDownloadInput);
                        applicationAttachments.Add(appDownloadFile);

                        foreach (var applicationAttachment in applicationAttachments)
                        {
                            if (applicationAttachment.DownloadFile != null)
                            {
                                var newAttachmentTemp = new EmailAttachmentModel();
                                newAttachmentTemp.TempEmailId = model.TempEmailId;
                                newAttachmentTemp.FileName = applicationAttachment.FileName;
                                newAttachmentTemp.PhysicalPath = physicalPath;
                                newAttachmentTemp.UploadedBy = "LoggedInUser";
                                newAttachmentTemp.FileSize = applicationAttachment?.DownloadFile?.Length;
                                int? newAttachmentTempId = _emailManager.SaveEmailAttachmentTemp(newAttachmentTemp);
                                if (newAttachmentTempId.HasValue)
                                {
                                    newAttachmentTemp.Id = newAttachmentTempId.Value;
                                    string temporaryFileName = newAttachmentTempId.Value.ToString() + Path.GetExtension(newAttachmentTemp.FileName);
                                    using (var fs = new FileStream(Path.Combine(physicalPath, temporaryFileName), FileMode.Create, FileAccess.Write))
                                    {
                                        fs.Write(applicationAttachment.DownloadFile, 0, applicationAttachment.DownloadFile?.Length ?? 0);
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region add quote attachment

                    if (inputModel.QuoteId.HasValue && inputModel.QuoteId > 0)
                    {
                        string physicalPath = _configuration.GetValue<string>("EmailAttachmentTempRootDirectory") + "\\" + "LoggedInUser";
                        // create own folder for each user id if it does not exist
                        Tools.CreateFolder(_configuration.GetValue<string>("EmailAttachmentTempRootDirectory"), "LoggedInUser");
                        List<QuoteDownloadResponse> quoteAttachments = new List<QuoteDownloadResponse>();
                        var quoteDownloadInput = new QuoteDownloadInput();
                        quoteDownloadInput.QuoteId = inputModel.QuoteId.Value;
                        quoteDownloadInput.DownloadMode = "Excel";
                        var quoteDownloadFile = _quoteDownloadManager.DownloadQuote(quoteDownloadInput);
                        if (!quoteDownloadFile.HasError)
                        {
                            quoteAttachments.Add(quoteDownloadFile);
                        }
                        else
                        {
                            return BadRequest(new
                            {
                                status = "Failed",
                                error = quoteDownloadFile.ErrorMessage
                            }); ;
                        }
                        quoteDownloadInput.DownloadMode = "Proposal";
                        var proposalDownloadFile = _quoteDownloadManager.DownloadQuote(quoteDownloadInput);
                        if (!proposalDownloadFile.HasError)
                        {
                            quoteAttachments.Add(proposalDownloadFile);
                        }
                        else
                        {
                            return BadRequest(new
                            {
                                status = "Failed",
                                error = quoteDownloadFile.ErrorMessage
                            }); ;
                        }


                        foreach (var quoteAttachment in quoteAttachments)
                        {
                            if (quoteAttachment.DownloadFile != null && quoteAttachment.DownloadFile.Length > 0)
                            {
                                var newAttachmentTemp = new EmailAttachmentModel();
                                newAttachmentTemp.TempEmailId = model.TempEmailId;
                                newAttachmentTemp.FileName = quoteAttachment.FileName;
                                newAttachmentTemp.PhysicalPath = physicalPath;
                                newAttachmentTemp.UploadedBy = "LoggedInUser";
                                newAttachmentTemp.FileSize = quoteAttachment.DownloadFile.Length;
                                int? newAttachmentTempId = _emailManager.SaveEmailAttachmentTemp(newAttachmentTemp);
                                if (newAttachmentTempId.HasValue)
                                {
                                    newAttachmentTemp.Id = newAttachmentTempId.Value;
                                    string temporaryFileName = newAttachmentTempId.Value.ToString() + Path.GetExtension(newAttachmentTemp.FileName);
                                    using (var fs = new FileStream(Path.Combine(physicalPath, temporaryFileName), FileMode.Create, FileAccess.Write))
                                    {
                                        fs.Write(quoteAttachment.DownloadFile, 0, quoteAttachment.DownloadFile.Length);
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "Failed",
                    error = ex.ToString()
                });
            }
        }



        [HttpGet]
        [Route("GetEmailHistory")]
        public IActionResult GetEmailHistory(int quoteId)
        {
            try
            {
                List<EmailHistory> emailHistory = new List<EmailHistory>();
                emailHistory = _emailManager.GetQuoteEmailHistory(quoteId);
                return Ok(emailHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }


        [HttpGet]
        [Route("GetApplicationEmailHistory")]
        public IActionResult GetApplicationEmailHistory(int applicationId)
        {
            try
            {
                List<EmailHistory> emailHistory = new List<EmailHistory>();
                emailHistory = _emailManager.GetApplicationEmailHistory(applicationId);
                return Ok(emailHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }


    }
}
