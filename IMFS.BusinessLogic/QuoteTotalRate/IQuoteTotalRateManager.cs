using IMFS.Web.Models.Misc;
using IMFS.Web.Models.QuoteTotalRate;
using IMFS.Web.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.BusinessLogic.QuoteTotalRate
{
    public interface IQuoteTotalRateManager
    {
        List<QuoteTotalRateResponse> GetRates(QuoteTotalRateModel inputModel);

        ErrorModel SaveRate(QuoteTotalRateResponse quoteRateResponse);

        DownloadResponse ExportRates(QuoteTotalRateModel inputModel);

        List<ErrorModel> UploadRate(IFormFile file, string funder, string financeType, string funderPlan);
    }
}
