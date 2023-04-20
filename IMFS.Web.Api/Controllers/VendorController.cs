using IMFS.BusinessLogic.Vendor;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendorController : BaseController
    {
        private readonly IVendorManager _vendorManager;

        public VendorController(IVendorManager vendorManager)
        {
            _vendorManager = vendorManager;
        }

        [Route("GetVendors")]
        [HttpGet]
        public IActionResult Get(bool includeInactive = false)
        {
            try
            {
                var vendors = _vendorManager.GetVendor(includeInactive);
                return Ok(vendors);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        [Route("SaveVendor")]
        [HttpPost]
        public IActionResult SaveVendor([FromBody] Models.DBModel.Vendor vendor)
        {
            try
            {
                var response = _vendorManager.SaveVendor(vendor);
                if(response.HasError)
                {
                    return Ok(new { status = "Error", message = response.ErrorMessage });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Vendor information updated successfully" });
                }
               
            }
            catch(Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }      

        
    }
}
