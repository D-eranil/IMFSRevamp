using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace IMFS.Services.Models
{
    public class PNAInput
    {
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.ingrammicro.com/product/PriceAndAvailabilityRequest_v2_6/types")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.ingrammicro.com/product/PriceAndAvailabilityRequest_v2_6/types", IsNullable = false)]
        public partial class ServiceRequest
        {

            private RequestPreamble requestPreambleField;

            private PriceAndAvailabilityRequest priceAndAvailabilityRequestField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
            public RequestPreamble RequestPreamble
            {
                get
                {
                    return this.requestPreambleField;
                }
                set
                {
                    this.requestPreambleField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
            public PriceAndAvailabilityRequest PriceAndAvailabilityRequest
            {
                get
                {
                    return this.priceAndAvailabilityRequestField;
                }
                set
                {
                    this.priceAndAvailabilityRequestField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class RequestPreamble
        {

            private string customerNumberField;

            private string vendorNumberField;

            private string associateIDField;

            private string companyCodeField;

            private string salesOrganizationField;

            private string purchasingOrganizationField;

            /// <remarks/>
            public string CustomerNumber
            {
                get
                {
                    return this.customerNumberField;
                }
                set
                {
                    this.customerNumberField = value;
                }
            }

            /// <remarks/>
            public string VendorNumber
            {
                get
                {
                    return this.vendorNumberField;
                }
                set
                {
                    this.vendorNumberField = value;
                }
            }

            /// <remarks/>
            public string AssociateID
            {
                get
                {
                    return this.associateIDField;
                }
                set
                {
                    this.associateIDField = value;
                }
            }

            /// <remarks/>
            public string CompanyCode
            {
                get
                {
                    return this.companyCodeField;
                }
                set
                {
                    this.companyCodeField = value;
                }
            }

            /// <remarks/>
            public string SalesOrganization
            {
                get
                {
                    return this.salesOrganizationField;
                }
                set
                {
                    this.salesOrganizationField = value;
                }
            }

            /// <remarks/>
            public string PurchasingOrganization
            {
                get
                {
                    return this.purchasingOrganizationField;
                }
                set
                {
                    this.purchasingOrganizationField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class PriceAndAvailabilityRequest
        {

            private string distributionChannelField;

            private string divisionField;

            private string customerPOTypeField;

            private bool isPricingRequiredField;

            private bool isAvailabilityRequiredField;

            private string currencyCodeField;

            private string shipToCustomerNumberField;

            private string shipToRegionCodeField;

            private string sourceModeField;

            private PriceAndAvailabilityRequestItem[] itemField;

            /// <remarks/>
            public string DistributionChannel
            {
                get
                {
                    return this.distributionChannelField;
                }
                set
                {
                    this.distributionChannelField = value;
                }
            }

            /// <remarks/>
            public string Division
            {
                get
                {
                    return this.divisionField;
                }
                set
                {
                    this.divisionField = value;
                }
            }

            /// <remarks/>
            public string CustomerPOType
            {
                get
                {
                    return this.customerPOTypeField;
                }
                set
                {
                    this.customerPOTypeField = value;
                }
            }

            /// <remarks/>
            public bool IsPricingRequired
            {
                get
                {
                    return this.isPricingRequiredField;
                }
                set
                {
                    this.isPricingRequiredField = value;
                }
            }

            /// <remarks/>
            public bool IsAvailabilityRequired
            {
                get
                {
                    return this.isAvailabilityRequiredField;
                }
                set
                {
                    this.isAvailabilityRequiredField = value;
                }
            }

            /// <remarks/>
            public string CurrencyCode
            {
                get
                {
                    return this.currencyCodeField;
                }
                set
                {
                    this.currencyCodeField = value;
                }
            }

            /// <remarks/>
            public string ShipToCustomerNumber
            {
                get
                {
                    return this.shipToCustomerNumberField;
                }
                set
                {
                    this.shipToCustomerNumberField = value;
                }
            }

            /// <remarks/>
            public string ShipToRegionCode
            {
                get
                {
                    return this.shipToRegionCodeField;
                }
                set
                {
                    this.shipToRegionCodeField = value;
                }
            }

            /// <remarks/>
            public string SourceMode
            {
                get
                {
                    return this.sourceModeField;
                }
                set
                {
                    this.sourceModeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Item")]
            public PriceAndAvailabilityRequestItem[] Item
            {
                get
                {
                    return this.itemField;
                }
                set
                {
                    this.itemField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class PriceAndAvailabilityRequestItem
        {

            private string itemNumberField;

            private string ingramPartNumberField;

            private string vendorPartNumberField;

            private string customerPartNumberField;

            private string eANUPCCodeField;

            private decimal requestedQuantityField;

            private bool requestedQuantityFieldSpecified;

            private string unitOfMeasureField;

            private string bidNumberField;

            private string endUserNumberField;

            private string shipToCustomerNumberField;

            private string shipToRegionCodeField;

            private string plantForPricingField;

            private string storageLocationForPricingField;

            private bool isReserveInventoryRequiredField;

            private bool isReserveInventoryRequiredFieldSpecified;

            private string createReasonCodeField;

            private string createReasonValueField;

            /// <remarks/>
            public string ItemNumber
            {
                get
                {
                    return this.itemNumberField;
                }
                set
                {
                    this.itemNumberField = value;
                }
            }

            /// <remarks/>
            public string IngramPartNumber
            {
                get
                {
                    return this.ingramPartNumberField;
                }
                set
                {
                    this.ingramPartNumberField = value;
                }
            }

            /// <remarks/>
            public string VendorPartNumber
            {
                get
                {
                    return this.vendorPartNumberField;
                }
                set
                {
                    this.vendorPartNumberField = value;
                }
            }

            /// <remarks/>
            public string CustomerPartNumber
            {
                get
                {
                    return this.customerPartNumberField;
                }
                set
                {
                    this.customerPartNumberField = value;
                }
            }

            /// <remarks/>
            public string EANUPCCode
            {
                get
                {
                    return this.eANUPCCodeField;
                }
                set
                {
                    this.eANUPCCodeField = value;
                }
            }

            /// <remarks/>
            public decimal RequestedQuantity
            {
                get
                {
                    return this.requestedQuantityField;
                }
                set
                {
                    this.requestedQuantityField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool RequestedQuantitySpecified
            {
                get
                {
                    return this.requestedQuantityFieldSpecified;
                }
                set
                {
                    this.requestedQuantityFieldSpecified = value;
                }
            }

            /// <remarks/>
            public string UnitOfMeasure
            {
                get
                {
                    return this.unitOfMeasureField;
                }
                set
                {
                    this.unitOfMeasureField = value;
                }
            }

            /// <remarks/>
            public string BidNumber
            {
                get
                {
                    return this.bidNumberField;
                }
                set
                {
                    this.bidNumberField = value;
                }
            }

            /// <remarks/>
            public string EndUserNumber
            {
                get
                {
                    return this.endUserNumberField;
                }
                set
                {
                    this.endUserNumberField = value;
                }
            }

            /// <remarks/>
            public string ShipToCustomerNumber
            {
                get
                {
                    return this.shipToCustomerNumberField;
                }
                set
                {
                    this.shipToCustomerNumberField = value;
                }
            }

            /// <remarks/>
            public string ShipToRegionCode
            {
                get
                {
                    return this.shipToRegionCodeField;
                }
                set
                {
                    this.shipToRegionCodeField = value;
                }
            }

            /// <remarks/>
            public string PlantForPricing
            {
                get
                {
                    return this.plantForPricingField;
                }
                set
                {
                    this.plantForPricingField = value;
                }
            }

            /// <remarks/>
            public string StorageLocationForPricing
            {
                get
                {
                    return this.storageLocationForPricingField;
                }
                set
                {
                    this.storageLocationForPricingField = value;
                }
            }

            /// <remarks/>
            public bool IsReserveInventoryRequired
            {
                get
                {
                    return this.isReserveInventoryRequiredField;
                }
                set
                {
                    this.isReserveInventoryRequiredField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool IsReserveInventoryRequiredSpecified
            {
                get
                {
                    return this.isReserveInventoryRequiredFieldSpecified;
                }
                set
                {
                    this.isReserveInventoryRequiredFieldSpecified = value;
                }
            }

            /// <remarks/>
            public string CreateReasonCode
            {
                get
                {
                    return this.createReasonCodeField;
                }
                set
                {
                    this.createReasonCodeField = value;
                }
            }

            /// <remarks/>
            public string CreateReasonValue
            {
                get
                {
                    return this.createReasonValueField;
                }
                set
                {
                    this.createReasonValueField = value;
                }
            }
        }
    }

    public class PNAResponse
    {
        /// <summary>
        /// Summary description for Response
        /// </summary>

        [XmlRoot(ElementName = "TransactionID", Namespace = "")]
        public class TransactionID
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "TransactionStartTime", Namespace = "")]
        public class TransactionStartTime
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "TransactionEndTime", Namespace = "")]
        public class TransactionEndTime
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "ComponentName", Namespace = "")]
        public class ComponentName
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "Duration", Namespace = "")]
        public class Duration
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "PerformanceData", Namespace = "")]
        public class PerformanceData
        {
            [XmlElement(ElementName = "ComponentName", Namespace = "")]
            public ComponentName ComponentName { get; set; }
            [XmlElement(ElementName = "Duration", Namespace = "")]
            public Duration Duration { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "ServiceResponseHeader", Namespace = "http://www.ingrammicro.com/common/ServiceResponseHeader_v2_2/types")]
        public class ServiceResponseHeader
        {
            [XmlElement(ElementName = "TransactionID", Namespace = "")]
            public TransactionID TransactionID { get; set; }
            [XmlElement(ElementName = "TransactionStartTime", Namespace = "")]
            public TransactionStartTime TransactionStartTime { get; set; }
            [XmlElement(ElementName = "TransactionEndTime", Namespace = "")]
            public TransactionEndTime TransactionEndTime { get; set; }
            [XmlElement(ElementName = "PerformanceData", Namespace = "")]
            public PerformanceData PerformanceData { get; set; }
            [XmlAttribute(AttributeName = "xs", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Xs { get; set; }
            [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Xsi { get; set; }
            [XmlAttribute(AttributeName = "ns", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Ns { get; set; }
            [XmlAttribute(AttributeName = "ns0", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Ns0 { get; set; }
        }

        [XmlRoot(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public class Header
        {
            [XmlElement(ElementName = "ServiceResponseHeader", Namespace = "http://www.ingrammicro.com/common/ServiceResponseHeader_v2_2/types")]
            public ServiceResponseHeader ServiceResponseHeader { get; set; }
        }

        [XmlRoot(ElementName = "RequestStatus", Namespace = "")]
        public class RequestStatus
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "Code", Namespace = "")]
        public class Code
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "Message", Namespace = "")]
        public class Message
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "Success", Namespace = "")]
        public class Success
        {
            [XmlElement(ElementName = "Code")]
            public Code Code { get; set; }
            [XmlElement(ElementName = "Message")]
            public Message Message { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "Error", Namespace = "")]
        public class Error
        {
            [XmlElement(ElementName = "Code")]
            public Code Code { get; set; }
            [XmlElement(ElementName = "Message")]
            public Message Message { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "ReturnMessages", Namespace = "")]
        public class ReturnMessages
        {
            [XmlElement(ElementName = "Success")]
            public List<Success> Success { get; set; }
            [XmlElement(ElementName = "Error")]
            public List<Error> Error { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "ResponsePreamble", Namespace = "")]
        public class ResponsePreamble
        {
            [XmlElement(ElementName = "RequestStatus", Namespace = "")]
            public RequestStatus RequestStatus { get; set; }
            [XmlElement(ElementName = "ReturnMessages", Namespace = "")]
            public ReturnMessages ReturnMessages { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "ItemStatus", Namespace = "")]
        public class ItemStatus
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "MultipleVendorPartExists", Namespace = "")]
        public class MultipleVendorPartExists
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "MultipleIngramPartExists", Namespace = "")]
        public class MultipleIngramPartExists
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "RequestedQuantity", Namespace = "")]
        public class RequestedQuantity
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "IsPartSoldSeperately", Namespace = "")]
        public class IsPartSoldSeperately
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "IsAvailable", Namespace = "")]
        public class IsAvailable
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "IsStockable", Namespace = "")]
        public class IsStockable
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "IsBOM", Namespace = "")]
        public class IsBOM
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "HasPromotions", Namespace = "")]
        public class HasPromotions
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "HasQuantityBreaks", Namespace = "")]
        public class HasQuantityBreaks
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "HasWebDiscount", Namespace = "")]
        public class HasWebDiscount
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "RequiresEndUser", Namespace = "")]
        public class RequiresEndUser
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "IsSBOPricing", Namespace = "")]
        public class IsSBOPricing
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "UnitNetAmount", Namespace = "")]
        public class UnitNetAmount
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "ExtendedNetAmount", Namespace = "")]
        public class ExtendedNetAmount
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "TaxAmount", Namespace = "")]
        public class TaxAmount
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "MSRPPrice", Namespace = "")]
        public class MSRPPrice
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "TotalFeeAmount", Namespace = "")]
        public class TotalFeeAmount
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "TotalPromotionAmount", Namespace = "")]
        public class TotalPromotionAmount
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "TotalEnvironmentalFees")]
        public class TotalEnvironmentalFees
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "MPDoBem")]
        public class MPDoBem
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "ICMSTax")]
        public class ICMSTax
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "ICMSST")]
        public class ICMSST
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "OtherTaxes")]
        public class OtherTaxes
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "ExtendedFees")]
        public class ExtendedFees
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "ItemResponse", Namespace = "")]
        public class ItemResponse
        {
            [XmlElement(ElementName = "ItemStatus", Namespace = "")]
            public ItemStatus ItemStatus { get; set; }
            [XmlElement(ElementName = "ReturnMessages", Namespace = "")]
            public ReturnMessages ReturnMessages { get; set; }
            [XmlElement(ElementName = "MultipleVendorPartExists", Namespace = "")]
            public MultipleVendorPartExists MultipleVendorPartExists { get; set; }
            [XmlElement(ElementName = "MultipleIngramPartExists", Namespace = "")]
            public MultipleIngramPartExists MultipleIngramPartExists { get; set; }
            [XmlElement(ElementName = "RequestedQuantity")]
            public RequestedQuantity RequestedQuantity { get; set; }
            [XmlElement(ElementName = "IsPartSoldSeperately")]
            public IsPartSoldSeperately IsPartSoldSeperately { get; set; }
            [XmlElement(ElementName = "IsAvailable", Namespace = "")]
            public IsAvailable IsAvailable { get; set; }
            [XmlElement(ElementName = "IsStockable")]
            public IsStockable IsStockable { get; set; }
            [XmlElement(ElementName = "IsBOM")]
            public IsBOM IsBOM { get; set; }
            [XmlElement(ElementName = "HasPromotions")]
            public HasPromotions HasPromotions { get; set; }
            [XmlElement(ElementName = "HasQuantityBreaks")]
            public HasQuantityBreaks HasQuantityBreaks { get; set; }
            [XmlElement(ElementName = "HasWebDiscount")]
            public HasWebDiscount HasWebDiscount { get; set; }
            [XmlElement(ElementName = "RequiresEndUser")]
            public RequiresEndUser RequiresEndUser { get; set; }
            [XmlElement(ElementName = "IsSBOPricing")]
            public IsSBOPricing IsSBOPricing { get; set; }
            [XmlElement(ElementName = "UnitNetAmount")]
            public UnitNetAmount UnitNetAmount { get; set; }
            [XmlElement(ElementName = "ExtendedNetAmount")]
            public ExtendedNetAmount ExtendedNetAmount { get; set; }
            [XmlElement(ElementName = "TaxAmount")]
            public TaxAmount TaxAmount { get; set; }
            [XmlElement(ElementName = "MSRPPrice")]
            public MSRPPrice MSRPPrice { get; set; }
            [XmlElement(ElementName = "TotalFeeAmount")]
            public TotalFeeAmount TotalFeeAmount { get; set; }
            [XmlElement(ElementName = "TotalPromotionAmount")]
            public TotalPromotionAmount TotalPromotionAmount { get; set; }
            [XmlElement(ElementName = "TotalEnvironmentalFees")]
            public TotalEnvironmentalFees TotalEnvironmentalFees { get; set; }
            [XmlElement(ElementName = "MPDoBem")]
            public MPDoBem MPDoBem { get; set; }
            [XmlElement(ElementName = "ICMSTax")]
            public ICMSTax ICMSTax { get; set; }
            [XmlElement(ElementName = "ICMSST")]
            public ICMSST ICMSST { get; set; }
            [XmlElement(ElementName = "OtherTaxes")]
            public OtherTaxes OtherTaxes { get; set; }
            [XmlElement(ElementName = "ExtendedFees")]
            public ExtendedFees ExtendedFees { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlElement(ElementName = "IngramPartNumber", Namespace = "")]
            public IngramPartNumber IngramPartNumber { get; set; }
            [XmlElement(ElementName = "IngramPartDescription", Namespace = "")]
            public IngramPartDescription IngramPartDescription { get; set; }
            [XmlElement(ElementName = "VendorPartNumber")]
            public VendorPartNumber VendorPartNumber { get; set; }
            [XmlElement(ElementName = "VendorNumber")]
            public VendorNumber VendorNumber { get; set; }
            [XmlElement(ElementName = "VendorName")]
            public VendorName VendorName { get; set; }
            [XmlElement(ElementName = "UnitOfMeasure", Namespace = "")]
            public UnitOfMeasure UnitOfMeasure { get; set; }
            [XmlElement(ElementName = "CurrencyCode", Namespace = "")]
            public CurrencyCode CurrencyCode { get; set; }
            [XmlElement(ElementName = "StateCode", Namespace = "")]
            public StateCode StateCode { get; set; }
            [XmlElement(ElementName = "Plant", Namespace = "")]
            public Plant Plant { get; set; }
            [XmlElement(ElementName = "PlantAvailability", Namespace = "")]
            public PlantAvailability PlantAvailability { get; set; }
            [XmlElement(ElementName = "QuantityBreakCondition", Namespace = "")]
            public List<QuantityBreakCondition> QuantityBreakCondition { get; set; }
            [XmlElement(ElementName = "InternalConditions", Namespace = "")]
            public List<InternalConditions> InternalConditions { get; set; }
            [XmlElement(ElementName = "ReserveStockDetails", Namespace = "")]
            public ReserveStockDetails ReserveStockDetails { get; set; }
            [XmlElement(ElementName = "EANUPCCode", Namespace = "")]
            public EANUPCCode EANUPCCode { get; set; }
            [XmlElement(ElementName = "SalesMargin", Namespace = "")]
            public Plant SalesMargin { get; set; }
            [XmlElement(ElementName = "CostIngram1", Namespace = "")]
            public CostIngram1 CostIngram1 { get; set; }
        }

        [XmlRoot(ElementName = "IngramPartNumber")]
        public class IngramPartNumber
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "IngramPartDescription")]
        public class IngramPartDescription
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "VendorPartNumber")]
        public class VendorPartNumber
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "VendorNumber")]
        public class VendorNumber
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "VendorName")]
        public class VendorName
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "UnitOfMeasure")]
        public class UnitOfMeasure
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "CurrencyCode")]
        public class CurrencyCode
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "StateCode")]
        public class StateCode
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "Plant")]
        public class Plant
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "CostIngram1")]
        public class CostIngram1
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "PlantDescription")]
        public class PlantDescription
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "StorageLocation")]
        public class StorageLocation
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "StorageLocationDescription")]
        public class StorageLocationDescription
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "AvailableQuantity")]
        public class AvailableQuantity
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "Location", Namespace = "")]
        public class Location
        {
            [XmlElement(ElementName = "StorageLocation", Namespace = "")]
            public StorageLocation StorageLocation { get; set; }
            [XmlElement(ElementName = "StorageLocationDescription", Namespace = "")]
            public StorageLocationDescription StorageLocationDescription { get; set; }
            [XmlElement(ElementName = "AvailableQuantity", Namespace = "")]
            public AvailableQuantity AvailableQuantity { get; set; }
            [XmlElement(ElementName = "UnitOfMeasure", Namespace = "")]
            public UnitOfMeasure UnitOfMeasure { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "PlantAvailability", Namespace = "")]
        public class PlantAvailability
        {
            [XmlElement(ElementName = "Plant", Namespace = "")]
            public Plant Plant { get; set; }
            [XmlElement(ElementName = "PlantDescription", Namespace = "")]
            public PlantDescription PlantDescription { get; set; }
            [XmlElement(ElementName = "Location", Namespace = "")]
            public Location Location { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "ConditionType")]
        public class ConditionType
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "Description")]
        public class Description
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "ScaleUnit")]
        public class ScaleUnit
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "Scale")]
        public class Scale
        {
            [XmlElement(ElementName = "ScaleValue", Namespace = "")]
            public ScaleValue ScaleValue { get; set; }
            [XmlElement(ElementName = "Amount", Namespace = "")]
            public Amount Amount { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "ScaleValue")]
        public class ScaleValue
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "Amount")]
        public class Amount
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "MaterialNumber")]
        public class MaterialNumber
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "Rate")]
        public class Rate
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "ConditionPricingUnit")]
        public class ConditionPricingUnit
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }


        [XmlRoot(ElementName = "QuantityBreakCondition", Namespace = "")]
        public class QuantityBreakCondition
        {
            [XmlElement(ElementName = "ConditionType", Namespace = "")]
            public ConditionType ConditionType { get; set; }
            [XmlElement(ElementName = "Description", Namespace = "")]
            public Description Description { get; set; }
            [XmlElement(ElementName = "CurrencyCode", Namespace = "")]
            public CurrencyCode CurrencyCode { get; set; }
            [XmlElement(ElementName = "ScaleBasis", Namespace = "")]
            public ScaleUnit ScaleBasis { get; set; }
            [XmlElement(ElementName = "ScaleUnit", Namespace = "")]
            public ScaleUnit ScaleUnit { get; set; }
            [XmlElement(ElementName = "Scale", Namespace = "")]
            public List<Scale> Scale { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "InternalConditions", Namespace = "")]
        public class InternalConditions
        {
            [XmlElement(ElementName = "ConditionType", Namespace = "")]
            public ConditionType ConditionType { get; set; }
            [XmlElement(ElementName = "Description", Namespace = "")]
            public Description Description { get; set; }
            [XmlElement(ElementName = "CurrencyCode", Namespace = "")]
            public CurrencyCode CurrencyCode { get; set; }
            [XmlElement(ElementName = "Rate", Namespace = "")]
            public Rate Rate { get; set; }
            [XmlElement(ElementName = "ConditionPricingUnit", Namespace = "")]
            public ConditionPricingUnit ConditionPricingUnit { get; set; }
            [XmlElement(ElementName = "Amount", Namespace = "")]
            public Amount Amount { get; set; }
            [XmlElement(ElementName = "MaterialNumber", Namespace = "")]
            public MaterialNumber MaterialNumber { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "Material")]
        public class Material
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "ReservedQuantity")]
        public class ReservedQuantity
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "PlantDetails")]
        public class PlantDetails
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "ReserveStockDetails", Namespace = "")]
        public class ReserveStockDetails
        {
            [XmlElement(ElementName = "Material")]
            public Material Material { get; set; }
            [XmlElement(ElementName = "ReservedQuantity")]
            public ReservedQuantity ReservedQuantity { get; set; }
            [XmlElement(ElementName = "PlantDetails")]
            public PlantDetails PlantDetails { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "EANUPCCode", Namespace = "")]
        public class EANUPCCode
        {
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlText]
            public string Text { get; set; }
        }

        [XmlRoot(ElementName = "PriceAndAvailabilityResponse", Namespace = "")]
        public class PriceAndAvailabilityResponse
        {
            [XmlElement(ElementName = "ItemResponse", Namespace = "")]
            public List<ItemResponse> ItemResponse { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }

        [XmlRoot(ElementName = "ServiceResponse", Namespace = "http://www.ingrammicro.com/product/PriceAndAvailabilityResponse_v2_6/types")]
        public class ServiceResponse
        {
            [XmlElement(ElementName = "ResponsePreamble", Namespace = "")]
            public ResponsePreamble ResponsePreamble { get; set; }
            [XmlElement(ElementName = "PriceAndAvailabilityResponse", Namespace = "")]
            public PriceAndAvailabilityResponse PriceAndAvailabilityResponse { get; set; }
            [XmlAttribute(AttributeName = "ns0", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Ns0 { get; set; }
        }

        [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public class Body
        {
            [XmlElement(ElementName = "ServiceResponse", Namespace = "http://www.ingrammicro.com/product/PriceAndAvailabilityResponse_v2_6/types")]
            public ServiceResponse ServiceResponse { get; set; }
        }

        [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public class Envelope
        {
            [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
            public Header Header { get; set; }
            [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
            public Body Body { get; set; }
            [XmlAttribute(AttributeName = "SOAP-ENV", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string SOAPENV { get; set; }
        }
    }
}

#region Sample Request
/*
  https://api.ingrammicro.com/PriceAndAvailability_vse0?WSDL

 <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
  <s:Header>
    <h:ServiceRequestHeader xmlns="http://www.ingrammicro.com/common/ServiceRequestHeader_v2_2/types" xmlns:h="http://www.ingrammicro.com/common/ServiceRequestHeader_v2_2/types" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
      <ApplicationCredential xmlns="">
        <ID>APPDMZ</ID>
        <Credential>93!mZEX8t</Credential>
      </ApplicationCredential>
      <TransactionModel xmlns="">
        <SynchronousRequest>true</SynchronousRequest>
      </TransactionModel>
      <UserCredential xmlns="">
        <ID>{68f116a4-2539-4f58-a107-9370a3e2e387}</ID>
      </UserCredential>
    </h:ServiceRequestHeader>
  </s:Header>
  <s:Body xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <ServiceRequest xmlns="http://www.ingrammicro.com/product/PriceAndAvailabilityRequest_v2_6/types">
      <RequestPreamble xmlns="">
        <CustomerNumber>160142</CustomerNumber>
        <SalesOrganization>2210</SalesOrganization>

      </RequestPreamble>
      <PriceAndAvailabilityRequest xmlns="">
        <DistributionChannel>10</DistributionChannel>
        <Division>10</Division>
        <IsPricingRequired>true</IsPricingRequired>
        <IsAvailabilityRequired>true</IsAvailabilityRequired>

        <Item>
         <IngramPartNumber>3153005</IngramPartNumber>
        </Item>
        <Item>
         <IngramPartNumber>1021818</IngramPartNumber>
        </Item>

      </PriceAndAvailabilityRequest>
    </ServiceRequest>
  </s:Body>
</s:Envelope>

*/
#endregion

