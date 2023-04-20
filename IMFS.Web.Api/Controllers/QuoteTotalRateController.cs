using IMFS.BusinessLogic.QuoteTotalRate;
using IMFS.Web.Api.Helper;
using IMFS.Web.Models.QuoteTotalRate;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuoteTotalRateController : BaseController
    {
        private readonly IQuoteTotalRateManager _quoteTotalRateManager;

        public QuoteTotalRateController(IQuoteTotalRateManager quoteTotalRateManager)
        {
            _quoteTotalRateManager = quoteTotalRateManager;
        }

        [Route("GetQuoteTotalRates")]
        [HttpPost]
        public IActionResult Get(QuoteTotalRateModel inputModel)
        {

            var rates = _quoteTotalRateManager.GetRates(inputModel);
            return Ok(rates);
        }

        [Route("SaveQuoteTotalRate")]
        [HttpPost]
        public IActionResult SaveRate(Models.ResponseModel.QuoteTotalRateResponse quoteTotalRateResponse)
        {

            try
            {
                var response = _quoteTotalRateManager.SaveRate(quoteTotalRateResponse);
                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Quote Total Rate information updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }


        [Route("ExportQuoteTotalRate")]
        [HttpPost]
        public IActionResult ExportRate(QuoteTotalRateModel inputModel)
        {
            var response = _quoteTotalRateManager.ExportRates(inputModel);
            IMFSGlobals.CreateDownloadResponse(Response, response);
            return File(response.DownloadFile, "text/csv", response.FileName);
        }


        /// <summary>
        /// This accepts FormData with files from client
        /// </summary>
        /// <returns></returns>
        [Route("UploadQuoteTotalRate")]
        [HttpPost]
        public IActionResult UploadRate()
        {
            try
            {
                var funder = HttpContext.Request.Form["Funder"].ToString();                
                var financeType = HttpContext.Request.Form["FinanceType"].ToString();
                var funderPlan = HttpContext.Request.Form["FunderPlan"].ToString();

                var allFiles = HttpContext.Request.Form.Files;
                for (int i = 0; i < allFiles.Count; i++)
                {
                    var file = allFiles[i];
                    var result = _quoteTotalRateManager.UploadRate(file, funder, financeType, funderPlan);
                    if (result.Count > 0 && result[0].HasError)
                    {
                        return Ok(new { status = "Error", message = JsonConvert.SerializeObject(result) });
                    }
                    else
                    {
                        return Ok(new { status = "Success", message = "Quote Total Rate information updated successfully" });
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
