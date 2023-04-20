using IMFS.Core;
using IMFS.DataAccess.Repository;
using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Email;
using IMFS.Web.Models.Enums;
using IMFS.Web.Models.Misc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace IMFS.BusinessLogic.Emails
{
    public class EmailManager: IEmailManager
    {
        private readonly IRepository<vw_Emails> _vwemailRepository;       
        private readonly IRepository<EmailsHTMLBody> _emailsHTMLBodyRepository;        
        private readonly IRepository<EmailAttachment> _emailAttachmentRepository;
        private readonly IRepository<EmailAttachmentsTemp> _emailAttachmentsTempRepository;
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;
        private readonly IRepository<EmailXref> _emailXrefRepository;
        private readonly IRepository<QuoteLog> _quoteLogRepository;
        private readonly IRepository<IMFS.Web.Models.DBModel.Emails> _emailsRepository;
        private readonly IRepository<IMFS.Web.Models.DBModel.EmailsArchived> _emailsArchivedRepository;

        public IConfiguration Configuration { get; }
        

        public EmailManager(
            IRepository<vw_Emails> vwemailsRepository,
            IRepository<EmailsHTMLBody> emailsHTMLBodyRepository,
            IRepository<EmailAttachment> emailAttachmentRepository,
            IRepository<EmailAttachmentsTemp> emailAttachmentsTempRepository,
            IRepository<EmailTemplate> emailTemplateRepository,
            IRepository<EmailXref> emailXrefRepository,
            IRepository<QuoteLog> quoteLogRepository,
            IRepository<IMFS.Web.Models.DBModel.Emails> emailsRepository,
            IRepository<IMFS.Web.Models.DBModel.EmailsArchived> emailsArchivedRepository,
            IConfiguration configuration
            )
        {
            _vwemailRepository = vwemailsRepository;
            _emailsHTMLBodyRepository = emailsHTMLBodyRepository;
            _emailAttachmentRepository = emailAttachmentRepository;
            _emailAttachmentsTempRepository = emailAttachmentsTempRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _emailXrefRepository = emailXrefRepository;
            _quoteLogRepository = quoteLogRepository;
            _emailsRepository = emailsRepository;
            _emailsArchivedRepository = emailsArchivedRepository;
            Configuration = configuration;
        }

        public void AddNotes(int emailId, string newNote)
        {
            var emailDetails = _vwemailRepository.Table.Where(x => x.Id == emailId).AsNoTracking().FirstOrDefault();
            if (emailDetails != null)
            {
                //newNote = DateTime.Now.ToString() + " - " + _contextManager.GetCurrentUserName() + " - " + newNote;
                newNote = DateTime.Now.ToString() + " - " + "loggedUser" + " - " + newNote;
                if (!string.IsNullOrEmpty(emailDetails.Notes))
                {
                    emailDetails.Notes = Environment.NewLine + emailDetails.Notes;
                }
                emailDetails.Notes = newNote + emailDetails.Notes;
                //_emailRepository.UpdateEmails(emailDetails);
                UpdateEmails(emailDetails);
            }
        }

        /*
        vw_Emails GetEmailById(int emailId);
        List<Models.Email> GetNewEmails(int queueId);
        void UpdateEmails(vw_Emails existingEmail);
        void InsertEmail(Email email);
        List<Email> SearchEmails(spGetEmails input);
        List<vw_Emails> GetEmailsByIds(List<int> emailIds);
        */

        public void AddNotes(int emailId, string currentUserName, string newNote)
        {
            var existingEmail = _vwemailRepository.Table.Where(x => x.Id == emailId).AsNoTracking().FirstOrDefault();
            if (existingEmail != null)
            {
                newNote = DateTime.Now.ToString() + " - " + currentUserName + " - " + newNote;
                if (!string.IsNullOrEmpty(existingEmail.Notes))
                {
                    existingEmail.Notes = Environment.NewLine + existingEmail.Notes;
                }
                existingEmail.Notes = newNote + existingEmail.Notes;

                UpdateEmails(existingEmail);
            }
        }

        public void UpdateEmails(vw_Emails emailDetails)
        {
            //Find email in first table
            var email = _emailsRepository.Table.Where(x => x.Id == emailDetails.Id).AsNoTracking().FirstOrDefault();
            if(email != null)
            {
                email.Notes = emailDetails.Notes;
                _emailsRepository.Update(email);
            }

            var emailArchived = _emailsArchivedRepository.Table.Where(x => x.Id == emailDetails.Id).AsNoTracking().FirstOrDefault();
            if (emailArchived != null)
            {
                emailArchived.Notes = emailDetails.Notes;
                _emailsArchivedRepository.Update(emailArchived);
            }
        }

        public void CreateEmailXref(int emailId, int quoteId=0, int applicationId = 0, int contractId = 0)
        {
            EmailXref xref = new EmailXref();
            xref.EmailID = emailId;
            xref.QuoteID = quoteId;
            xref.ApplicationID = applicationId;
            xref.ContractID = contractId;

            _emailXrefRepository.Insert(xref);
        }

        public List<EmailHistory> GetApplicationEmailHistory(int applicationId)
        {
            List<EmailHistory> emailHistory = new List<EmailHistory>();
            var emailXref = _emailXrefRepository.Table.Where(x => x.ApplicationID == applicationId).ToList();

            foreach (var xref in emailXref)
            {
                var history = new EmailHistory();
                var email = _vwemailRepository.Table.Where(x => x.Id == xref.EmailID).FirstOrDefault();
                if (email != null)
                {
                    history.EmailId = email.Id;
                    history.ApplicationId = applicationId;                    
                    history.To = email.ToAddress;
                    history.Subject = email.Subject;

                    var dateFormat = Configuration.GetSection("DateFormat").Value;
                    var timeFormat = Configuration.GetSection("TimeFormat").Value;

                    history.Date = email.DateTimeCreated.Value.ToString(dateFormat);
                    history.Time = email.DateTimeCreated.Value.ToString(timeFormat);


                    emailHistory.Add(history);
                }
            }

            return emailHistory;
        }

        public List<EmailHistory> GetQuoteEmailHistory(int quoteId)
        {
            List<EmailHistory> emailHistory = new List<EmailHistory>();
            var emailXref = _emailXrefRepository.Table.Where(x => x.QuoteID == quoteId).ToList();

            foreach(var xref in emailXref)
            {
                var history = new EmailHistory();
                var email = _vwemailRepository.Table.Where(x => x.Id == xref.EmailID).FirstOrDefault();
                if(email != null)
                {
                    history.EmailId = email.Id;
                    history.QuoteId = quoteId;
                    history.To = email.ToAddress;
                    history.Subject = email.Subject;

                    var dateFormat = Configuration.GetSection("DateFormat").Value;
                    var timeFormat = Configuration.GetSection("TimeFormat").Value;

                    history.Date = email.DateTimeCreated.Value.ToString(dateFormat);
                    history.Time = email.DateTimeCreated.Value.ToString(timeFormat);


                    emailHistory.Add(history);
                }
            }

            return emailHistory;
        }

        public List<EmailAttachmentModel> AttachQuoteDownload(AttachQuoteModel inputModel, string emailAttachmentsFolder)
        {
            throw new NotImplementedException();
        }

        public void ChangeEmailStatus(IMFSEnums.EmailStatus newStatus, int emailId)
        {
            throw new NotImplementedException();
        }

        public List<LinkedResource> ConverBase64StringsToResources(int emailId, string emailHTML, out string newEmailHTML, out Dictionary<string, string> linkedResourcesCollection)
        {
            List<LinkedResource> linkedResources = new List<LinkedResource>();
            linkedResourcesCollection = new Dictionary<string, string>();
            string imgStartTag = "<img ";
            string imgEndtTag = ">";
            int startImgIndex = -1;
            int endImgIndex = -1;
            int imageCounter = 0;
            do
            {
                startImgIndex = emailHTML.IndexOf(imgStartTag, startImgIndex + 1);
                if (startImgIndex > 0)
                {
                    endImgIndex = emailHTML.IndexOf(imgEndtTag, startImgIndex);
                    string imgHTML = emailHTML.Substring(startImgIndex, endImgIndex - startImgIndex + 1);
                    if (imgHTML.ToLower().Contains("image removed by sender"))
                    {
                        continue;
                    }
                    //extract src value and check if it is not valid url
                    int cursor = imgHTML.IndexOf("src");
                    if (cursor > 0)
                    {
                        int sourceStartIndex = imgHTML.IndexOf("\"", cursor);
                        int sourceEndIndex = imgHTML.IndexOf("\"", sourceStartIndex + 1);
                        string src = imgHTML.Substring(sourceStartIndex, sourceEndIndex - sourceStartIndex);
                        if (string.IsNullOrEmpty(src))
                        {
                            continue;
                        }
                        if (src.ToLower().IndexOf("https://") >= 0 || src.ToLower().IndexOf("http://") >= 0)
                        {
                            continue;
                        }
                        var srcTextSplit = src.Split(',');
                        if (srcTextSplit.Length < 2)
                        {
                            continue;
                        }
                        var base64Text = srcTextSplit[1];
                        if (String.IsNullOrEmpty(base64Text))
                        {
                            continue;
                        }

                        var headerText = srcTextSplit[0];
                        var startPos = headerText.IndexOf('/');
                        var endPos = headerText.IndexOf(';');
                        var extension = headerText.Substring(startPos + 1, endPos - startPos - 1);
                        imageCounter++;
                        var newImageName = "image" + imageCounter + "." + extension;
                        string newSrc = "cid:" + newImageName;

                        byte[] imageBytes = Convert.FromBase64String(base64Text);
                        Stream imageStream = new MemoryStream(imageBytes);
                        LinkedResource PictureRes = new LinkedResource(imageStream);
                        if (string.IsNullOrEmpty(extension))
                        {
                            extension = "jpg";
                        }
                        PictureRes.ContentType.MediaType = "image/" + extension;
                        PictureRes.ContentId = newImageName;
                        string newImageHTML = string.Format("<img src=\"cid:{0}\" data-filename=\"{1}\" >", newImageName, newImageName);
                        emailHTML = emailHTML.Replace(imgHTML, newImageHTML);
                        linkedResources.Add(PictureRes);
                        linkedResourcesCollection.Add(newImageName, base64Text);
                    }
                }
            } while (startImgIndex > 0);
            newEmailHTML = emailHTML;
            return linkedResources;
        }

        public void DeleteEmailAttachmentTemp(int emailAttachmentTempId)
        {
            var objToDelete = _emailAttachmentsTempRepository.GetById(emailAttachmentTempId);
            if (objToDelete != null)
            {
                var fileName = objToDelete.Id + Path.GetExtension(objToDelete.FileName);
                Tools.DeleteFile(objToDelete.PhysicalPath + "\\" + fileName);
                _emailAttachmentsTempRepository.Delete(objToDelete);
            }
        }

        public void DeleteEmailTemplate(int id)
        {
            throw new NotImplementedException();
        }

        public EmailAttachment GetEmailAttachment(int emailId, string imageName, string contentId)
        {
            var emailAttachment = _emailAttachmentRepository.Table.Where(x => x.EmailId == emailId && (x.FileName == imageName || x.ContentId == contentId)).FirstOrDefault();
            return emailAttachment;
        }

        public EmailAttachment GetEmailAttachment(int attachmentId)
        {
            var emailAttachment = _emailAttachmentRepository.Table.Where(x => x.Id == attachmentId).FirstOrDefault();
            return emailAttachment;
        }

        public List<EmailAttachment> GetEmailAttachments(int emailId)
        {
            var res = _emailAttachmentRepository.Table.Where(x => x.EmailId == emailId).ToList();
            return res;
        }

        public List<EmailAttachmentsTemp> GetEmailAttachmentsTemp(Guid tempEmailId)
        {
            var res = _emailAttachmentsTempRepository.Table.Where(x => x.TempEmailId == tempEmailId).ToList();
            return res;
        }

        public EmailAttachmentsTemp GetEmailAttachmentTemp(int attachmentId)
        {
            var res = _emailAttachmentsTempRepository.Table.Where(x => x.Id == attachmentId).FirstOrDefault();
            return res;
        }

        public vw_Emails GetEmailByInternetMessageId(string internetMessageId, string subject, DateTime? receivedDateTime)
        {
            throw new NotImplementedException();
        }

        public vw_Emails GetEmailDetails(int emailId)
        {
            return _vwemailRepository.Table.Where(x => x.Id == emailId).FirstOrDefault(); 
        }

        public EmailModel GetEmailDetailsModel(int? emailId, bool loadHtml = false, bool loadQueueDetails = false, string webApiUrl = "")
        {
            throw new NotImplementedException();
        }

        public EmailBodyModel GetEmailHTMLBody(int emailId)
        {
            var emailBodyModel = new EmailBodyModel();
            string htmlBody = "";
            var emailHTMLBody = _emailsHTMLBodyRepository.Table.Where(x => x.Id == emailId).FirstOrDefault();
            if (emailHTMLBody != null)
            {                
                string webApiUrl = Configuration.GetSection("WebApiUrl").Value;
                List<string> imageContentIds = new List<string>();
                List<string> imageNames = new List<string>();
                htmlBody = ConvertImagesToApiSrc(emailId, emailHTMLBody.HTMLBody, webApiUrl, out imageContentIds, out imageNames);
                emailBodyModel.HtmlBody = htmlBody;
                emailBodyModel.ImageContentIds = imageContentIds;
                emailBodyModel.ImageNames = imageNames;
            }
            return emailBodyModel;
        }

        private string ConvertImagesToApiSrc(int emailId, string html, string baseUrl, out List<string> imageContentIds, out List<string> imageNames)
        {
            string imgStartTag = "<img ";
            string imgEndtTag = ">";
            string cidSourceTag = "src=\"cid:";
            int startImgIndex = -1;
            int endImgIndex = -1;
            imageContentIds = new List<string>();
            imageNames = new List<string>();

            // do not process if html is blank
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }
            do
            {
                startImgIndex = html.IndexOf(imgStartTag, startImgIndex + 1);
                if (startImgIndex > 0)
                {
                    try
                    {
                        endImgIndex = html.IndexOf(imgEndtTag, startImgIndex);
                        string imgHTML = html.Substring(startImgIndex, endImgIndex - startImgIndex + 1);
                        var cidSourceIndex = imgHTML.IndexOf(cidSourceTag);
                        if (cidSourceIndex != -1)
                        {
                            int cidValueStartIndex = cidSourceIndex + cidSourceTag.Length;
                            var cidSourceIndexEnd = imgHTML.IndexOf("\" ", cidValueStartIndex);
                            if (cidSourceIndexEnd == -1)
                            {
                                cidSourceIndexEnd = imgHTML.IndexOf("\"", cidValueStartIndex);
                            }
                            var imageName = "";
                            if (imgHTML.IndexOf("@") > 0)
                            {
                                imageName = imgHTML.Substring(cidValueStartIndex, imgHTML.IndexOf("@", cidValueStartIndex) - cidValueStartIndex);
                            }
                            else
                            {
                                imageName = imgHTML.Substring(cidValueStartIndex, cidSourceIndexEnd - cidValueStartIndex);
                            }
                            var contentId = imgHTML.Substring(cidValueStartIndex, cidSourceIndexEnd - cidValueStartIndex);
                            imageContentIds.Add(contentId);
                            imageNames.Add(imageName);
                            var imageAttachment = GetEmailAttachment(emailId, imageName, contentId);
                            if (imageAttachment != null)
                            {
                                string fileExtension = Path.GetExtension(imageAttachment.FileName);
                                string temporaryFileName = imageAttachment.Id.ToString() + fileExtension;
                                var filePath = imageAttachment.PhysicalPath + "\\" + temporaryFileName;
                                if (System.IO.File.Exists(filePath))
                                {
                                    string imageSrc = baseUrl + "/Attachment/DownloadImageAttachment?attachmentId=" + imageAttachment.Id.ToString();
                                    string oldSource = imgHTML.Substring(cidSourceIndex + 5, cidSourceIndexEnd - cidSourceIndex - 5);
                                    html = html.Replace(oldSource, imageSrc);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        startImgIndex++;
                        continue;
                    }
                }
            } while (startImgIndex > 0);
            return html;
        }

        public List<vw_Emails> GetEmailInfos(List<int> emailIds)
        {
            throw new NotImplementedException();
        }

        public EmailTemplate GetEmailTemplateDetails(int templateId)
        {
            var template = _emailTemplateRepository.Table.Where(x => x.DefaultID == templateId).AsNoTracking().FirstOrDefault();

            return template;                
        }

        public List<EmailTemplateModel> GetEmailTemplateList()
        {
            throw new NotImplementedException();
        }

        public List<LookupModel> GetEmailTemplatesLookup(int? id, List<int> vendorIds, List<int> secondaryCategoriesId, bool standaloneEmail = false)
        {
            throw new NotImplementedException();
        }

        public List<EmailListViewModel> GetNewEmails(int queueId)
        {
            throw new NotImplementedException();
        }

        public void MarkAllEmailsAsRead(int jobId)
        {
            throw new NotImplementedException();
        }

        public void ReportPhishingEmail(int emailId)
        {
            throw new NotImplementedException();
        }

        public ErrorModel SaveAttachment(EmailAttachment emailattachment)
        {
            var response = new ErrorModel();
            try
            {
                // find existing one
                // compare with filename and content id
                var existingAttachment = _emailAttachmentRepository.Table.Where(x => x.EmailId == emailattachment.EmailId && x.FileName == emailattachment.FileName && x.ContentId == emailattachment.ContentId).FirstOrDefault();
                if (existingAttachment != null)
                {
                    emailattachment.Id = existingAttachment.Id;
                }
                else
                {
                    _emailAttachmentRepository.Insert(emailattachment);
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = Tools.GetErrorMessage(ex);
            }
            return response;

            // throw new NotImplementedException();
        }

        public int SaveEmailAttachmentTemp(EmailAttachmentModel attachment)
        {
            EmailAttachmentsTemp newAttachment = new EmailAttachmentsTemp();
            try
            {
                newAttachment.FileName = attachment.FileName;
                newAttachment.TempEmailId = attachment.TempEmailId;
                newAttachment.PhysicalPath = attachment.PhysicalPath;
                newAttachment.UploadedBy = attachment.UploadedBy;
                newAttachment.CreatedDate = DateTime.Now;

                var todayDate = DateTime.Now.Date;
                // clear other temporary files first
                var oldAttachments = _emailAttachmentsTempRepository.Table.Where(x => x.TempEmailId != newAttachment.TempEmailId && x.CreatedDate < todayDate).ToList();
                foreach (var oldAttachment in oldAttachments)
                {
                    var fileName = oldAttachment.Id + Path.GetExtension(oldAttachment.FileName);
                    Tools.DeleteFile(oldAttachment.PhysicalPath + "\\" + fileName);
                    _emailAttachmentsTempRepository.Delete(oldAttachment);
                }

                var existingDocument = _emailAttachmentsTempRepository.Table.Where(x => x.TempEmailId == newAttachment.TempEmailId && x.FileName == newAttachment.FileName).FirstOrDefault();

                if (existingDocument != null)
                {                    
                    throw new Exception(newAttachment.FileName + " already exists at " + newAttachment.PhysicalPath);
                }
                else
                {
                    _emailAttachmentsTempRepository.Insert(newAttachment);
                    return newAttachment.Id;
                }
            }
            catch (Exception ex)
            {
                string logErrorMessage = Tools.GetErrorMessage(ex);
                string errorMessage = "Error saving email attachment TEMP Email Id - " + newAttachment.TempEmailId + " - " + newAttachment.FileName;
                throw new Exception(errorMessage);
                
            }
        }

        public void SaveEmailQuoteReminder(List<int> attachedQuoteIds, int emailId)
        {
            throw new NotImplementedException();
        }

        public ErrorModel SaveEmails(IMFS.Web.Models.DBModel.Emails email)
        {
            throw new NotImplementedException();
        }

        public void SaveEmailTemplate(EmailTemplate emailTemplate)
        {
            throw new NotImplementedException();
        }

        public void SaveEmailTemplateSecondaryCategories(int templateId, List<string> secondaryCategoryIds)
        {
            throw new NotImplementedException();
        }

        public void SaveEmailTemplateVendors(int templateId, List<string> vendorIds)
        {
            throw new NotImplementedException();
        }

        public void SaveRepliedEmails(IMFS.Web.Models.DBModel.Emails email)
        {            
            email.DateTimeCreated = DateTime.Now;
            var rawHtml = email.Body;
            email.Body = Tools.GetPlainTextFromHtml(email.Body, 8000, true);
            _emailsRepository.Insert(email);

            var emailHTML = new EmailsHTMLBody();
            emailHTML.Id = email.Id;
            emailHTML.HTMLBody = rawHtml;
            _emailsHTMLBodyRepository.Insert(emailHTML);
        }

        public List<IMFS.Web.Models.DBModel.Emails> SearchEmails(EmailSearchViewModel emailSearchModel)
        {
            throw new NotImplementedException();
        }

        public void SendReportPhishingEmail(int emailId)
        {
            throw new NotImplementedException();
        }

        public void SetEmailStatusRead(int emailId, string assignee, int? workflowjobid)
        {
            throw new NotImplementedException();
        }

        public void SetNewEmailQueue(int emailId, int newEmailQueue)
        {
            throw new NotImplementedException();
        }        

        public void UpdateExistingEmailStatusFAC(int parentEmailId, int newEmailId)
        {
            throw new NotImplementedException();
        }

        public void UpdateExistingEmailStatusRAC(int parentEmailId, int newEmailId)
        {
            throw new NotImplementedException();
        }

       
    }
}
