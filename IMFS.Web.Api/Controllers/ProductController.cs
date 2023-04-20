using IMFS.BusinessLogic.Product;
using IMFS.Services.Models;
using IMFS.Web.Models.Product;
using Microsoft.AspNetCore.Mvc;
using System;
//using System.Web.Http;
using System.Collections.Generic;
using System.Linq;

namespace IMFS.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]   
    public class ProductController : BaseController
    {
        private readonly IProductManager _productManager;
        //private readonly ILogManager _logger;

        public ProductController(IProductManager productManager)
        {
            _productManager = productManager;
        }


        //[Route("GetProducts")]
        //[HttpGet]
        //public IActionResult GetProducts()
        //{
        //        return Ok();
            
        //}


        [Route("GetProducts")]
        [HttpGet]
        public IActionResult GetProducts([System.Web.Http.FromUri] List<string> sku, bool isVPN = false)
        {
            List<ProductAutocomplete> res = new List<ProductAutocomplete>();
            if (sku == null || sku.Count == 0) return Ok("");
            try
            {
                var products = _productManager.GetProducts(sku, isVPN);

                res = products.Select(item => new ProductAutocomplete
                {
                    ProductID = item.ProductID.ToString(),
                    SKU = item.InternalSKUID,
                    VPN = item.VendorSKUID,
                    PurchasingBlock = item.PurchasingBlock,
                    SalesBlock = item.SalesBlock,
                    Description = item.ProductDescription,
                    DisplayLabel = item.InternalSKUID + " (" + item.VendorSKUID + " - " + item.ProductDescription + ")"
                }).ToList();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

        [Route("GetProductDetails")]
        [HttpPost]
        public IActionResult GetProductDetails(GetProductDetails inputModel)
        {
            try
            {
                List<ProductEnquiry> skus = new List<ProductEnquiry>();
                if (inputModel.imskus != null && inputModel.imskus.Count > 0)
                {
                    inputModel.imskus.ForEach(sku => skus.Add(new ProductEnquiry() { StockCode = sku, Quantity = 1 }));
                }
                List<ProductEnquiry> vpns = new List<ProductEnquiry>();
                if (inputModel.vpns != null && inputModel.vpns.Count > 0)
                {
                    inputModel.vpns.ForEach(vpn => vpns.Add(new ProductEnquiry() { StockCode = vpn, Quantity = 1 }));
                }
                var products = _productManager.GetProductDetails(inputModel.resellerNumber, skus, vpns, true, inputModel.ean);
                return Ok(products);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "ProductController.GetProductDetails :: Unhandled exception");
                return BadRequest(new { status = "Failed", error = ex.ToString() });
            }
        }

    }
}

