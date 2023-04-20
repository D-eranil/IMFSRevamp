using IMFS.DataAccess.Repository;
using IMFS.Web.Models.DBModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class OptionsController : BaseController
    {
        
        private readonly IRepository<Types> _typesRepository;
        private readonly IRepository<Categories> _categoriesRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Status> _statusRepository;



        public OptionsController(
           IRepository<Types> typesRepository,
           IRepository<Categories> categoriesRepository,
           IRepository<Product> productRepository,
           IRepository<Status> statusRepository
           )
        {           
            _typesRepository = typesRepository;
            _categoriesRepository = categoriesRepository;
            _productRepository = productRepository;
            _statusRepository = statusRepository;
        }

        [Route("GetTypes")]        
        [HttpGet]
        public IActionResult GetTypes(bool includeInactive = false)
        {
            try
            {
                var result = _typesRepository.Table.ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed to get Types", error = ex.ToString() });
            }
        }

        [Route("GetCategories")]        
        [HttpGet]
        public IActionResult GetCategories(bool includeInactive = false)
        {
            try
            {
                var result = _categoriesRepository.Table.ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed to get Categories", error = ex.ToString() });
            }
        }

        [Route("GetStatus")]        
        [HttpGet]
        public IActionResult GetStatus(bool includeInactive = false, bool quoteOnly = false, bool applicationOnly = false)
        {
            try
            {
                List<Status> result = new List<Status>();
                if (quoteOnly)
                {
                    result = _statusRepository.Table.Where(s => s.IsQuote == quoteOnly).ToList();                    
                }
                if (applicationOnly)
                {
                    result = _statusRepository.Table.Where(s => s.IsApplication == applicationOnly).ToList();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed to get Status", error = ex.ToString() });
            }
        }

    }
}
