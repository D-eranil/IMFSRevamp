using IMFS.Web.Models.Misc;

namespace IMFS.Web.Models.Quote
{
    public class QuoteDownloadInput
    {
        public int QuoteId { get; set; }
        public string DownloadMode { get; set; }  // Excel, Proposal
    }

    public class QuoteDownloadResponse : DownloadResponse
    {
        public string QuoteName { get; set; }
        public int QuoteNumber { get; set; }
        public int Version { get; set; }
    }
}
