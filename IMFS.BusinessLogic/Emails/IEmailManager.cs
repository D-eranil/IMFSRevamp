using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Email;
using IMFS.Web.Models.Enums;
using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace IMFS.BusinessLogic.Emails
{
    public interface IEmailManager
    {
        //IEnumerable<EmailRulesModel> GetEmailRules();

        //void SaveEmailRule(EmailRulesModel emailRule);

        //void DeleteEmailRule(int id); 
        List<EmailHistory> GetQuoteEmailHistory(int quoteId);


        List<EmailHistory> GetApplicationEmailHistory(int applicationId);

        void CreateEmailXref(int emailId, int quoteId = 0, int applicationId = 0, int contractId = 0);

        List<EmailListViewModel> GetNewEmails(int queueId);

        void ChangeEmailStatus(IMFSEnums.EmailStatus newStatus, int emailId);

        EmailModel GetEmailDetailsModel(int? emailId, bool loadHtml = false, bool loadQueueDetails = false, string webApiUrl = "");

        EmailBodyModel GetEmailHTMLBody(int emailId);

        EmailAttachment GetEmailAttachment(int emailId, string imageName, string contentId);

        EmailAttachment GetEmailAttachment(int attachmentId);

        vw_Emails GetEmailDetails(int emailId);

        void ReportPhishingEmail(int emailId);

        void SendReportPhishingEmail(int emailId);

        List<vw_Emails> GetEmailInfos(List<int> emailIds);

        void AddNotes(int emailId, string newNote);

        //List<int> GetWorkflowEmailJobIds(int emailId);

        //List<EmailJobSC> GetWorkflowEmailJobs(List<int> emailIds);

        int SaveEmailAttachmentTemp(EmailAttachmentModel attachment);

        List<EmailAttachmentsTemp> GetEmailAttachmentsTemp(Guid tempEmailId);

        EmailAttachmentsTemp GetEmailAttachmentTemp(int attachmentId);

        void SaveRepliedEmails(IMFS.Web.Models.DBModel.Emails email);

        void DeleteEmailAttachmentTemp(int emailAttachmentTempId);

        void UpdateExistingEmailStatusRAC(int parentEmailId, int newEmailId);

        void UpdateExistingEmailStatusFAC(int parentEmailId, int newEmailId);

        ErrorModel SaveAttachment(EmailAttachment emailattachment);

        void AddNotes(int emailId, string currentUserName, string newNote);

        List<LinkedResource> ConverBase64StringsToResources(int emailId, string emailHTML, out string newEmailHTML, out Dictionary<string, string> linkedResourcesCollection);

        List<LookupModel> GetEmailTemplatesLookup(int? id, List<int> vendorIds, List<int> secondaryCategoriesId, bool standaloneEmail = false);

        List<EmailAttachment> GetEmailAttachments(int emailId);

        EmailTemplate GetEmailTemplateDetails(int templateId);

        //List<List<WorkflowEmails>> getWorkflowEmailByIds(List<int> includeEmailIds);

        void MarkAllEmailsAsRead(int jobId);

        void UpdateEmails(vw_Emails emailDetails);

        //void DeleteWFEmails(int emailId);

        void SetEmailStatusRead(int emailId, string assignee, int? workflowjobid);

        void SetNewEmailQueue(int emailId, int newEmailQueue);

        List<IMFS.Web.Models.DBModel.Emails> SearchEmails(EmailSearchViewModel emailSearchModel);

        List<EmailTemplateModel> GetEmailTemplateList();

        void DeleteEmailTemplate(int id);

        void SaveEmailTemplate(EmailTemplate emailTemplate);

        void SaveEmailTemplateSecondaryCategories(int templateId, List<string> secondaryCategoryIds);

        void SaveEmailTemplateVendors(int templateId, List<string> vendorIds);

       // List<int> UploadEmailAttachments(int iJobID, string physicalPath, HttpFileCollection Files, AspNetUser currentUser);

        vw_Emails GetEmailByInternetMessageId(string internetMessageId, string subject, DateTime? receivedDateTime);

        ErrorModel SaveEmails(IMFS.Web.Models.DBModel.Emails email);

        // List<EmailRule> GetEmailRuleListByEmailQueueId(int emailQueueId);

        //List<List<AnnuityEmails>> getAnnuityEmailByIds(List<int> includeEmailIds);

        //RelatedQuoteInformation GetRelatedJobQuotes(int? jobId);

        //RelatedQuoteInformation GetRelatedAnnuityQuotes(int? annuityId);

        List<EmailAttachmentModel> AttachQuoteDownload(AttachQuoteModel inputModel, string emailAttachmentsFolder);

        void SaveEmailQuoteReminder(List<int> attachedQuoteIds, int emailId);

        //void SetAnnuityEmailStatusRead(int emailId, string assignee, int? annuityId);

        //void MarkAllAnnuityEmailsAsRead(int aaId);
    }

}
