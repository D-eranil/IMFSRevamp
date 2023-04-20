using IMFS.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.OPRDBModel;
using IMFS.DataAccess.Models.StoreProcedures;
using IMFS.Web.Models.Product;
using IMFS.Services.Models;
using IMFS.Web.Models.Enums;
using IMFS.Services.IMWebServices;
using IMFS.DataAccess.DBContexts;

namespace IMFS.BusinessLogic.Product
{
    public class ProductManager : IProductManager
    {
        
        private readonly IGenericORPrepository<Web.Models.OPRDBModel.mProduct> _mProductRepository;
    
        private readonly IIMWebServicesManager _webServicesManager;        
        private readonly IRepository<Web.Models.DBModel.Vendor> _vendorRepository;
        

        public ProductManager(IGenericORPrepository<Web.Models.OPRDBModel.mProduct> mProductRepository,
            IIMWebServicesManager webServicesManager,
            IRepository<Web.Models.DBModel.Vendor> vendorRepository        
            )
        {
            _mProductRepository = mProductRepository;
            _webServicesManager = webServicesManager;
            _vendorRepository = vendorRepository;
            //_orpDBCOntext = orpDBCOntext;
        }

        public List<mProduct> GetProducts(List<string> sku, bool isVPN)
        {
            if (isVPN)
            {
                return _mProductRepository.Table.Where(x => sku.Contains(x.VendorSKUID)).ToList();
            }
            else
            {
                return _mProductRepository.Table.Where(x => sku.Contains(x.InternalSKUID)).ToList();
            }
        }

        public List<mProduct> GetProducts(List<string> sku)
        {
            return _mProductRepository.Table.Where(x => sku.Contains(x.InternalSKUID) || sku.Contains(x.VendorSKUID)).ToList();
        }

