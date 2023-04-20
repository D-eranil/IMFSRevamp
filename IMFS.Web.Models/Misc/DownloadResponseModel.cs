namespace IMFS.Web.Models.Misc
{
    public class DownloadResponse : ErrorModel
    {
        public string FileName { get; set; }
        public byte[] DownloadFile { get; set; }
    }
}
