using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Email
{
    public partial class EmailRulesModel
    {
        public EmailRulesModel()
        { }

        public int Id { get; set; }
        public string Description { get; set; }
        public string FromAddress { get; set; }
        public string FromDomain { get; set; }
        public string ToAddress { get; set; }
        public string EmailQueue { get; set; }
        public string EmailRule { get; set; }
        public bool? AutoComplete { get; set; }
        public bool? AutoCreate { get; set; }
        public bool? SubjectRegex { get; set; }
        public string RegularExpressionDescription { get; set; }
        public int? EmailQueueId { get; set; }
        public int? VendorId { get; set; }
        public string ActionType { get; set; }
        public string SecondaryCategoryId { get; set; }
        public string CreatedBy { get; set; }
        public string Subject { get; set; }
        public int? Priority { get; set; }
    }

    public partial class EmailListViewModel
    {
        public int Id { get; set; }
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public string VendorName { get; set; }
        public DateTime? DateTimeReceived { get; set; }
        public bool HasOtherEmailQueues { get; set; }
        public List<EmailJobSC> WorkflowJobList { get; set; }

        public EmailListViewModel()
        {
            this.WorkflowJobList = new List<EmailJobSC>();
        }
    }

    public class EmailJobSC
    {
        public int EmailId { get; set; }
        public int JobId { get; set; }
        public string SecondaryCategoryName { get; set; }
        public string VendorName { get; set; }
    }

    
    public class WorkflowEmails
    {
        public int EmailID { get; set; }
        public int JobId { get; set; }
        public string EmailType { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public DateTime DateTimeReceived { get; set; }
        public int? ParentEmailId { get; set; }
        public bool? IsNew { get; set; }
        public bool? HasEmailAttachments { get; set; }
        public string AttachmentNames { get; set; }

        public WorkflowEmails()
        {
            EmailType = string.Empty;
            FromAddress = string.Empty;
            ToAddress = string.Empty;
            Subject = string.Empty;
        }
    }

    public partial class SendEmailViewModel
    {
        public int EmailId { get; set; }
        public string QuoteId { get; set; }
        public string ApplicationId { get; set; }
        public Guid TempEmailId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string CCEmail { get; set; }
        public string BCCEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ParentEmailId { get; set; }
        public string JobId { get; set; }
        public string AnnuityId { get; set; }
        public string ParentEmailStatus { get; set; }
        public bool StandaloneEmail { get; set; }
        public bool KeepJobOpen { get; set; }
        public string EmailMode { get; set; }
        public List<int> QuotesAttached { get; set; }
        public string BlockSOId { get; set; }
        public int? BlockSOLineId { get; set; }

        public SendEmailViewModel()
        {
            //LinkResources = new Dictionary<string, string>();
            StandaloneEmail = false;
            KeepJobOpen = false;
        }
    }

    public class EmailSearchViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EmailID { get; set; }
        public string Status { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string EmailType { get; set; }
        public string AttachmentFileName { get; set; }

        public List<string> EmailQueues { get; set; }

        public EmailSearchViewModel()
        {
            EmailQueues = new List<string>();
        }
    }

    public class EmailBodyModel
    {
        public string HtmlBody { get; set; }
        public List<string> ImageContentIds { get; set; }
        public List<string> ImageNames { get; set; }
    }

    public class AnnuityEmails
    {
        public int EmailID { get; set; }
        public int AnnuityAgreementId { get; set; }
        public string EmailType { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public DateTime DateTimeReceived { get; set; }
        public int? ParentEmailId { get; set; }
        public bool? IsNew { get; set; }
        public bool? HasEmailAttachments { get; set; }
        public string AttachmentNames { get; set; }

        public AnnuityEmails()
        {
            EmailType = string.Empty;
            FromAddress = string.Empty;
            ToAddress = string.Empty;
            Subject = string.Empty;
        }
    }

    public class AttachEmailToJobModel
    {
        public int JobId { get; set; }
        public List<int> EmailIds { get; set; }
        public string JobStatusAction { get; set; } // closed or reopen
        public string NewAssigneeId { get; set; }
    }

    public class AttachEmailToAnnuityModel
    {
        public int AnnuityId { get; set; }
        public List<int> EmailIds { get; set; }
        public string NewAssigneeId { get; set; }
    }

    public class RelatedQuoteInformation
    {
        public int Id { get; set; }
        public List<string> CustomerNumbers { get; set; }
        public List<string> CustomerNames { get; set; }
        public List<RelatedQuote> Quotes { get; set; }
    }

    public class RelatedQuote
    {
        public string QuoteId { get; set; }
        public int Version { get; set; }
        public string QuoteName { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? QuoteTotal { get; set; }
        public string QuoteStatus { get; set; }
        public string QuoteCustomerNumber { get; set; }
        public string QuoteCustomerName { get; set; }
    }

    public class AttachQuoteModel
    {
        public string TempEmailId { get; set; }
        public List<string> QuotesList { get; set; }
    }

    public class ReportPhishEmailTemplateModel
    {
        public string EmailId { get; set; }
        public string EmailQueueName { get; set; }
        public string EmailQueueAddress { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string DateReceived { get; set; }
        public string Subject { get; set; }
        public string EmailLink { get; set; }
        public string O365Link { get; set; }
        public string SubmittedBy { get; set; }
    }
}
