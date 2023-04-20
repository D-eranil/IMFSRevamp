using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Product
{
    public class GetProductsInfoInputModel
    {
        public string stockCodes { get; set; }
        public string templateId { get; set; }
        public string quantity { get; set; }
        public string resellerNumber { get; set; }
    }

    public class ProductQuantityBreakScale
    {
        public decimal? ScaleValue { get; set; }
        public decimal? Amount { get; set; }
    }


    public class ProductEnquiry
    {
        public string StockCode { get; set; }
        public int Quantity { get; set; }
    }

    public class BundleItemInfo
    {
        public string SKU { get; set; }
        public string VPN { get; set; }
        public string Description { get; set; }
        public decimal? CostPrice { get; set; }
        public int? Qty { get; set; }
    }


    public class ProductData
    {
        public string InternalSKUID { get; set; }
        public string VendorSKUID { get; set; }
        public string ProductDescription { get; set; }
        public int? VSRID { get; set; }
        public bool ShowSAPIcon { get; set; }
        public string CurrencyCode { get; set; }

        public decimal? UnitNetAmount { get; set; }
    }


}
