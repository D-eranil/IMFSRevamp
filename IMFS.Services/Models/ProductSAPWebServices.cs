using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Services.Models
{
    public class ProductSAPConfig
    {
        public string TestResellerAccount { get; set; }
        public string GenericResellerAccount { get; set; }
        public string SalesOrganization { get; set; }
        public string DistributionChannel { get; set; }
        public string Division { get; set; }
        public string CustomerPOType { get; set; }

        public string Url { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserCredentialID { get; set; }
        public string PNAInputTemplatePath { get; set; }
    }

    public class ProductSAPEnquiry
    {
        public string ResellerNumber { get; set; }
        public List<ProductSAPInputItem> Items { get; set; }
        public string SalesOrganization { get; set; }
        public string DistributionChannel { get; set; }
        public string Division { get; set; }
        public string CustomerPOType { get; set; }

        public ProductSAPEnquiry()
        {
            ResellerNumber = string.Empty;
            Items = new List<ProductSAPInputItem>();
            SalesOrganization = string.Empty;
            DistributionChannel = string.Empty;
            Division = string.Empty;
            CustomerPOType = string.Empty;
        }
    }

    public class ProductSAPInputItem
    {
        public string SKU { get; set; }
        public string VPN { get; set; }
        public int RequestedQty { get; set; }
        public string MaterialType { get; set; }
    }

    public class ProductSAPEnquiryOutput
    {
        public List<ProductSAPOutputItem> Items { get; set; }

        public ProductSAPEnquiryOutput()
        {
            Items = new List<ProductSAPOutputItem>();
        }
    }

    public class ProductSAPOutputItem
    {
        public string ErrorCode { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }

        public string IMSKU { get; set; }
        public string VPN { get; set; }
        public string Description { get; set; }
        public string EANUPCCode { get; set; }
        public int VendorId { get; set; }
        public string VendorNumber { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }

        public string UnitOfMeasure { get; set; }
        public string CurrencyCode { get; set; }
        public string StateCode { get; set; }
        public string Plant { get; set; }
        public decimal? SalesMargin { get; set; }

        public int? QtyAvailable { get; set; }
        public int? QtyReserved { get; set; }

        public bool? MultipleVendorPartExists { get; set; }
        public bool? MultipleIngramPartExists { get; set; }
        public int? RequestedQty { get; set; }
        public bool? IsPartSoldSeperately { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? IsStockable { get; set; }
        public bool? IsBOM { get; set; }
        public bool? HasPromotions { get; set; }
        public bool? HasQuantityBreaks { get; set; }
        public bool? HasWebDiscount { get; set; }
        public bool? RequiresEndUser { get; set; }
        public bool? IsSBOPricing { get; set; }
        public decimal? UnitNetAmount { get; set; }
        public decimal? ExtendedNetAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? MSRPPrice { get; set; }
        public decimal? CostIngramLocal { get; set; }
        public decimal? NetSalesCost { get; set; }
        public decimal? TotalFeeAmount { get; set; }
        public decimal? TotalPromotionAmount { get; set; }
        public decimal? TotalEnvironmentalFees { get; set; }
        public decimal? MPDoBem { get; set; }
        public decimal? ICMSTax { get; set; }
        public decimal? ICMSST { get; set; }
        public decimal? OtherTaxes { get; set; }
        public decimal? ExtendedFees { get; set; }
        public QuantityBreakCondition QuantityBreakCondition { get; set; }
        public bool? IsBundle { get; set; }
        public List<SAPBundleItemInfo> BundleItems { get; set; }
    }

    public class QuantityBreakCondition
    {
        public string ConditionType { get; set; }
        public string Description { get; set; }
        public string CurrencyCode { get; set; }
        public string ScaleUnit { get; set; }
        public List<QuantityBreakConditionScale> Scales { get; set; }

        public QuantityBreakCondition()
        {
            Scales = new List<QuantityBreakConditionScale>();
        }
    }

    public class QuantityBreakConditionScale
    {
        public decimal? ScaleValue { get; set; }
        public decimal? Amount { get; set; }
    }

    public class SAPBundleItemInfo
    {
        public string SKU { get; set; }
        public decimal? CostPrice { get; set; }
        public int? Qty { get; set; }
    }

    public class GetProductDetails
    {
        public List<string> imskus { get; set; }
        public List<string> vpns { get; set; }
        public string resellerNumber { get; set; }
        public string ean { get; set; }
        public GetProductDetails()
        {
            imskus = new List<string>();
            vpns = new List<string>();
        }
    }
}
