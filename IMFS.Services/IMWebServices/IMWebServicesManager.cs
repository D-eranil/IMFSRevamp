using IMFS.Core.Extensions;
using IMFS.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IMFS.Services.IMWebServices
{
    public class IMWebServicesManager: IIMWebServicesManager
    {
        public ProductSAPConfig _productSAPConfig { get; set; }        
        public POCreateSAPConfig _poCreateSAPConfig;

        public IMWebServicesManager(ProductSAPConfig productSAPConfig, POCreateSAPConfig poCreateSAPConfig = null)
        {
            _productSAPConfig = productSAPConfig;
            _poCreateSAPConfig = poCreateSAPConfig;
        }

        public ProductSAPEnquiryOutput ProductSAPEnquiry(string resellerNumber, List<ProductSAPInputItem> items)
        {

            var inputModel = new ProductSAPEnquiry();
            var testResellerAccount = _productSAPConfig.TestResellerAccount.Split(',');

            if (string.IsNullOrEmpty(resellerNumber) || testResellerAccount.Contains(resellerNumber))
            {
                resellerNumber = _productSAPConfig.GenericResellerAccount;
            }
            inputModel.ResellerNumber = resellerNumber;
            inputModel.SalesOrganization = _productSAPConfig.SalesOrganization;
            inputModel.DistributionChannel = _productSAPConfig.DistributionChannel;
            inputModel.Division = _productSAPConfig.Division;
            inputModel.CustomerPOType = _productSAPConfig.CustomerPOType;
            inputModel.Items.AddRange(items);

            var requestModel = new PNAInput.ServiceRequest();
            requestModel.RequestPreamble = new PNAInput.RequestPreamble();
            requestModel.PriceAndAvailabilityRequest = new PNAInput.PriceAndAvailabilityRequest();

            requestModel.RequestPreamble.CustomerNumber = inputModel.ResellerNumber;
            requestModel.RequestPreamble.SalesOrganization = inputModel.SalesOrganization;
            requestModel.PriceAndAvailabilityRequest.DistributionChannel = inputModel.DistributionChannel;
            requestModel.PriceAndAvailabilityRequest.Division = inputModel.Division;
            requestModel.PriceAndAvailabilityRequest.CustomerPOType = inputModel.CustomerPOType;
            requestModel.PriceAndAvailabilityRequest.IsPricingRequired = true;
            requestModel.PriceAndAvailabilityRequest.IsAvailabilityRequired = true;

            requestModel.PriceAndAvailabilityRequest.Item = new PNAInput.PriceAndAvailabilityRequestItem[inputModel.Items.Count];
            for (int i = 0; i < inputModel.Items.Count; i++)
            {
                var item = inputModel.Items.ElementAt(i);
                if (!string.IsNullOrEmpty(item.SKU) && item.SKU.Length < 18)
                {
                    requestModel.PriceAndAvailabilityRequest.Item[i] = new PNAInput.PriceAndAvailabilityRequestItem()
                    {
                        IngramPartNumber = item.SKU.Trim(),
                        RequestedQuantity = item.RequestedQty,
                        RequestedQuantitySpecified = true
                    };
                }
                if (!string.IsNullOrEmpty(item.VPN))
                {
                    requestModel.PriceAndAvailabilityRequest.Item[i] = new PNAInput.PriceAndAvailabilityRequestItem()
                    {
                        VendorPartNumber = item.VPN.Trim(),
                        RequestedQuantity = item.RequestedQty,
                        RequestedQuantitySpecified = true
                    };
                }
            }
            return sendPNARequest(requestModel);
        }


        #region private methods

        private ProductSAPEnquiryOutput sendPNARequest(PNAInput.ServiceRequest requestModel)
        {
            HttpWebRequest webRequest = null;
            var response = new ProductSAPEnquiryOutput();

            string serviceRequestData = string.Empty;

            var xmlStringWriter = new StringWriter();
            var inputSerializer = new XmlSerializer(requestModel.GetType());
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            using (var xmlWriter = XmlWriter.Create(xmlStringWriter, settings))
            {
                inputSerializer.Serialize(xmlWriter, requestModel, emptyNamepsaces);
                serviceRequestData = xmlStringWriter.ToString();
            }

            webRequest = (HttpWebRequest)WebRequest.Create(_productSAPConfig.Url);
            webRequest.Headers.Add("SOAPAction", _productSAPConfig.Action);
            webRequest.Method = "POST";
            webRequest.Accept = "text/xml";
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";

            //ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            StreamWriter writer = new StreamWriter(webRequest.GetRequestStream());

            var inputXML = File.ReadAllText(_productSAPConfig.PNAInputTemplatePath);
            inputXML = string.Format(inputXML, _productSAPConfig.UserName, _productSAPConfig.Password, _productSAPConfig.UserCredentialID, serviceRequestData);
            writer.WriteLine(inputXML);
            writer.Close();
            // Send the data to the webserver
            var webResponse = webRequest.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream());

            var responseSerializer = new XmlSerializer(typeof(PNAResponse.Envelope));
            var rawResponse = responseSerializer.Deserialize(sr) as PNAResponse.Envelope;

            sr.Close();

            #region convert raw output to ProductSAPEnquiryOutput

            if (rawResponse.Body.ServiceResponse.PriceAndAvailabilityResponse == null)
            {
                // something wrong with web service call
                return null;
            }

            var itemResponses = rawResponse.Body.ServiceResponse.PriceAndAvailabilityResponse.ItemResponse;
            if (itemResponses != null)
            {
                foreach (var itemResponse in itemResponses)
                {
                    var item = new ProductSAPOutputItem();
                    item.IMSKU = (itemResponse.IngramPartNumber != null) ? itemResponse.IngramPartNumber.Text.TrimStart('0') : string.Empty;
                    item.VPN = (itemResponse.VendorPartNumber != null) ? itemResponse.VendorPartNumber.Text : string.Empty;
                    if (itemResponse.ItemStatus.Text == "SUCCESS")
                    {
                        #region Set Item Information

                        var requestedQty = itemResponse.RequestedQuantity.Text.ToDoubleNullable();
                        item.RequestedQty = Convert.ToInt32(requestedQty);

                        item.Description = itemResponse.IngramPartDescription.Text;
                        item.EANUPCCode = (itemResponse.EANUPCCode != null) ? itemResponse.EANUPCCode.Text : string.Empty;
                        item.VendorNumber = itemResponse.VendorNumber.Text.TrimStart('0');
                        item.VendorName = itemResponse.VendorName.Text;
                        item.UnitOfMeasure = (itemResponse.UnitOfMeasure != null) ? itemResponse.UnitOfMeasure.Text : string.Empty;
                        item.CurrencyCode = (itemResponse.CurrencyCode != null) ? itemResponse.CurrencyCode.Text : string.Empty;
                        item.StateCode = (itemResponse.StateCode != null) ? itemResponse.StateCode.Text : string.Empty;
                        item.Plant = (itemResponse.Plant != null) ? itemResponse.Plant.Text : string.Empty;
                        if (itemResponse.SalesMargin != null)
                        {
                            item.SalesMargin = itemResponse.SalesMargin.Text.ToDecimalNullable();
                        }
                        if (itemResponse.CostIngram1 != null)
                        {
                            item.CostIngramLocal = itemResponse.CostIngram1.Text.ToDecimalNullable();
                        }

                        if (itemResponse.InternalConditions != null && itemResponse.InternalConditions.Count > 0)
                        {
                            var netSalesCostConditions = itemResponse.InternalConditions.Where(x => x.ConditionType != null && x.ConditionType.Text == "ZNSC").ToList();
                            if (netSalesCostConditions != null)
                            {
                                // sometime SAP API return multiple ZNSC for same items, in this case get the first item for sales cost
                                // if item is bundle item, it has MaterialNumber inside netsales cost condition
                                var hasMaterialNumber = netSalesCostConditions.Any(x => x.MaterialNumber != null);

                                if ((netSalesCostConditions.Count == 1 && netSalesCostConditions.FirstOrDefault().Amount != null) || !hasMaterialNumber)
                                {
                                    // only one net sales cost
                                    var netSalesCostCondition = netSalesCostConditions.FirstOrDefault();
                                    item.NetSalesCost = netSalesCostCondition.Amount.Text.ToDecimal(0) / item.RequestedQty;

                                    //Cover PNA API bug
                                    // there are bundle SKU with only one item
                                    if (netSalesCostCondition.MaterialNumber != null)
                                    {
                                        var bundleSKU = netSalesCostCondition.MaterialNumber.Text.TrimStart('0');
                                        if (bundleSKU != item.IMSKU)
                                        {
                                            item.IsBundle = true;
                                            item.BundleItems = new List<SAPBundleItemInfo>();
                                            var bundleItem = new SAPBundleItemInfo();
                                            bundleItem.SKU = bundleSKU;
                                            if (netSalesCostCondition.Amount != null)
                                            {
                                                bundleItem.CostPrice = netSalesCostCondition.Amount.Text.ToDecimal(0);
                                            }
                                            var zcstResult = itemResponse.InternalConditions.Where(x => x.ConditionType != null && x.ConditionType.Text == "ZCST" && x.MaterialNumber.Text.TrimStart('0') == bundleItem.SKU).FirstOrDefault();
                                            if (zcstResult != null)
                                            {
                                                var amount = zcstResult.Amount.Text.ToDecimal();
                                                var rate = zcstResult.Rate.Text.ToDecimal();

                                                if (amount > 0 && amount > 0)
                                                {
                                                    int? qty = Convert.ToInt32(Math.Round(amount / rate));
                                                    if (qty.HasValue && qty.Value > 0)
                                                    {
                                                        bundleItem.Qty = qty;
                                                    }
                                                }
                                                else
                                                {
                                                    bundleItem.Qty = 1;
                                                }
                                            }
                                            else
                                            {
                                                bundleItem.Qty = 1;
                                            }
                                            item.BundleItems.Add(bundleItem);
                                        }
                                    }
                                }
                                else
                                {
                                    // more than one net sales cost. item is bundle
                                    item.NetSalesCost = 0;
                                    item.BundleItems = new List<SAPBundleItemInfo>();
                                    item.IsBundle = true;
                                    foreach (var netSalesCostCondition in netSalesCostConditions)
                                    {
                                        if (netSalesCostCondition.Amount != null)
                                        {
                                            item.NetSalesCost += netSalesCostCondition.Amount.Text.ToDecimal(0) / item.RequestedQty;
                                        }
                                        if (netSalesCostCondition.MaterialNumber != null)
                                        {
                                            var bundleItem = new SAPBundleItemInfo();
                                            bundleItem.SKU = netSalesCostCondition.MaterialNumber.Text.TrimStart('0');
                                            if (netSalesCostCondition.Amount != null)
                                            {
                                                bundleItem.CostPrice = netSalesCostCondition.Amount.Text.ToDecimal(0);
                                            }
                                            var zcstResult = itemResponse.InternalConditions.Where(x => x.ConditionType != null && x.ConditionType.Text == "ZCST" && x.MaterialNumber.Text.TrimStart('0') == bundleItem.SKU).FirstOrDefault();
                                            if (zcstResult != null)
                                            {
                                                var amount = zcstResult.Amount.Text.ToDecimal();
                                                var rate = zcstResult.Rate.Text.ToDecimal();

                                                if (amount > 0 && amount > 0)
                                                {
                                                    int? qty = Convert.ToInt32(Math.Round(amount / rate));
                                                    if (qty.HasValue && qty.Value > 0)
                                                    {
                                                        bundleItem.Qty = qty;
                                                    }
                                                }
                                                else
                                                {
                                                    bundleItem.Qty = 1;
                                                }
                                            }
                                            else
                                            {
                                                bundleItem.Qty = 1;
                                            }

                                            item.BundleItems.Add(bundleItem);
                                        }
                                    }
                                }
                            }
                        }

                        if (itemResponse.PlantAvailability != null)
                        {
                            // sometimes qty is returned with decimal
                            var qtyAvailable = itemResponse.PlantAvailability.Location.AvailableQuantity.Text.ToDoubleNullable();
                            item.QtyAvailable = Convert.ToInt32(qtyAvailable);
                        }
                        if (itemResponse.ReserveStockDetails != null)
                        {
                            // sometimes qty is returned with decimal
                            var qtyReserved = itemResponse.ReserveStockDetails.ReservedQuantity.Text.ToDoubleNullable();
                            item.QtyReserved = Convert.ToInt32(qtyReserved);
                        }

                        item.MultipleIngramPartExists = (itemResponse.MultipleIngramPartExists != null) ? itemResponse.MultipleIngramPartExists.Text.ToBoolNullable("true") : null;
                        item.MultipleVendorPartExists = (itemResponse.MultipleVendorPartExists != null) ? itemResponse.MultipleVendorPartExists.Text.ToBoolNullable("true") : null;
                        item.IsPartSoldSeperately = (itemResponse.IsPartSoldSeperately != null) ? itemResponse.IsPartSoldSeperately.Text.ToBoolNullable("true") : null;
                        item.IsAvailable = (itemResponse.IsAvailable != null) ? itemResponse.IsAvailable.Text.ToBoolNullable("true") : null;
                        item.IsStockable = (itemResponse.IsStockable != null) ? itemResponse.IsStockable.Text.ToBoolNullable("true") : null;
                        item.IsBOM = (itemResponse.IsBOM != null) ? itemResponse.IsBOM.Text.ToBoolNullable("true") : null;
                        item.HasPromotions = (itemResponse.HasPromotions != null) ? itemResponse.HasPromotions.Text.ToBoolNullable("true") : null;
                        item.HasQuantityBreaks = (itemResponse.HasQuantityBreaks != null) ? itemResponse.HasQuantityBreaks.Text.ToBoolNullable("true") : null;
                        item.HasWebDiscount = (itemResponse.HasWebDiscount != null) ? itemResponse.HasWebDiscount.Text.ToBoolNullable("true") : null;
                        item.RequiresEndUser = (itemResponse.RequiresEndUser != null) ? itemResponse.RequiresEndUser.Text.ToBoolNullable("true") : null;
                        item.IsSBOPricing = (itemResponse.IsSBOPricing != null) ? itemResponse.IsSBOPricing.Text.ToBoolNullable("true") : null;

                        item.UnitNetAmount = (itemResponse.UnitNetAmount != null) ? itemResponse.UnitNetAmount.Text.ToDecimalNullable() : null;
                        item.ExtendedNetAmount = (itemResponse.ExtendedNetAmount != null) ? itemResponse.ExtendedNetAmount.Text.ToDecimalNullable() : null;
                        item.TaxAmount = (itemResponse.TaxAmount != null) ? itemResponse.TaxAmount.Text.ToDecimalNullable() : null;
                        item.MSRPPrice = (itemResponse.MSRPPrice != null) ? itemResponse.MSRPPrice.Text.ToDecimalNullable() : null;

                        // MSRPPrice (RRP) is total price. Need to divided by Qty to get Unit RRP
                        if (item.MSRPPrice.HasValue && item.RequestedQty.HasValue && item.RequestedQty > 0)
                        {
                            item.MSRPPrice = item.MSRPPrice / item.RequestedQty;
                        }

                        if (!item.SalesMargin.HasValue && item.UnitNetAmount.HasValue && item.NetSalesCost.HasValue)
                        {
                            if (item.UnitNetAmount == 0)
                            {
                                if (item.NetSalesCost > 0)
                                {
                                    // if cost price has value and reseller price is 0, set gross profit percentage to -100%
                                    item.SalesMargin = -100;
                                }
                                else
                                {
                                    // if cost price and reseller price is 0, set gross profit percentage to 0
                                    item.SalesMargin = 0;
                                }
                            }
                            else
                            {
                                var grossProfit = item.UnitNetAmount - item.NetSalesCost;

                                item.SalesMargin = (grossProfit / item.UnitNetAmount) * 100;
                                item.SalesMargin = Math.Round(item.SalesMargin.Value, 2, MidpointRounding.AwayFromZero);
                            }
                        }

                        item.TotalFeeAmount = (itemResponse.TotalFeeAmount != null) ? itemResponse.TotalFeeAmount.Text.ToDecimalNullable() : null;
                        item.TotalPromotionAmount = (itemResponse.TotalPromotionAmount != null) ? itemResponse.TotalPromotionAmount.Text.ToDecimalNullable() : null;
                        item.TotalEnvironmentalFees = (itemResponse.TotalEnvironmentalFees != null) ? itemResponse.TotalEnvironmentalFees.Text.ToDecimalNullable() : null;
                        item.MPDoBem = (itemResponse.MPDoBem != null) ? itemResponse.MPDoBem.Text.ToDecimalNullable() : null;
                        item.ICMSTax = (itemResponse.ICMSTax != null) ? itemResponse.ICMSTax.Text.ToDecimalNullable() : null;
                        item.ICMSST = (itemResponse.ICMSST != null) ? itemResponse.ICMSST.Text.ToDecimalNullable() : null;
                        item.OtherTaxes = (itemResponse.OtherTaxes != null) ? itemResponse.OtherTaxes.Text.ToDecimalNullable() : null;
                        item.ExtendedFees = (itemResponse.ExtendedFees != null) ? itemResponse.ExtendedFees.Text.ToDecimalNullable() : null;

                        // Quantity Break Condition
                        if (itemResponse.QuantityBreakCondition != null && itemResponse.QuantityBreakCondition.Count > 0)
                        {
                            item.QuantityBreakCondition = new QuantityBreakCondition();
                            foreach (var qb in itemResponse.QuantityBreakCondition)
                            {
                                item.QuantityBreakCondition.ConditionType = (qb.ConditionType.Text != null) ? qb.ConditionType.Text : string.Empty;
                                item.QuantityBreakCondition.CurrencyCode = (qb.CurrencyCode.Text != null) ? qb.CurrencyCode.Text : string.Empty;
                                item.QuantityBreakCondition.Description = (qb.Description.Text != null) ? qb.Description.Text : string.Empty;
                                item.QuantityBreakCondition.ScaleUnit = (qb.ScaleUnit.Text != null) ? qb.ScaleUnit.Text : string.Empty;

                                if (qb.Scale != null && qb.Scale.Count() > 0)
                                {
                                    foreach (var scale in qb.Scale)
                                    {

                                        // Amount is the reseller price
                                        item.QuantityBreakCondition.Scales.Add(new QuantityBreakConditionScale()
                                        { ScaleValue = scale.ScaleValue.Text.ToDecimalNullable(), Amount = scale.Amount.Text.ToDecimalNullable() });

                                        //if (item.QuantityBreakCondition.ConditionType == "ZR00" && scale.Amount != null && scale.ScaleValue != null)
                                        //{
                                        //    // Amount is the reseller price
                                        //    item.QuantityBreakCondition.Scales.Add(new QuantityBreakConditionScale()
                                        //    { ScaleValue = scale.ScaleValue.Text.ToDecimalNullable(), Amount = scale.Amount.Text.ToDecimalNullable() });
                                        //}
                                        //else if ((item.QuantityBreakCondition.ConditionType == "ZLDS" || item.QuantityBreakCondition.ConditionType == "ZV03") && scale.Amount != null)
                                        //{
                                        //    var listDiscountedPrice = scale.Amount.Text.ToDecimalNullable();
                                        //    if (listDiscountedPrice > 0)
                                        //    {
                                        //        var unitRRP = item.MSRPPrice / item.RequestedQty;

                                        //        var discountPercent = 100 * (listDiscountedPrice - unitRRP) / listDiscountedPrice;
                                        //        decimal? newResellerPrice = null;

                                        //        newResellerPrice = unitRRP + unitRRP * discountPercent / 100;

                                        //        item.QuantityBreakCondition.Scales.Add(new QuantityBreakConditionScale()
                                        //        { ScaleValue = scale.ScaleValue.Text.ToDecimalNullable(), Amount = newResellerPrice });
                                        //    }
                                        //}
                                    }
                                }
                            }
                        }

                        #endregion Set Item Information
                    }
                    else
                    {
                        // item is failed.
                        // Error code is in Header and we need to find it with item line number

                        item.HasError = true;

                        // get line number first
                        var itemError = itemResponse.ReturnMessages.Error.FirstOrDefault();
                        var headerErrors = rawResponse.Body.ServiceResponse.ResponsePreamble.ReturnMessages.Error;

                        if (itemError != null && headerErrors != null)
                        {
                            var errorMessage = itemError.Message.Text.Split(':');
                            var lineNumber = (errorMessage.Count() > 1) ? errorMessage[1] : string.Empty;

                            var headerError = headerErrors.Where(x => x.Message.Text.Contains(lineNumber)).FirstOrDefault();
                            if (headerError != null)
                            {
                                item.ErrorCode = headerError.Code.Text;
                                item.ErrorMessage = errorMessage[0] + ":";
                                if (!string.IsNullOrEmpty(item.IMSKU))
                                {
                                    item.ErrorMessage += " " + item.IMSKU;
                                }
                                if (!string.IsNullOrEmpty(item.VPN))
                                {
                                    item.ErrorMessage += " " + item.VPN;
                                }
                            }
                        }
                    }
                    response.Items.Add(item);
                }
            }

            #endregion convert raw output to ProductSAPEnquiryOutput

            return response;
        }

        #endregion

    }
}
