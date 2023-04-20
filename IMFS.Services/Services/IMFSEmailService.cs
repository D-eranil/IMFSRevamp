using IMFS.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IMFS.Services.Services
{
    public class IMFSEmailService : IIMFSEmailService
    {
        public SmtpConfig _smtpConfig { get; set; }        

        public IMFSEmailService(SmtpConfig smtpConfig)
        {
            _smtpConfig = smtpConfig;            
        }

        public LinkedResource CreateLinkedResourceFromImage(string imageName, int imageCounter, string templateFolderPath, out string newImageHTML)
        {
            newImageHTML = "";
            LinkedResource pictureRes = null;
            string extension = imageName.Substring(imageName.IndexOf(".") + 1);
            var newImageName = "image" + imageCounter + "." + extension;
            string newSrc = "cid:" + newImageName;
            string filePath = templateFolderPath + "\\images\\" + imageName;
            if (System.IO.File.Exists(filePath))
            {
                Byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                Stream imageStream = new MemoryStream(imageBytes);
                pictureRes = new LinkedResource(imageStream);
                if (string.IsNullOrEmpty(extension))
                {
                    extension = "jpg";
                }
                pictureRes.ContentType.MediaType = "image/" + extension;
                pictureRes.ContentId = newImageName;
                newImageHTML = string.Format("<img src=\"cid:{0}\" data-filename=\"{1}\" >", newImageName, newImageName);
            }
            return pictureRes;
        }

        public void Send(string from, string to, string cc, string bcc, string subject, string body, List<Attachment> attachments = null, AlternateView altView = null, string messageId = "")
        {
            SendEmail(from, to, cc, bcc, subject, body, attachments, altView, messageId);
        }

        public Task SendMailAsync(string from, string to, string subject, string body, bool isHTMLBody = true)
        {
            SmtpClient client = new SmtpClient();
            var mailMessage = new MailMessage(from, to, subject, body);
            mailMessage.IsBodyHtml = isHTMLBody;
            return client.SendMailAsync(mailMessage);
        }

        private void SendEmail(string from, string to, string cc, string bcc, string subject, string body, List<Attachment> attachments = null, AlternateView altView = null, string messageId = "")
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);

            if (!string.IsNullOrEmpty(messageId))
            {
                mail.Headers.Add("Message-Id", messageId);
            }

            foreach (var toEmail in to.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                mail.To.Add(new MailAddress(toEmail));
            }
            if (!string.IsNullOrEmpty(cc))
            {
                foreach (var ccEmail in cc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.CC.Add(new MailAddress(ccEmail));
                }
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                foreach (var bccEmail in bcc.Split(','))
                    mail.Bcc.Add(new MailAddress(bccEmail));
            }

            mail.Subject = subject;
           
            mail.Body = body;
            mail.IsBodyHtml = true;

            if (attachments != null && attachments.Count > 0)
            {
                foreach (var attachment in attachments)
                {
                    if (attachment != null)
                        mail.Attachments.Add(attachment);
                }
            }

            // for link resources
            if (altView != null)
            {
                mail.AlternateViews.Add(altView);
            }

            using (SmtpClient smtp = new SmtpClient())
            {
                smtp.Timeout = 300000;

                smtp.Host = _smtpConfig.Host;
                smtp.Port = _smtpConfig.Port;
                smtp.UseDefaultCredentials = _smtpConfig.UseDefaultCredentials;

                //smtp.Host = "10.22.103.18";
                //smtp.Port = 25;
                //smtp.UseDefaultCredentials = false;

                smtp.Send(mail);
            }
        }

    }
}
