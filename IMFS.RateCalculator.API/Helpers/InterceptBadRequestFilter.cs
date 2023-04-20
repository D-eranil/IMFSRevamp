using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Web.Http.Controllers;


namespace IMFS.RateCalculator.API.Helpers
{
    public class InterceptBadRequestFilter : IActionFilter
    {

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var countryCode = context.HttpContext.Request.Headers.SingleOrDefault(x => x.Key.ToLower() == "countrycode").Value.ToString().ToLower();

            if (string.IsNullOrEmpty(countryCode))
            {
                string message = "CountryCode is missing in header.";
                byte[] bytes = Encoding.Unicode.GetBytes(message);
                context.HttpContext.Response.Body.Write(bytes, 0, bytes.Length);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                context.Result = new BadRequestObjectResult("");
            }
            else if (countryCode != "au" && countryCode != "nz")
            {
                string message = "Invalid CountryCode. Only AU and NZ are valid";
                byte[] bytes = Encoding.Unicode.GetBytes(message);                
                context.HttpContext.Response.Body.Write(bytes, 0, bytes.Length);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                context.Result = new BadRequestObjectResult("");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new System.NotImplementedException();
        }

    }
}
