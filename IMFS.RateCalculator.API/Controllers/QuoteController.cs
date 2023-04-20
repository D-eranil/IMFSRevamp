using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using IMFS.Web.Models.QuoteRateCalculation;
using IMFS.BusinessLogic.Quote;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace IMFS.RateCalculator.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuoteController : Controller
    {
        private readonly ILogger<QuoteController> _logger;
        private readonly IQuoteManager _quoteManager;

        private readonly IConfiguration _configuration;       

        public QuoteController(IQuoteManager quoteManager, IConfiguration configuration)
        {
            _quoteManager = quoteManager;
            _configuration = configuration;
        }


        [Route("GetQuote")]
        [HttpGet]
        public IActionResult Get(bool includeInactive = false)
        {
            try
            {
                string message = "test handler";
                return Ok(message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }


        [Route("CalculateRate")]
        [HttpPost]
        public IActionResult CalculateRate(QuoteRateCalculationModel quoteModel)
        {
            try
            {
                var defaultFinanceType = _configuration.GetValue<string>("DefaultFinanceType");
                var defaultFrequency = _configuration.GetValue<string>("DefaultFrequency");
                var defaultDuration = _configuration.GetValue<string>("DefaultDuration");

                var response = _quoteManager.CalculateRate(quoteModel, defaultFrequency, defaultDuration, defaultFinanceType);
                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {                    
                    return Ok(new { status = "Success", message = "Quote Rate Calculate Successful", financeDetails = response.QuoteResponse });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

    }
}
