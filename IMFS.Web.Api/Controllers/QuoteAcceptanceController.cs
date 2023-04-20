using IMFS.BusinessLogic.Emails;
using IMFS.BusinessLogic.Quote;
using IMFS.Services.Services;
using IMFS.Web.Api.Helper;
using IMFS.Web.Models.Email;
using IMFS.Web.Models.Enums;
using IMFS.Web.Models.OTC;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IMFS.Web.Models.Quote;
using IMFS.BusinessLogic.UserManagement;
using IMFS.Core;
using IMFS.Web.Models.DBModel;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using IMFS.BusinessLogic.ApplicationManagement;
using IMFS.Web.Models.Application;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class QuoteAcceptanceController : BaseController
    {
        private IEmailManager _emailManager;
        private IQuoteManager _quoteManager;
        private IIMFSEmailService _imfsEmailService;
        private readonly IConfiguration _configuration;
        private IQuoteAcceptanceManager _quoteAcceptanceManager;
        private IUserManager _userManager;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private IApplicationManager _applicationManager;


        public QuoteAcceptanceController(IEmailManager emailManager, IIMFSEmailService imfsEmailService,
            IConfiguration configuration, IQuoteManager quoteManager,
            IQuoteAcceptanceManager quoteAcceptanceManager,
            IUserManager userManager, IApplicationManager applicationManager)
        {
            _emailManager = emailManager;
            _userManager = userManager;
            _imfsEmailService = imfsEmailService;
            _configuration = configuration;
            _quoteManager = quoteManager;
            _quoteAcceptanceManager = quoteAcceptanceManager;
            _applicationManager = applicationManager;
        }

        [HttpPost]
        [Route("VerifyCode")]
        public IActionResult VerifyCode(OTCModel otcModel)
        {
            try
            {
                _logger.Info("Verify Code Model:" + JsonConvert.SerializeObject(otcModel));

                var verified = _quoteAcceptanceManager.VerifyCode(otcModel);

                if (verified)
                {
                    var otcObject = _quoteAcceptanceManager.GetOTC(otcModel);

                    if (otcObject != null)
                    {
                        otcObject.Accepted = DateTime.Now.ToLocalTime();
                        otcObject.AcceptedIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                        _quoteAcceptanceManager.UpdateOTC(otcObject);


                        //Create Quote Log
                        _quoteManager.InsertQuoteLog(Convert.ToInt32(otcModel.QuoteId), IMFSEnums.QuoteLogTypes.QuoteAccepted.ToString(),
                            string.Format("Code: {0} accepted for Quote: {1} ", otcModel.Code, otcModel.QuoteId));

                        //Verify & Create Application for - HPEFS
                        var result = _applicationManager.VerifyCreatedApplication(Convert.ToInt32(otcModel.QuoteId));
                        if (result.IsHPEFSFunder)
                        {
                            if (result.HasError)
                                return StatusCode(412, new { status = "Failed", data = result, message = "Error" });
                            else
                                return Ok(new { status = "Success", data = result, message = "HPEFS application created successfully," + result.ErrorMessage });
                        }


                        //Update Quote Status to 'End User/Customer Accepted'
                        _quoteManager.UpdateQuoteStatus(Convert.ToInt32(otcModel.QuoteId), Convert.ToInt32(_configuration.GetValue<string>("EndCustomerAcceptedStatus")));

                        var quoteDetails = _quoteManager.GetQuoteDetails(otcModel.QuoteId);
                        if (quoteDetails.HasError)
                        {
                            //logger
                            _logger.Error(string.Format("Application cannot be created for Quote: {0}", quoteDetails.QuoteDetails.Id));
                        }
                        else
                        {

                            var outputFile = GenerateApplicationPdf(quoteDetails.QuoteDetails);
                            if (!string.IsNullOrEmpty(outputFile))
                            {
                                #region Attachments                                
                                List<Attachment> attachmentList = new List<Attachment>();

                                var filePath = outputFile;
                                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                                Stream fileStream = new MemoryStream(fileBytes);
                                attachmentList.Add(new Attachment(fileStream, outputFile.Split("\\").Last()));

                                //Create Quote Log
                                _quoteManager.InsertQuoteLog(quoteDetails.QuoteDetails.Id, IMFSEnums.QuoteLogTypes.AttachFile.ToString(), string.Format("Document: {0} attached", filePath));


                                #endregion Attachments

                                SendEmail(quoteDetails.QuoteDetails, attachmentList, outputFile);
                            }

                        }


                    }
                    return Ok(new { status = "Success", data = "", message = "Code verified successfully" });
                }
                else
                {
                    return BadRequest(new { status = "Failed", message = "Code doesn't match, please try again" });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }


            return Ok(new { status = "Success", data = "", message = "Code verified successfully" });
        }


        private string GenerateApplicationPdf(QuoteDetailsModel quoteDetails)
        {
            string outputFile = string.Empty;
            try
            {

                string pdfTemplate = _configuration.GetValue<string>("ApplicationPdfTemplate");
                string physicalPath = _configuration.GetValue<string>("EmailAttachmentTempRootDirectory") + "\\" + "LoggedInUser";
                // create own folder for each user id if it does not exist
                Tools.CreateFolder(physicalPath);
                string fileName = "IMFS Equipment Finance Application - " + System.DateTime.Now.ToLocalTime().ToString("ddMMyyyyHHmmss") + " - " + quoteDetails.Id + ".pdf";
                outputFile = physicalPath + "\\" + fileName;
                using (Stream pdfInputStream = new FileStream(path: pdfTemplate, mode: FileMode.Open))
                using (Stream resultPDFOutputStream = new FileStream(path: outputFile, mode: FileMode.Create))
                using (Stream resultPDFStream = FillPdfForm(pdfInputStream, quoteDetails))
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


        private Stream FillPdfForm(Stream inputStream, QuoteDetailsModel model)
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
                form.SetField("Customer", model.EndUserDetails.EndCustomerName);
                form.SetField("ACNABN", model.EndUserDetails.EndCustomerABN);
                form.SetField("Trading name", model.EndUserDetails.EndCustomerName);

                var endCustomerpostalAddress = model.EndUserDetails.EndCustomerAddressLine1 + ",";

                if (!string.IsNullOrEmpty(model.EndUserDetails.EndCustomerAddressLine2))
                    endCustomerpostalAddress += model.EndUserDetails.EndCustomerAddressLine2 + ",";

                endCustomerpostalAddress += model.EndUserDetails.EndCustomerCity + " " +
                    model.EndUserDetails.EndCustomerState + " " +
                    model.EndUserDetails.EndCustomerPostCode;

                //Business Address
                //form.SetField("Business address", endCustomerpostalAddress);
                form.SetField("BusinessAddressLine1", model.EndUserDetails.EndCustomerAddressLine1);
                form.SetField("BusinessAddressLine2", model.EndUserDetails.EndCustomerAddressLine2);
                form.SetField("BusinessAddressCity", model.EndUserDetails.EndCustomerCity);
                form.SetField("BusinessAddressState", model.EndUserDetails.EndCustomerState);
                form.SetField("BusinessAddressPostcode", model.EndUserDetails.EndCustomerPostCode);
                form.SetField("BusinessAddressCountry", model.EndUserDetails.EndCustomerCountry);

                //Postal Address
                //form.SetField("Postal address", endCustomerpostalAddress);
                form.SetField("PostalAddressLine1", model.EndUserDetails.EndCustomerAddressLine1);
                form.SetField("PostalAddressLine2", model.EndUserDetails.EndCustomerAddressLine2);
                form.SetField("PostalAddressCity", model.EndUserDetails.EndCustomerCity);
                form.SetField("PostalAddressState", model.EndUserDetails.EndCustomerState);
                form.SetField("PostalAddressPostCode", model.EndUserDetails.EndCustomerPostCode);
                form.SetField("PostalAddressCountry", model.EndUserDetails.EndCustomerCountry);

                //Not populating goods address
                //var endCustomerDeliveryAddress = string.Empty;
                //form.SetField("Goods address", "");


                form.SetField("Phone", model.EndUserDetails.EndCustomerPhone);
                form.SetField("Contact name", model.EndUserDetails.EndCustomerContact);
                form.SetField("Mobile", model.EndUserDetails.EndCustomerPhone);
                form.SetField("Email", model.EndUserDetails.EndCustomerEmail);

                form.SetField("Years in principal business activity", model.EndUserDetails.EndCustomerYearsTrading);
                form.SetField("Finance quote number", Convert.ToString(model.Id));

                //if (model.QuoteHeader.Frequency == "Monthly")
                //    form.SetField("Freq_Monthly", "Yes");
                //if (model.QuoteHeader.Frequency == "Quarterly")
                //    form.SetField("Freq_Quarterly", "Yes");
                //if (model.QuoteHeader.Frequency == "Yearly")
                //    form.SetField("Freq_Yearly", "Yes");

                if (model.QuoteHeader.Frequency == "Monthly")
                    form.SetField("PaymentFrequencyPicker", "Monthly");
                if (model.QuoteHeader.Frequency == "Quarterly")
                    form.SetField("PaymentFrequencyPicker", "Quarterly");
                if (model.QuoteHeader.Frequency == "Yearly")
                    form.SetField("PaymentFrequencyPicker", "Yearly");

                if (model.QuoteHeader.FinanceType == "1")
                    form.SetField("FinanceTypePicker", "Lease");
                if (model.QuoteHeader.FinanceType == "2")
                    form.SetField("FinanceTypePicker", "Rental");
                if (model.QuoteHeader.FinanceType == "4")
                    form.SetField("FinanceTypePicker", "Instalment");



                form.SetField("Finance quote value", model.QuoteHeader.QuoteTotal.Value.ToString("0.00"));
                form.SetField("Finance quote payment", model.QuoteHeader.FinanceValue.Value.ToString("0.00"));
                form.SetField("Finance Term", model.QuoteHeader.QuoteDuration);

                form.SetField("Sales representative", model.CustomerDetails.CustomerContact);
                form.SetField("Dealername", model.CustomerDetails.CustomerName);

                form.SetField("Equipment description", model.QuoteHeader.QuoteName);

                //form.SetField(SampleFormFieldNames.IAmAwesomeCheck, model.AwesomeCheck ? "Yes" : "Off");
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

        private string SendEmail(QuoteDetailsModel quoteDetails, List<Attachment> attachmentList, string pdfTempFilePath)
        {

            int emailId = 0;
            var email = new IMFS.Web.Models.DBModel.Emails();
            email.FromAddress = _configuration.GetValue<string>("DefaultFromAddress");
            email.ToAddress = quoteDetails.EndUserDetails.EndCustomerEmail;
            email.CCEmail = quoteDetails.CustomerDetails.CustomerEmail;
            email.CCEmail += "," + _configuration.GetValue<string>("ApplicationDefaultCcAddress"); //Replace by QuoteCreated By

            email.Subject = "IMFS Application form (" + quoteDetails.Id + ")";

            var emailbody = _configuration.GetValue<string>("ApplicationFormEmail");

            emailbody = emailbody.Replace("{{EndCustomerName}}", quoteDetails.EndUserDetails.EndCustomerName);
            emailbody = emailbody.Replace("{{QuoteNumber}}", quoteDetails.QuoteHeader.QuoteNumber);
            emailbody = emailbody.Replace("{{DefaultFromAddress}}", _configuration.GetValue<string>("DefaultFromAddress"));
            if (!string.IsNullOrEmpty(quoteDetails.QuoteHeader.QuoteCreatedBy))
            {
                var user = _userManager.GetUserDetails(quoteDetails.QuoteHeader.QuoteCreatedBy);
                if (user != null)
                {
                    emailbody = emailbody.Replace("{{QuoteCreatedBy}}", user.FirstName + " " + user.LastName);
                    emailbody = emailbody.Replace("{{QuoteCreatedByPhone}}", user.PhoneNumber);
                }
            }

            email.Body = Regex.Replace(emailbody, @"[^\u0000-\u007F]+", string.Empty).Trim();  // remove hidden characters

            var messageId = String.Format("<{0}@{1}>", Guid.NewGuid().ToString(), "ingrammicro.com");

            email.InternetMessageId = messageId;

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

            string physicalPath = _configuration.GetValue<string>("EmailAttachmentRootDirectory");
            var foldername = DateTime.Today.ToString("dd'.'MM'.'yyyy");
            Tools.CreateFolder(physicalPath, foldername);

            foreach (var attachment in attachmentList)
            {
                var attachmentobj = new EmailAttachment();

                attachmentobj.EmailId = emailId;
                attachmentobj.FileName = attachment.Name.Split("\\").Last();
                attachmentobj.PhysicalPath = physicalPath + "\\" + foldername;
                attachmentobj.ContentId = "pdf";
                _emailManager.SaveAttachment(attachmentobj);

                var sourceFile = _configuration.GetValue<string>("EmailAttachmentTempRootDirectory") + "\\" + "LoggedInUser" + "\\" + attachmentobj.FileName;
                var newFile = attachmentobj.PhysicalPath + "\\" + attachmentobj.Id + Path.GetExtension(attachmentobj.FileName);
                Tools.CopyFile(sourceFile, newFile);
                Tools.DeleteFile(sourceFile);

            }
            #endregion

            //Create Quote Log
            _quoteManager.InsertQuoteLog(quoteDetails.Id, IMFSEnums.QuoteLogTypes.EmailSent.ToString(), string.Format("Email: {0} Sent to End User", emailId));

            string message = string.Empty;
            try
            {

                _imfsEmailService.Send(email.FromAddress, email.ToAddress, email.CCEmail, string.Empty, email.Subject, emailbody, attachmentList, null, messageId);

                //Create an email reference with quote
                _emailManager.CreateEmailXref(emailId, Convert.ToInt32(quoteDetails.Id));


                //Create Quote Log
                _quoteManager.InsertQuoteLog(Convert.ToInt32(quoteDetails.Id),
                    IMFSEnums.QuoteLogTypes.ApplicationFormSent.ToString(), string.Format("Email: {0} Sent to End Customer for Application", emailId));

                //Update Quote Status
                _quoteManager.UpdateQuoteStatus(Convert.ToInt32(quoteDetails.Id), _configuration.GetValue<int>("EndCustomerAwaitingApplication"));

            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("in QuoteAcceptance SendEmail():{0}", ex.Message));
                return null;
            }
            return "Email sent successfully";
        }

        [HttpPost]
        [Route("SendCodeEmail")]
        public IActionResult SendCodeEmail(SendEmailViewModel model)
        {
            try
            {

                //Delete all previous codes generated for this quote
                _quoteAcceptanceManager.DeletePreviousCodes(Convert.ToInt32(model.QuoteId));

                //Get the code length from settings
                int minCodeLength = _configuration.GetValue<int>("AcceptQuoteEmailMinCodeRange");
                int maxCodeLength = _configuration.GetValue<int>("AcceptQuoteEmailMaxCodeRange");

                Random rndCode = new Random();
                int encryptedCode = rndCode.Next(minCodeLength, maxCodeLength);

                int emailId = 0;

                //var encryptedCode = Convert.ToBase64String(Guid.NewGuid().ToByteArray()); 

                int tempQuoteId;
                int.TryParse(model.QuoteId, out tempQuoteId);

                var emailbody = _configuration.GetValue<string>("VerificationEmail");
                emailbody = emailbody.Replace("{{Code}}", Convert.ToString(encryptedCode));

                var messageId = String.Format("<{0}@{1}>", Guid.NewGuid().ToString(), "ingrammicro.com");

                var email = new IMFS.Web.Models.DBModel.Emails();
                email.FromAddress = model.FromAddress;
                email.ToAddress = model.ToAddress;
                email.CCEmail = model.CCEmail;
                email.BCCEmail = model.BCCEmail;
                email.Subject = model.Subject;
                email.EmailType = IMFSEnums.EmailType.Sent.ToString();
                email.Body = Regex.Replace(emailbody, @"[^\u0000-\u007F]+", string.Empty).Trim();  // remove hidden characters
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

                model.Body = email.Body;

                //save email
                email.Status = IMFSEnums.EmailStatus.Outbound.ToString();

                _emailManager.SaveRepliedEmails(email);
                emailId = email.Id;

                //Create an email reference with quote
                _emailManager.CreateEmailXref(emailId, tempQuoteId);

                //Create Quote Log
                _quoteManager.InsertQuoteLog(tempQuoteId, IMFSEnums.QuoteLogTypes.EmailSent.ToString(), string.Format("Email: {0} Sent to End User", emailId));


                //create a job and save
                string message = string.Empty;
                try
                {

                    _imfsEmailService.Send(model.FromAddress, model.ToAddress, model.CCEmail, model.BCCEmail, model.Subject, model.Body, null, null, messageId);

                    //Add Record in One Time Code table
                    _quoteAcceptanceManager.InsertOTC(new Models.DBModel.OTC()
                    {
                        QuoteId = Convert.ToInt32(model.QuoteId),
                        Code = Convert.ToString(encryptedCode),
                        Sent = DateTime.Now.ToLocalTime(),
                        RequestedIP = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                        Recipient = model.ToAddress
                    });

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

        [Route("GetDecodedQuoteId")]
        [HttpPost]
        public IActionResult GetDecodedQuoteId(OTCEncryptionModel otcEncryptionModel)
        {
            try
            {
                var key = _configuration.GetValue<string>("CipherKey");

                var decrypted = EncryptDecryptHelper.DecryptString(otcEncryptionModel.EncryptedQuoteId, key);

                return Ok(new { status = "Success", data = decrypted, message = "Decoded successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        [Route("RequestChangesQuote")]
        [HttpGet]
        public IActionResult RequestChangesQuote(string quoteId)
        {
            try
            {
                int emailId = 0;
                //Get Quote Details
                var quoteDetails = _quoteManager.GetQuoteDetails(quoteId);

                var emailbody = _configuration.GetValue<string>("RequestChangesEmail");
                emailbody = emailbody.Replace("{{EndCustomerName}}", quoteDetails.QuoteDetails.EndUserDetails.EndCustomerName);
                emailbody = emailbody.Replace("{{QuoteNumber}}", quoteDetails.QuoteDetails.QuoteHeader.QuoteNumber);
                emailbody = emailbody.Replace("{{EndCustomerName}}", quoteDetails.QuoteDetails.EndUserDetails.EndCustomerName);
                emailbody = emailbody.Replace("{{EndCustomerEmail}}", quoteDetails.QuoteDetails.EndUserDetails.EndCustomerEmail);
                emailbody = emailbody.Replace("{{EndCustomerPhone}}", quoteDetails.QuoteDetails.EndUserDetails.EndCustomerPhone);
                emailbody = emailbody.Replace("{{QuoteLink}}", _configuration.GetValue<string>("QuoteLink").Replace("{{QuoteNumber}}", quoteDetails.QuoteDetails.QuoteHeader.QuoteNumber));


                var email = new IMFS.Web.Models.DBModel.Emails();
                email.FromAddress = _configuration.GetValue<string>("DefaultFromAddress");

                bool validEmailAddress = false;
                try
                {
                    MailAddress m = new MailAddress(quoteDetails.QuoteDetails.CustomerDetails.CustomerEmail);
                    validEmailAddress = true;
                }
                catch (FormatException)
                {
                    validEmailAddress = false;
                }

                //TODO: enable it later
                //if (validEmailAddress)
                //{

                email.ToAddress = quoteDetails.QuoteDetails.CustomerDetails.CustomerEmail;

                email.CCEmail = _configuration.GetValue<string>("DefaultCcAddress");
                email.Subject = "End Customer request for changes in quote " + quoteDetails.QuoteDetails.QuoteHeader.QuoteNumber;

                email.EmailType = IMFSEnums.EmailType.Sent.ToString();
                email.Body = Regex.Replace(emailbody, @"[^\u0000-\u007F]+", string.Empty).Trim();  // remove hidden characters
                var messageId = String.Format("<{0}@{1}>", Guid.NewGuid().ToString(), "ingrammicro.com");
                email.InternetMessageId = messageId;

                email.BodyType = IMFSEnums.EmailBodyType.HTML.ToString();
                email.Status = IMFSEnums.EmailStatus.Completed.ToString();
                email.Importance = IMFSEnums.EmailImportance.Normal.ToString();
                email.DateTimeCreated = DateTime.Now.ToLocalTime();
                email.DateTimeReceived = DateTime.Now.ToLocalTime();


                email.Notes = DateTime.Now.ToString() + " - " + "LoggedInUser" + " - " + "Email sent by " + "LoggedInUser";

                var emailSendModel = new SendEmailViewModel();
                emailSendModel.FromAddress = email.FromAddress;
                emailSendModel.ToAddress = email.ToAddress;
                emailSendModel.CCEmail = string.Empty;
                emailSendModel.BCCEmail = string.Empty;
                emailSendModel.Subject = email.Subject;
                emailSendModel.Body = email.Body;

                //save email
                email.Status = IMFSEnums.EmailStatus.Outbound.ToString();

                _emailManager.SaveRepliedEmails(email);
                emailId = email.Id;

                //Create an email reference with quote
                _emailManager.CreateEmailXref(emailId, Convert.ToInt32(quoteDetails.QuoteDetails.QuoteHeader.QuoteNumber));

                //Create Quote Log
                _quoteManager.InsertQuoteLog(Convert.ToInt32(quoteDetails.QuoteDetails.QuoteHeader.QuoteNumber),
                    IMFSEnums.QuoteLogTypes.EmailSent.ToString(), string.Format("Email: {0} Sent to Reseller", emailId));

                //Update Quote Status
                _quoteManager.UpdateQuoteStatus(Convert.ToInt32(quoteDetails.QuoteDetails.QuoteHeader.QuoteNumber), _configuration.GetValue<int>("RequestChangesStatus"));

                //Send Email
                try
                {
                    _imfsEmailService.Send(emailSendModel.FromAddress, emailSendModel.ToAddress, emailSendModel.CCEmail, emailSendModel.BCCEmail, emailSendModel.Subject, emailSendModel.Body, null, null, messageId);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { status = "Failed", error = ex.ToString() });
                }


                return Ok(new { status = "Success", data = emailId, message = _configuration.GetValue<string>("RequestChangesResponseMessage") });
                //}
                //else
                //{
                //    return BadRequest(new { status = "Failed", message = "Reseller email address is not valid" });
                //}

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        [Route("RejectQuote")]
        [HttpPost]
        public IActionResult RejectQuote(RejectQuoteDetail model)
        {
            try
            {
                // call update method
                int emailId = 0;
                bool result = false;
                //Get Quote Details
                var quoteDetails = _quoteManager.GetQuoteDetails(model.QuoteId);

                if (quoteDetails != null)
                {
                    result = _quoteManager.RejectQuoteDetails(model);
                    var emailbody = _configuration.GetValue<string>("RejectQuoteEmail");
                    emailbody = emailbody.Replace("{{EndCustomerName}}", quoteDetails.QuoteDetails.EndUserDetails.EndCustomerName);
                    emailbody = emailbody.Replace("{{QuoteNumber}}", quoteDetails.QuoteDetails.QuoteHeader.QuoteNumber);
                    emailbody = emailbody.Replace("{{EndCustomerName}}", quoteDetails.QuoteDetails.EndUserDetails.EndCustomerName);
                    emailbody = emailbody.Replace("{{EndCustomerEmail}}", quoteDetails.QuoteDetails.EndUserDetails.EndCustomerEmail);
                    emailbody = emailbody.Replace("{{EndCustomerPhone}}", quoteDetails.QuoteDetails.EndUserDetails.EndCustomerPhone);
                    emailbody = emailbody.Replace("{{RejectReason}}", model.Reason);
                    emailbody = emailbody.Replace("{{Comment}}", model.Comment);
                    emailbody = emailbody.Replace("{{QuoteLink}}", _configuration.GetValue<string>("QuoteLink").Replace("{{QuoteNumber}}", quoteDetails.QuoteDetails.QuoteHeader.QuoteNumber));

                    var email = new IMFS.Web.Models.DBModel.Emails();
                    email.FromAddress = _configuration.GetValue<string>("DefaultFromAddress");

                    bool validEmailAddress = false;
                    try
                    {
                        MailAddress m = new MailAddress(quoteDetails.QuoteDetails.CustomerDetails.CustomerEmail);
                        validEmailAddress = true;
                    }
                    catch (FormatException)
                    {
                        validEmailAddress = false;
                    }

                    email.ToAddress = quoteDetails.QuoteDetails.CustomerDetails.CustomerEmail;

                    email.CCEmail = _configuration.GetValue<string>("DefaultCcAddress"); ;
                    email.Subject = "End Customer rejected quote " + quoteDetails.QuoteDetails.QuoteHeader.QuoteNumber;

                    email.EmailType = IMFSEnums.EmailType.Sent.ToString();
                    email.Body = Regex.Replace(emailbody, @"[^\u0000-\u007F]+", string.Empty).Trim();  // remove hidden characters
                    var messageId = String.Format("<{0}@{1}>", Guid.NewGuid().ToString(), "ingrammicro.com");
                    email.InternetMessageId = messageId;

                    email.BodyType = IMFSEnums.EmailBodyType.HTML.ToString();
                    email.Status = IMFSEnums.EmailStatus.Completed.ToString();
                    email.Importance = IMFSEnums.EmailImportance.Normal.ToString();
                    email.DateTimeCreated = DateTime.Now.ToLocalTime();
                    email.DateTimeReceived = DateTime.Now.ToLocalTime();


                    email.Notes = DateTime.Now.ToString() + " - " + "LoggedInUser" + " - " + "Email sent by " + "LoggedInUser";

                    var emailSendModel = new SendEmailViewModel();
                    emailSendModel.FromAddress = email.FromAddress;
                    emailSendModel.ToAddress = email.ToAddress;
                    emailSendModel.CCEmail = string.Empty;
                    emailSendModel.BCCEmail = string.Empty;
                    emailSendModel.Subject = email.Subject;
                    emailSendModel.Body = email.Body;

                    //save email
                    email.Status = IMFSEnums.EmailStatus.Outbound.ToString();

                    _emailManager.SaveRepliedEmails(email);
                    emailId = email.Id;

                    //Create an email reference with quote
                    _emailManager.CreateEmailXref(emailId, Convert.ToInt32(quoteDetails.QuoteDetails.QuoteHeader.QuoteNumber));

                    //Create Quote Log
                    _quoteManager.InsertQuoteLog(Convert.ToInt32(quoteDetails.QuoteDetails.QuoteHeader.QuoteNumber),
                        IMFSEnums.QuoteLogTypes.EmailSent.ToString(), string.Format("Email: {0} Sent to Reseller", emailId));

                    //Update Quote Status
                    _quoteManager.UpdateQuoteStatus(Convert.ToInt32(quoteDetails.QuoteDetails.QuoteHeader.QuoteNumber), _configuration.GetValue<int>("EndCustomerRejectedStatus"));

                    //Send Email
                    try
                    {
                        _imfsEmailService.Send(emailSendModel.FromAddress, emailSendModel.ToAddress, emailSendModel.CCEmail, emailSendModel.BCCEmail, emailSendModel.Subject, emailSendModel.Body, null, null, messageId);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { status = "Failed", error = ex.ToString() });
                    }

                    return Ok(new { status = "Success", data = emailId, message = _configuration.GetValue<string>("RejectQuoteResponseMessage") });
                }
                else
                    return Ok(new { status = "Failed", error = "This Quote not exist" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        //[HttpPost]
        //[Route("GetComboRequest")]
        //public IActionResult TempHPAPIComboRequest(CreateComboRequest request)
        //{
        //    try
        //    {

        //        //_applicationManager.TempCreateComboRequest(applicationId, xml);
        //        var xml = _applicationManager.GetComboRequestTest(request);

        //        return Ok(xml);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { status = "Failed", error = ex.ToString() });
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("GetComboRequestStatus")]
        //public IActionResult TempGetComboRequestStatus(ComboRequestForStatus request)
        //{
        //    try
        //    {
        //        var xml = _applicationManager.GetComboRequestStatusTest(request);

        //        return Ok(xml);
        //        //return Ok(new { status = "Success", data = response, message = "Sucessed" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { status = "Failed", error = ex.ToString() });
        //    }
        //}

        //[HttpPost]
        //[Route("GetAcceptQuoteComboRequest")]
        //public IActionResult TempGetACComboRequest(AcceptQuoteRequest request)
        //{
        //    try
        //    {
        //        var xml = _applicationManager.GetAcceptQuoteComboRequestTest(request);

        //        return Ok(xml);
        //        //return Ok(new { status = "Success", data = response, message = "Sucessed" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { status = "Failed", error = ex.ToString() });
        //    }
        //}
    }


}
