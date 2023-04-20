using IMFS.BusinessLogic.FinanceProductType;
using IMFS.BusinessLogic.FinanceType;
using IMFS.BusinessLogic.Funder;
using IMFS.BusinessLogic.Quote;
using IMFS.DataAccess.Repository;
using IMFS.Web.Models.DBModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMFS.RateCalculator.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OptionsController : Controller
    {
        private readonly IFunderManager _funderManager;
        private readonly IFinanceTypeManager _financeTypeManager;
        private readonly IFinanceProductTypeManager _financeProductTypeManager;
        private readonly IRepository<Types> _typesRepository;
        private readonly IRepository<Categories> _categoriesRepository;
        private readonly IRepository<Product> _productRepository;

        public OptionsController(IFunderManager funderManager,
            IFinanceTypeManager financeTypeManager,
            IFinanceProductTypeManager financeProductTypeManager,
            IRepository<Types> typesRepository,
            IRepository<Categories> categoriesRepository,
            IRepository<Product> productRepository
            )
        {
            _funderManager = funderManager;
            _financeTypeManager = financeTypeManager;
            _financeProductTypeManager = financeProductTypeManager;
            _typesRepository = typesRepository;
            _categoriesRepository = categoriesRepository;
            _productRepository = productRepository;
        }

        [Route("GetFunders")]
        [HttpGet]        
        public IActionResult GetFunders(bool includeInactive = false)//[FromHeader] string countryCode
        {
            try
            {
                var result = _funderManager.GetFunder();
                return Ok(result);                
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed to get Funders", error = ex.ToString() });
            }
        }

        [Route("GetFinanceTypes")]
        [HttpGet]
        public IActionResult GetFinanceTypes(bool includeInactive = false)
        {
            try
            {
                var result = _financeTypeManager.GetFinanceType();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed to get Finance Types", error = ex.ToString() });
            }
        }

        [Route("GetFinanceProductTypes")]
        [HttpGet]
        public IActionResult GetFinanceProductTypes(bool includeInactive = false)
        {
            try
            {
                var result = _financeProductTypeManager.GetFinanceProductType();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed to get Finance Product Types", error = ex.ToString() });
            }
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

        [Route("GetProducts")]
        [HttpGet]
        public IActionResult GetProducts(bool includeInactive = false)
        {
            try
            {
                var result = _productRepository.Table.ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed to get Categories", error = ex.ToString() });
            }
        }

    }
}