        public List<ProductData> GetProductDetails(string resellerNumber, List<ProductEnquiry> skus, List<ProductEnquiry> vpns, bool callSAPWebService = true, string ean = "", List<ProductData> products = null)
        {
            if (skus == null)
            {
                skus = new List<ProductEnquiry>();
            }
            if (vpns == null)
            {
                vpns = new List<ProductEnquiry>();
            }
            if (products == null)
            {
                products = GetProductDetails(skus.Select(x => x.StockCode).ToList(), vpns.Select(x => x.StockCode).ToList(), ean);
            }

            //foreach (var prod in products)
            //{
            //    prod.ORPDBSell = prod.Sell;
            //    prod.ORPDBRRP = prod.RRP;
            //    prod.ORPDBNettSalesCost = prod.NettSalesCost;
            //    prod.ORPDBQtyAvailable = prod.QtyAvailable;
            //}

            if (!callSAPWebService) return products;

            #region call sap web service to get production information

            var items = new List<ProductSAPInputItem>();
            if (skus != null && skus.Count() > 0)
            {
                items.AddRange(skus.Select(x => new ProductSAPInputItem()
                {
                    SKU = x.StockCode,
                    RequestedQty = x.Quantity > 0 ? x.Quantity : 1,
                }).ToList());
            }

            if (vpns != null && vpns.Count() > 0)
            {
                items.AddRange(vpns.Select(x => new ProductSAPInputItem()
                {
                    VPN = x.StockCode,
                    RequestedQty = x.Quantity > 0 ? x.Quantity : 1,
                }).ToList());
            }

            var sapProductResponse = _webServicesManager.ProductSAPEnquiry(resellerNumber, items);

            #endregion call sap web service to get production information

            if (sapProductResponse == null) return products;

            //var exchangeRates = _exchangeRateManager.GetExchangeRateVals();
            //decimal usdExchangeRate = exchangeRates.Where(x => x.Currency == IMFSEnums.CurrencyType.USD.ToString()).FirstOrDefault().Rate.ToDecimal();

            foreach (var item in sapProductResponse.Items)
            {
                var product = products.Where(x => !string.IsNullOrEmpty(x.InternalSKUID) && !string.IsNullOrEmpty(item.IMSKU) &&
                x.InternalSKUID.ToLower() == item.IMSKU.ToLower()).FirstOrDefault();

                if (product == null)
                {
                    product = new ProductData();
                    products.Add(product);
                }
                if (item.HasError)
                {
                    // Error Code 007 means item is only available for certain reseller
                    //if (item.ErrorCode == "007")
                    //{
                    //    product.AuthorizedResellerOnly = true;
                    //}
                }
                else
                {
                    //Check if Vendor Number exists in mVendors
                    if (!CheckVendorNumberExists(item.VendorNumber))
                        continue;

                    product.ShowSAPIcon = true;
                    product.InternalSKUID = item.IMSKU;
                    product.VendorSKUID = item.VPN;
                    product.ProductDescription = item.Description;
                    product.CurrencyCode = item.CurrencyCode;
                    product.UnitNetAmount = item.UnitNetAmount;

                    //product.ORPDBQtyAvailable = product.QtyAvailable;
                    //product.QtyAvailable = item.QtyAvailable.HasValue ? item.QtyAvailable.Value : 0;

                    // some customer are set up as USD in SAP, in this case SAP return price in US$
                    if (item.CurrencyCode == IMFSEnums.CurrencyType.USD.ToString())
                    {
                        //product.ExchangeRate = (double)usdExchangeRate;
                        //item.MSRPPrice = item.MSRPPrice / usdExchangeRate;
                        //item.UnitNetAmount = item.UnitNetAmount / usdExchangeRate;
                        
                        item.MSRPPrice = item.MSRPPrice;
                        item.UnitNetAmount = item.UnitNetAmount;
                    }

                    //product.ORPDBSell = product.Sell;
                    //product.Sell = item.UnitNetAmount;

                    //product.ORPDBRRP = product.RRP;
                    //product.RRP = item.MSRPPrice;

                    //if (item.NetSalesCost.HasValue)
                    //{
                    //    product.ORPDBNettSalesCost = product.NettSalesCost;
                    //    product.NettSalesCost = item.NetSalesCost;
                    //}

                    //if (product.Sell.HasValue && product.NettSalesCost.HasValue)
                    //{
                    //    // if reseller price is 0, use different calculation to avoid divided by 0 problem
                    //    if (product.Sell == 0)
                    //    {
                    //        if (product.NettSalesCost > 0)
                    //        {
                    //            // if cost price has value and reseller price is 0, set gross profit percentage to -100%
                    //            product.GrossMargin = -100;
                    //        }
                    //        else
                    //        {
                    //            // if cost price and reseller price is 0, set gross profit percentage to 0
                    //            product.GrossMargin = 0;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        var grossProfit = product.Sell - product.NettSalesCost;

                    //        product.GrossMargin = (grossProfit / product.Sell) * 100;
                    //        product.GrossMargin = Math.Round(product.GrossMargin.Value, 2, MidpointRounding.AwayFromZero);
                    //    }
                    //}

                    //if (item.QuantityBreakCondition != null && item.QuantityBreakCondition.Scales != null && item.QuantityBreakCondition.Scales.Count() > 0)
                    //{
                    //    product.QuantityBreakPrice = new List<ProductQuantityBreakScale>();
                    //    foreach (var scale in item.QuantityBreakCondition.Scales)
                    //    {
                    //        product.QuantityBreakPrice.Add(new ProductQuantityBreakScale()
                    //        {
                    //            ScaleValue = scale.ScaleValue,
                    //            Amount = scale.Amount
                    //        });
                    //    }
                    //}

                    // get bundle info details
                    //if (item.IsBundle == true && item.BundleItems != null && item.BundleItems.Count > 0)
                    //{
                    //    var bundleSKUs = item.BundleItems.Select(x => x.SKU).ToList();
                    //    var bundleDetails = GetProductDetails(bundleSKUs, null);
                    //    product.BundleItems = new List<BundleItemInfo>();
                    //    foreach (var sapBundleItem in item.BundleItems)
                    //    {
                    //        var bundleInfo = bundleDetails.Where(x => x.InternalSKUID == sapBundleItem.SKU).FirstOrDefault();
                    //        var bundleItem = new BundleItemInfo();
                    //        bundleItem.SKU = sapBundleItem.SKU;
                    //        bundleItem.Qty = sapBundleItem.Qty;
                    //        bundleItem.CostPrice = sapBundleItem.CostPrice;
                    //        if (bundleInfo != null)
                    //        {
                    //            bundleItem.VPN = bundleInfo.VendorSKUID;
                    //            bundleItem.Description = bundleInfo.ProductDescription;
                    //        }
                    //        product.BundleItems.Add(bundleItem);
                    //    }
                    //}
                }
            }

            // remove all product without SKU to prevent from showing blank items
            products.RemoveAll(p => string.IsNullOrEmpty(p.InternalSKUID));
            return products;
        }


