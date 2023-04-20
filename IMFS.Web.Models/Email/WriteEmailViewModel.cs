using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Email
{
    public class GetWriteEmailModel
    {
        public string EmailMode { get; set; }
        public int? EmailId { get; set; }
        public int? QuoteId { get; set; }
        public int? ApplicationId { get; set; }
        public bool removeAcceptanceLink { get; set; }
    }

    public class WriteEmailViewModel
    {
        public int EmailId { get; set; }
        public string FromAddress { get; set; }
        
        public string ToAddress { get; set; }
        public string EmailMode { get; set; }  //Quote or Application or Contract
        public string CCEmail { get; set; }
        public string BCCEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Guid TempEmailId { get; set; }
        public List<EmailAttachmentModel> Attachments { get; set; }

        public WriteEmailViewModel()
        {   
            this.Attachments = new List<EmailAttachmentModel>();
            this.Subject = "";         
        }
    }
}
