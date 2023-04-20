using IMFS.Services.Models;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IMFS.Services.Services
{
    public interface IIMFSEmailService
    {
        SmtpConfig _smtpConfig { get; set; }
        void Send(string from, string to, string cc, string bcc, string subject, string body, List<Attachment> attachments = null, AlternateView altView = null, string messageId = "");
        Task SendMailAsync(string from, string to, string subject, string body, bool isHTMLBody = true);
        LinkedResource CreateLinkedResourceFromImage(string imageName, int imageCounter, string templateFolderPath, out string newImageHTML);
    }
}

