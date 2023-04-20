using IMFS.Web.Models.Misc;
using IMFS.Web.Models.QuotePercentRate;
using IMFS.Web.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.BusinessLogic.QuotePercentRate
{
    public interface IQuotePercentRateManager
    {
        List<QuotePercentRateResponse> GetRates(QuotePercentRateModel inputModel);

        ErrorModel SaveRate(QuotePercentRateResponse quoteRateResponse);

        DownloadResponse ExportRates(QuotePercentRateModel inputModel);

        List<ErrorModel> UploadRate(IFormFile file, string funder, string productType, string financeType, string funderPlan);
    }
}
