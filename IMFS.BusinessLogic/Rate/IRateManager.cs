using IMFS.Web.Models.Misc;
using IMFS.Web.Models.Rates;
using IMFS.Web.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace IMFS.BusinessLogic.Rate
{
    public interface IRateManager
    {       

        ErrorModel SaveRate(QuoteRateResponse quoteRateResponse);

        List<QuoteRateResponse> GetRates(RateInputModel inputModel);

        DownloadResponse ExportRates(RateInputModel inputModel);

        List<ErrorModel> UploadRate(IFormFile file, string funder, string productType, string financeType);

    }
}
