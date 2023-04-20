using IMFS.BusinessLogic.Rate;
using IMFS.Web.Api.Helper;
using IMFS.Web.Models.Rates;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RateController : BaseController
    {

        private readonly IRateManager _rateManager;

        public RateController(IRateManager rateManager)
        {
            _rateManager = rateManager;
        }

        [Route("GetRates")]
        [HttpPost]
        public IActionResult Get(RateInputModel inputModel)
        {
            
            var rates = _rateManager.GetRates(inputModel);
            return Ok(rates);
        }

        [Route("ExportRate")]
        [HttpPost]
        public IActionResult ExportRate(RateInputModel inputModel)
        {
            var response = _rateManager.ExportRates(inputModel);
            IMFSGlobals.CreateDownloadResponse(Response, response);
            return File(response.DownloadFile, "text/csv", response.FileName);
        }


        [Route("SaveRate")]
        [HttpPost]
        public IActionResult SaveRate(Models.ResponseModel.QuoteRateResponse quoteRateResponse)
        {         

            try
            {
                var response = _rateManager.SaveRate(quoteRateResponse);
                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Quote Rate information updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        /// <summary>
        /// This accepts FormData with files from client
        /// </summary>
        /// <returns></returns>
        [Route("UploadRate")]
        [HttpPost]
        public IActionResult UploadRate()
        {
            try
            {
                var funder = HttpContext.Request.Form["Funder"].ToString();
                var productType = HttpContext.Request.Form["ProductType"].ToString();
                var financeType = HttpContext.Request.Form["FinanceType"].ToString();

                var allFiles = HttpContext.Request.Form.Files;
                for (int i = 0; i < allFiles.Count; i++)
                {
                    var file = allFiles[i];
                    var result = _rateManager.UploadRate(file, funder, productType, financeType);
                    if (result.Count > 0 && result[0].HasError)
                    {
                        return Ok(new { status = "Error", message = JsonConvert.SerializeObject(result) });
                    }
                    else
                    {
                        return Ok(new { status = "Success", message = "Funder information updated successfully" });
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