        public List<ProductData> GetProductDetails(List<string> sku, List<string> vpn, string ean = "")
        {           

            var prodData = new List<ProductData>();
            if (sku != null && sku.Count > 0)
            {
                sku.ForEach(s =>
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        var prd = _mProductRepository.Table.Where(x => x.InternalSKUID == s).FirstOrDefault();
                        if (prd != null)
                        {
                            var prdData = new ProductData();
                            prdData.InternalSKUID = prd.InternalSKUID;
                            prdData.VendorSKUID = prd.VendorSKUID;
                            prdData.ProductDescription = prd.ProductDescription;
                            prdData.VSRID = prd.VSRID;

                            prodData.Add(prdData);
                        }

                    }
                });
            }

            if (vpn != null && vpn.Count > 0)
            {
                vpn.ForEach(s =>
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        var prd = _mProductRepository.Table.Where(x => x.VendorSKUID == s).FirstOrDefault();
                        if (prd != null)
                        {
                            var prdData = new ProductData();
                            prdData.InternalSKUID = prd.InternalSKUID;
                            prdData.VendorSKUID = prd.VendorSKUID;
                            prdData.ProductDescription = prd.ProductDescription;
                            prdData.VSRID = prd.VSRID;

                            prodData.Add(prdData);
                        }
                    }
                });
            }

            return prodData;


            //var prods = _mProductRepository.Table.Where(x => sku.Contains(x.VendorSKUID)).ToList();
            //spGetProductDetails inputParams = new spGetProductDetails();
            //if (sku != null && sku.Count > 0)
            //{
            //    sku.ForEach(s =>
            //    {
            //        if (!string.IsNullOrEmpty(s))
            //        {
            //            inputParams.SKU.Add(new StringIDs() { ID = s });
            //        }
            //    });
            //}

            //if (vpn != null && vpn.Count > 0)
            //{
            //    vpn.ForEach(s =>
            //    {
            //        if (!string.IsNullOrEmpty(s))
            //        {
            //            inputParams.VPN.Add(new StringIDs() { ID = s });
            //        }
            //    });
            //}

            //if (!string.IsNullOrEmpty(ean))
            //{
            //    inputParams.EAN = ean;
            //}




        }

        private bool CheckVendorNumberExists(string vendorNumber)
        {
            if (string.IsNullOrEmpty(vendorNumber))
                return false;

            return true;
        //    vendorNumber = vendorNumber.TrimStart(new Char[] { '0' });
        //    var resVM = _vendorRepository.Table.Where(x => x.SupplierNumber == vendorNumber).FirstOrDefault();

        //    if (resVM == null)
        //        return false;
        //    else
        //        return true;
        }

        List<spGetProductDetailsResult> IProductManager.GetProductDetails(List<string> sku, List<string> vpn, string ean)
        {
            throw new NotImplementedException();
        }
    }
}
