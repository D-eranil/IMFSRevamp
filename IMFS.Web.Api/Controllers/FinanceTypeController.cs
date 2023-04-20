using IMFS.BusinessLogic.FinanceType;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FinanceTypeController : BaseController
    {
        private readonly IFinanceTypeManager _financeTypeManager;

        public FinanceTypeController(IFinanceTypeManager financeTypeManager)
        {
            _financeTypeManager = financeTypeManager;
        }

        [Route("GetFinanceTypes")]
        [HttpGet]
        public IActionResult Get(bool includeInactive = false)
        {
            try
            {
                var financeTypes = _financeTypeManager.GetFinanceType(includeInactive);
                return Ok(financeTypes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }

        }

        [Route("SaveFinanceType")]
        [HttpPost]
        public IActionResult SaveFinanceType([FromBody] Models.DBModel.FinanceType financeType)
        {
            try
            {
                var response = _financeTypeManager.SaveFinanceType(financeType);
                if (response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Finance Type information updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

    }
    }
