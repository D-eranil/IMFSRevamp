using IMFS.BusinessLogic.FinanceProductType;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FinanceProductTypeController : BaseController
    {
        private readonly IFinanceProductTypeManager _financeProductTypeManager;

        public FinanceProductTypeController(IFinanceProductTypeManager financeProductTypeManager)
        {
            _financeProductTypeManager = financeProductTypeManager;
        }

        [Route("GetFinanceProductTypes")]
        [HttpGet]
        public IActionResult Get(bool includeInactive = false)
        {
            try
            {
                var financeProductTypes = _financeProductTypeManager.GetFinanceProductType(includeInactive);
                return Ok(financeProductTypes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }

        }

        [Route("SaveFinanceProductType")]
        [HttpPost]
        public IActionResult SaveFinanceProductType([FromBody] Models.DBModel.FinanceProductType financeProductType)
        {
            try
            {
                var response = _financeProductTypeManager.SaveFinanceProductType(financeProductType);
                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Finance Product Type information updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }
    }
}
