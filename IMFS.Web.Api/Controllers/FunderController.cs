using IMFS.BusinessLogic.Funder;
using Microsoft.AspNetCore.Mvc;
using System;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FunderController : BaseController
    {
        private readonly IFunderManager _funderManager;

        public FunderController(IFunderManager funderManager)
        {
            _funderManager = funderManager;
        }

        [Route("GetFunders")]
        [HttpGet]
        public IActionResult Get(bool includeInactive = false)
        {
            try
            {
                var funders = _funderManager.GetFunder(includeInactive);
                return Ok(funders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }

        }

        [Route("SaveFunder")]
        [HttpPost]
        public IActionResult SaveFunder([FromBody] Models.DBModel.Funder funder)
        {
            try
            {
               var response =  _funderManager.SaveFunder(funder);
                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Funder information updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }
    }
}
