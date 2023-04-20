using IMFS.Web.Models.OPRDBModel;
using IMFS.Web.Models.Product;
using System;
using System.Collections.Generic;
using System.Text;
using IMFS.DataAccess.Models.StoreProcedures;


namespace IMFS.BusinessLogic.Product
{
    public interface IProductManager
    {
        List<mProduct> GetProducts(List<string> sku);
        List<mProduct> GetProducts(List<string> sku, bool isVPN);
        //List<spGetProductDetailsResult> GetProductDetails(string resellerNumber, List<ProductEnquiry> skus, List<ProductEnquiry> vpns, bool callSAPWebService = true, string ean = "", List<spGetProductDetailsResult> products = null);
        List<spGetProductDetailsResult> GetProductDetails(List<string> sku, List<string> vpn, string ean = "");

        List<ProductData> GetProductDetails(string resellerNumber, List<ProductEnquiry> skus, List<ProductEnquiry> vpns, bool callSAPWebService = true, string ean = "", List<ProductData> products = null);

    }
}
