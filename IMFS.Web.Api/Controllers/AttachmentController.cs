using IMFS.BusinessLogic.Emails;
using IMFS.Web.Models.Email;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using IMFS.Web.Models.Enums;
using IMFS.Services.Services;
using IMFS.Web.Models.DBModel;
using IMFS.Core;
using IMFS.Web.Api.Helper;
using Microsoft.Extensions.Configuration;
using IMFS.Web.Models.Misc;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AttachmentController : BaseController
    {
        private IEmailManager _emailManager;
        private IIMFSEmailService _imfsEmailService;
        private readonly IConfiguration _configuration;

        public AttachmentController(IEmailManager emailManager, IIMFSEmailService imfsEmailService, IConfiguration configuration)
        {
            _emailManager = emailManager;
            _imfsEmailService = imfsEmailService;
            _configuration = configuration;
        }

        [Route("DownloadEmailAttachment")]
        [HttpGet]
        public IActionResult DownloadEmailAttachment(int attachmentId)
        {
            try
            {
                DownloadResponse result = new DownloadResponse();
                var imageAttachment = _emailManager.GetEmailAttachment(attachmentId);
                if (imageAttachment != null)
                {
                    string fileExtension = Path.GetExtension(imageAttachment.FileName);
                    string temporaryFileName = imageAttachment.Id.ToString() + fileExtension;
                    var filePath = imageAttachment.PhysicalPath + "\\" + temporaryFileName;
                    if (System.IO.File.Exists(filePath))
                    {
                        var dataBytes = System.IO.File.ReadAllBytes(filePath);
                        result.DownloadFile = dataBytes;
                        result.FileName = imageAttachment.FileName;
                        IMFSGlobals.CreateDownloadResponse(Response, result);
                        return File(result.DownloadFile, MimeTypes.GetMimeType(result.FileName), result.FileName);
                    }
                    else
                    {
                        //get attachment from archived folder
                        var folderName = imageAttachment.PhysicalPath.Split('\\').Last();
                        string filePathArchived = _configuration.GetValue<string>("EmailAttachmentArchivedRootFolder")  + "\\" + folderName + "\\" + temporaryFileName;
                        var dataBytes = System.IO.File.ReadAllBytes(filePathArchived);
                        result.DownloadFile = dataBytes;
                        result.FileName = imageAttachment.FileName;
                        IMFSGlobals.CreateDownloadResponse(Response, result);
                        return File(result.DownloadFile, MimeTypes.GetMimeType(result.FileName), result.FileName);
                    }
                }

                return BadRequest(new { status = "Failed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        [Route("DownloadEmailTempAttachment")]
        [HttpGet]
        public IActionResult DownloadEmailTempAttachment(int attachmentId)
        {
            try
            {
                DownloadResponse result = new DownloadResponse();
                var imageAttachment = _emailManager.GetEmailAttachmentTemp(attachmentId);
                if (imageAttachment != null)
                {
                    string fileExtension = Path.GetExtension(imageAttachment.FileName);
                    string temporaryFileName = imageAttachment.Id.ToString() + fileExtension;
                    var filePath = imageAttachment.PhysicalPath + "\\" + temporaryFileName;
                    if (System.IO.File.Exists(filePath))
                    {
                        var dataBytes = System.IO.File.ReadAllBytes(filePath);
                        result.DownloadFile = dataBytes;
                        result.FileName = imageAttachment.FileName;
                        IMFSGlobals.CreateDownloadResponse(Response, result);
                        return File(result.DownloadFile, MimeTypes.GetMimeType(result.FileName), result.FileName);
                    }
                }
                return BadRequest(new { status = "Failed"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }
        
        [Route("DownloadImageAttachment")]
        [HttpGet]
        public IActionResult DownloadImageAttachment(int attachmentId)
        {
            try
            {
                DownloadResponse result = new DownloadResponse();
                var imageAttachment = _emailManager.GetEmailAttachment(attachmentId);
                if (imageAttachment != null)
                {
                    string fileExtension = Path.GetExtension(imageAttachment.FileName);
                    string temporaryFileName = imageAttachment.Id.ToString() + fileExtension;
                    var filePath = imageAttachment.PhysicalPath + "\\" + temporaryFileName;
                    if (System.IO.File.Exists(filePath))
                    {
                        var dataBytes = System.IO.File.ReadAllBytes(filePath);
                        result.DownloadFile = dataBytes;
                        result.FileName = imageAttachment.FileName;
                        IMFSGlobals.CreateDownloadResponse(Response, result);
                        return File(result.DownloadFile, MimeTypes.GetMimeType(result.FileName), result.FileName);
                        //return File(result.DownloadFile, "image/png", result.FileName);
                    }
                }
                return BadRequest(new { status = "Failed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        [Route("SaveEmailAttachmentTempFile")]
        [HttpPost]
        public IActionResult SaveEmailAttachmentTempFile()
        {
            try
            {
                Guid tempEmailGuid = Guid.Empty;
                List<EmailAttachmentModel> successAttachments = new List<EmailAttachmentModel>();
                var Files = HttpContext.Request.Form.Files;
                //var userId = IdentityHelper.GetClaimValue(User, ClaimTypes.NameIdentifier);
                string physicalPath = _configuration.GetValue<string>("EmailAttachmentTempRootDirectory") + "\\" + "LoggedInUser";
                // create own folder for each user id if it does not exist
                Tools.CreateFolder(_configuration.GetValue<string>("EmailAttachmentTempRootDirectory"), "LoggedInUser");

                string tempEmailId = HttpContext.Request.Form["tempEmailId"].ToString(); 
                if (string.IsNullOrEmpty(tempEmailId) || !Guid.TryParse(tempEmailId, out tempEmailGuid))
                {
                    return BadRequest(new { status = "Failed", error = "Invalid temporary email id" });                    
                }

                for (int index = 0; index < Files.Count; index++)
                {
                    var file = Files[index];
                    if (file == null) continue;

                    var newAttachmentTemp = new EmailAttachmentModel();
                    newAttachmentTemp.TempEmailId = tempEmailGuid;
                    newAttachmentTemp.FileName = file.FileName;
                    newAttachmentTemp.PhysicalPath = physicalPath;
                    newAttachmentTemp.UploadedBy = "LoggedInUser";
                    newAttachmentTemp.FileSize = file.Length;
                    int? newAttachmentTempId = _emailManager.SaveEmailAttachmentTemp(newAttachmentTemp);
                    if (newAttachmentTempId.HasValue)
                    {
                        newAttachmentTemp.Id = newAttachmentTempId.Value;
                        successAttachments.Add(newAttachmentTemp);
                        string temporaryFileName = newAttachmentTempId.Value.ToString() + Path.GetExtension(newAttachmentTemp.FileName);
                        using (Stream fileStream = new FileStream(Path.Combine(physicalPath, temporaryFileName), FileMode.Create))
                        {
                            file.CopyToAsync(fileStream);
                        }

                        //file.SaveAs(Path.Combine(physicalPath, temporaryFileName));
                    }
                }

                return Ok(successAttachments);
                //return Content(HttpStatusCode.OK, new { data = successAttachments });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        [Route("RemoveEmailAttachmentTempFile")]
        [HttpGet]
        public IActionResult RemoveEmailAttachmentTempFile(int? emailAttachmentTempId)
        {
            try
            {
                if (!emailAttachmentTempId.HasValue)
                {
                    return BadRequest(new { status = "Failed", error = "Invalid temporary email attachment id" });                    
                }
                _emailManager.DeleteEmailAttachmentTemp(emailAttachmentTempId.Value);

                return Ok(new { status = "Success", message = "File removed successfully" });
                
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        [HttpGet]
        [Route("GetEmailAttachments")]
        public IActionResult GetEmailAttachments(int? emailId, string tempEmailId)
        {
            try
            {
                if (emailId.HasValue && emailId > 0)
                {
                    var emailAttachments = _emailManager.GetEmailAttachments(emailId.Value);
                    var attachmentModel = emailAttachments.Select(x => new { fileName = x.FileName, Id = x.Id });
                    return Ok(attachmentModel);
                }
                else if (!string.IsNullOrEmpty(tempEmailId))
                {
                    Guid tempEmailIdGuid;
                    Guid.TryParse(tempEmailId, out tempEmailIdGuid);
                    var tempEmailAttachments = _emailManager.GetEmailAttachmentsTemp(tempEmailIdGuid);
                    var attachmentModel = tempEmailAttachments.Select(x => new { fileName = x.FileName, Id = x.Id });
                    return Ok(attachmentModel);
                }

                return Ok(new { status = "Success", message = "Files retrieved successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }
    }
}
