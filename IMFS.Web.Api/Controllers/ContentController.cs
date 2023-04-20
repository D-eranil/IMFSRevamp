using IMFS.Web.Api.WebModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;


namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class ContentController : BaseController
    {
   		private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration _configuration;
        
        public ContentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("GetSystemConfig")]
        [HttpGet]
        public IActionResult GetSystemConfig()
        {
            try
            {
                var country = _configuration.GetValue<string>("CountryCode");
                var sideNavItemsfilePath = _configuration.GetValue<string>("TopNavItemsFilePath" + country);
                var suideNavItemsContent = System.IO.File.ReadAllText(sideNavItemsfilePath);
                
                return Ok(new
                {
                    navItems = JsonConvert.DeserializeObject<List<TopNavItem>>(suideNavItemsContent)
                });
            }
            catch (Exception ex)
            {
                _logger.Error("GetSystemConfig: " + ex.Message);
                //localLogger.LogError(ex, "ContentController.GetSystemConfig :: Unhandled exception");
                //return Content(HttpStatusCode.InternalServerError, new { status = "Failed", error = ex.ToString() }); //TODO:
            }
            return Ok();
        }
    }
}
