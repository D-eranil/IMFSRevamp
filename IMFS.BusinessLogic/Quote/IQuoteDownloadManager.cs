using IMFS.Web.Models.Quote;

namespace IMFS.BusinessLogic.Quote
{
    public interface IQuoteDownloadManager
    {
        QuoteDownloadResponse DownloadQuote(QuoteDownloadInput inputModel);
    }
}
