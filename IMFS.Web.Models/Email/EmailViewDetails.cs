using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Email
{
    public partial class EmailModel
    {
        public EmailModel()
        {
            RelatedJobIds = new List<int>();
            RelatedEmailIds = new List<int>();
            //FavoriteSCList = new List<NewJobCategory>();
        }
        public int Id { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string CCEmail { get; set; }
        public string Subject { get; set; }
        public string ReplyTo { get; set; }
        public string Body { get; set; }
        public string BodyType { get; set; }
        public string Importance { get; set; }
        public string InternetMessageId { get; set; }
        public bool? IsNew { get; set; }
        public string EmailType { get; set; }
        public DateTime? DateTimeCreated { get; set; }
        public string Table { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> DateTimeReceived { get; set; }
        public string EmailUniqueID { get; set; }
        public string BCCEmail { get; set; }
        public List<int> RelatedEmailIds { get; set; }
        public List<int> RelatedJobIds { get; set; }

        public int NextEmailId { get; set; }
        public int PreviousEmailId { get; set; }
        public int? ParentEmailId { get; set; }
        public EmailQueueModel QueueDetails { get; set; }
        public string HtmlBody { get; set; }
        public string Notes { get; set; }
        public bool HasOtherEmailQueues { get; set; }

        public List<LookupModel> EmailAttachments { get; set; }
        public bool ShowRemoveButton { get; set; }
        //public List<NewJobCategory> FavoriteSCList { get; set; }
    }
    public class EmailAttachmentModel
    {
        public int Id { get; set; }
        public string ContentId { get; set; }
        public string FileName { get; set; }
        public Guid? TempEmailId { get; set; }
        public string PhysicalPath { get; set; }
        public string UploadedBy { get; set; }
        public double? FileSize { get; set; }
        public string QuoteIdAttached { get; set; }

    }
    public class EmailQueueModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public bool? IsActive { get; set; }
        public bool HeavyQueue { get; set; }
        public string Status { get; set; }
        public bool? CheckResellerDomain { get; set; }
        public int? SecondaryCategoryId { get; set; }
        public DateTime? LastRunDate { get; set; }
        public List<LookupModel> AssignedUsers { get; set; }
        public List<LookupModel> AssignedDivisions { get; set; }
        public List<LookupModel> Aliases { get; set; }
        public LookupModel SelectedSecondaryCategory { get; set; }

        public EmailQueueModel()
        {
            AssignedUsers = new List<LookupModel>();
            AssignedDivisions = new List<LookupModel>();
            Aliases = new List<LookupModel>();
        }
    }

    public class EmailTemplateModel
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public bool? DisplayInStandalone { get; set; }
        public string Body { get; set; }
        public List<LookupModel> AssignedVendors { get; set; }
        public List<LookupModel> AssignedSecondaryCategories { get; set; }
        public EmailTemplateModel()
        {
            AssignedVendors = new List<LookupModel>();
            AssignedSecondaryCategories = new List<LookupModel>();
        }
    }
}
