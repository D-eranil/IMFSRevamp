using IMFS.BusinessLogic.QuotePercentRate;
using IMFS.Web.Api.Helper;
using IMFS.Web.Models.QuotePercentRate;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuotePercentRateController : BaseController
    {
        private readonly IQuotePercentRateManager _quotePercentRateManager;

        public QuotePercentRateController(IQuotePercentRateManager quotePercentRateManager)
        {
            _quotePercentRateManager = quotePercentRateManager;
        }

        [Route("GetQuotePercentRates")]
        [HttpPost]
        public IActionResult Get(QuotePercentRateModel inputModel)
        {

            var rates = _quotePercentRateManager.GetRates(inputModel);
            return Ok(rates);
        }

        [Route("SaveQuotePercentRate")]
        [HttpPost]
        public IActionResult SaveRate(Models.ResponseModel.QuotePercentRateResponse quotePercentRateResponse)
        {

            try
            {
                var response = _quotePercentRateManager.SaveRate(quotePercentRateResponse);
                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Quote Percent Rate information updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }


        [Route("ExportQuotePercentRate")]
        [HttpPost]
        public IActionResult ExportRate(QuotePercentRateModel inputModel)
        {
            var response = _quotePercentRateManager.ExportRates(inputModel);
            IMFSGlobals.CreateDownloadResponse(Response, response);
            return File(response.DownloadFile, "text/csv", response.FileName);
        }


        /// <summary>
        /// This accepts FormData with files from client
        /// </summary>
        /// <returns></returns>
        [Route("UploadQuotePercentRate")]
        [HttpPost]
        public IActionResult UploadRate()
        {
            try
            {
                var funder = HttpContext.Request.Form["Funder"].ToString();
                var productType = HttpContext.Request.Form["ProductType"].ToString();
                var financeType = HttpContext.Request.Form["FinanceType"].ToString();
                var funderPlan = HttpContext.Request.Form["FunderPlan"].ToString();

                var allFiles = HttpContext.Request.Form.Files;
                for (int i = 0; i < allFiles.Count; i++)
                {
                    var file = allFiles[i];
                    var result = _quotePercentRateManager.UploadRate(file, funder, productType, financeType, funderPlan);
                    if (result.Count > 0 && result[0].HasError)
                    {
                        return Ok(new { status = "Error", message = JsonConvert.SerializeObject(result) });
                    }
                    else
                    {
                        return Ok(new { status = "Success", message = "Quote Percent Rate information updated successfully" });
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }




            return Ok();
        }
    }
}
