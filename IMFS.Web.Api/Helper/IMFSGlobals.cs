using IMFS.Web.Models.Misc;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace IMFS.Web.Api.Helper
{
    public class IMFSGlobals
    {
        public static void CreateDownloadResponse(HttpResponse response, DownloadResponse fileDownload)
        {
            //adding bytes to memory stream
            //response.Body = new MemoryStream(fileDownload.DownloadFile);

            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileDownload.FileName,
                Inline = false  // false = prompt the user for downloading;  true = browser to try to show the file inline
            };
            response.Headers.Add("Content-Disposition", cd.ToString());
            response.ContentType = "application/octet-stream";
            response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        }
    }
}
