using IMFS.BusinessLogic.FunderPlan;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FunderPlanController : BaseController
    {
        private readonly IFunderPlanManager _funderPlanManager;

        public FunderPlanController(IFunderPlanManager funderPlanManager)
        {
            _funderPlanManager = funderPlanManager;
        }

        [Route("GetFunderPlans")]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var funderPlans = _funderPlanManager.GetFunderPlan();
                return Ok(funderPlans);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }

        }
    }
}
