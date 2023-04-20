using IMFS.BusinessLogic.Quote;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ORPCustomerController : BaseController
    {
        private readonly IQuoteManager _quoteManager;
        private IConfiguration _configuration;

        public ORPCustomerController(IQuoteManager quoteManager, IConfiguration configuration)
        {
            _quoteManager = quoteManager;  
            _configuration = configuration;
        }


        [Route("GetCustomers")]
        [HttpGet]
        public IActionResult Get(string customerName)
        {
            try
            {                
                var response = _quoteManager.GetCustomer(customerName);
                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Get Customer Successful", customerDetails = response.CustomerResponse });
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        
        [Route("GetCustomerContacts")]
        [HttpGet]
        public IActionResult GetCustomerContacts(string customerNumber)
        {
            try
            {
                string auORPConnnectionString = _configuration.GetConnectionString("AUORPDataContext");
                var response = _quoteManager.GetCustomerContacts(customerNumber, auORPConnnectionString);
                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Get Contact Successful", customerDetails = response.ContactResponse });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }
    }
}
