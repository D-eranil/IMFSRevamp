using EntityFrameworkExtras.EF6;
using IMFS.Web.Models.Product;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IMFS.DataAccess.Models.StoreProcedures
{
    [StoredProcedure("sp_GetProductDetails")]
    public class spGetProductDetails
    {
        [StoredProcedureParameter(SqlDbType.Udt)]
        public List<StringIDs> SKU { get; set; }

        [StoredProcedureParameter(SqlDbType.Udt)]
        public List<StringIDs> VPN { get; set; }

        [StoredProcedureParameter(SqlDbType.VarChar)]
        public string EAN { get; set; }

        public spGetProductDetails()
        {
            SKU = new List<StringIDs>();
            VPN = new List<StringIDs>();
        }
    }

    public class spGetProductDetailsResult
    {
        public string InternalSKUID { get; set; }
        public string VendorSKUID { get; set; }
        public string ProductDescription { get; set; }
        public string SalesBlock { get; set; }
        public string PurchasingBlock { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LongDescription { get; set; }
        public bool? HasAddOnOptions { get; set; }
        public string ProductHierarchy { get; set; }
        public string ProductHierarchyLevel1 { get; set; }
        public string ProductHierarchyLevel2 { get; set; }
        public string ProductHierarchyLevel3 { get; set; }
        public string MaterialGroup { get; set; }
        public string ItemCategory { get; set; }
        public long? PIR { get; set; }
        public bool? PriceFile { get; set; }
        public int? ProfitCenter { get; set; }
        public bool? Web { get; set; }
        public string UPCEAN { get; set; }
        public string ABCStatus { get; set; }
        public bool? NoBackOrders { get; set; }
        public string ReasonType { get; set; }
        public string ReasonValue { get; set; }
        public string PurchaseCurrency { get; set; }
        public string SerialProfile { get; set; }
        public string Incoterm { get; set; }
        public int? LeadTime { get; set; }
        public bool? BulkFlag { get; set; }
        public int? VSRID { get; set; }
        public string VSRDescription { get; set; }
        public string VSROperationGroup { get; set; }
        public string VSRGroupDescription { get; set; }
        public string VSRDepartmentDescription { get; set; }
        public string VSRBusinessUnitDescription { get; set; }
        public string LOB { get; set; }
        public string ProductManager { get; set; }
        public string ProductManagerName { get; set; }
        public string BusinessManager { get; set; }
        public string BusinessManagerName { get; set; }
        public string GeneralManager { get; set; }
        public string GeneralManagerName { get; set; }
        public string LocalOperationContact { get; set; }
        public string LocalOperationContactName { get; set; }
        public decimal? DickerSell { get; set; }
        public int? DickerSOH { get; set; }
        public decimal? SynnexSell { get; set; }
        public int? SynnexSOH { get; set; }
        public int? QtyAvailable { get; set; }
        public int? ORPDBQtyAvailable { get; set; }     // this field is set in code if SAP web service call is made
        public int? QtyOnHand { get; set; }
        public int? QTYCommitted { get; set; }
        public int? QTYOnOrder { get; set; }
        public decimal? ReplacementCost { get; set; }
        public decimal? AverageCost { get; set; }
        public decimal? ORPDBSell { get; set; }         // this field is set in code if SAP web service call is made
        public decimal? Sell { get; set; }
        public decimal? ORPDBRRP { get; set; }          // this field is set in code if SAP web service call is made
        public decimal? RRP { get; set; }
        public decimal? NettSalesCost { get; set; }
        public decimal? ORPDBNettSalesCost { get; set; }       // this field is set in code if SAP web service call is made
        public decimal? GrossMargin { get; set; }
        public int? VendorID { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string SupplierNumber { get; set; }
        public string ManufacturerNumber { get; set; }
        public string MaterialTypeCode { get; set; }

        public bool AuthorizedResellerOnly { get; set; } // this field is set in code after calling sap web service
        public bool ShowSAPIcon { get; set; }   // this field is set in code after calling sap web service

        public string CurrencyCode { get; set; }    // this field is set in code after calling sap web service. some customer in SAP are set up as USD
        public double? ExchangeRate { get; set; }   // this field is set in code after calling sap web service. some customer in SAP are set up as USD

        public int? SOH { get; set; }
        public double? SoldCurrent { get; set; }
        public double? Sold1 { get; set; }
        public double? Sold2 { get; set; }
        public double? Sold3 { get; set; }
        public double? Sold4 { get; set; }
        public double? Sold5 { get; set; }
        public double? Sold6 { get; set; }
        public double? Aged90U { get; set; }
        public double? Aged180U { get; set; }
        public double? DosTarget { get; set; }
        public bool? OnWeb { get; set; }
        public double? Sold7 { get; set; }
        public double? Sold8 { get; set; }
        public double? Sold9 { get; set; }
        public double? Sold10 { get; set; }
        public double? Sold11 { get; set; }
        public double? Sold12 { get; set; }
        public decimal? MAP { get; set; }
        public double? Aged30 { get; set; }
        public double? Aged60 { get; set; }
        public double? Aged90 { get; set; }
        public double? Aged120 { get; set; }
        public double? Aged121 { get; set; }
        public double? MarginFloor { get; set; }
        public int? MinQty { get; set; }
        public string HelpNotes { get; set; }

        public List<ProductQuantityBreakScale> QuantityBreakPrice { get; set; }      // this field is set in code after calling sap web service. some SKU in SAP are set up as quantity break price
        public List<BundleItemInfo> BundleItems { get; set; }      // this field is set in code after calling sap web service. some SKU in SAP are set up as bundle and it return cost price and SKU

    }
}
