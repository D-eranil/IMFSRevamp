using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Quote
{
    public class QuoteDetailsModel
    {
        public int Id { get; set; }
        public QuoteHeader QuoteHeader { get; set; }
        public CustomerDetails CustomerDetails { get; set; }
        public EndUserDetails EndUserDetails { get; set; }
        public ORPCustomer SelectedCustomer { get; set; }
        public List<QuoteLine> QuoteLines { get; set; }

        public QuoteDetailsModel()
        {
            QuoteHeader = new QuoteHeader();
            QuoteLines = new List<QuoteLine>();
            CustomerDetails = new CustomerDetails();
            EndUserDetails = new EndUserDetails();
        }
    }

    public class QuoteHeader
    {
        public string QuoteCreatedBy { get; set; }
        public string QuoteNumber { get; set; }
        public string QuoteName { get; set; }
        public int QuoteVersion { get; set; }
        public string QuoteOrigin { get; set; }
        public string QuoteType { get; set; }
        public string FinanceType { get; set; }
        public string FinanceTypeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int? Status { get; set; }
        public decimal? QuoteTotal { get; set; }
        //public string QuoteTotal { get; set; }

        public decimal? FinanceValue { get; set; }
        public string QuoteDuration { get; set; }
        public string Frequency { get; set; }
        public string FunderId { get; set; }
        public string FunderPlan { get; set; }
        //added new columns for save funder code and gst included
        public string FunderCode { get; set; }
        public int? GstInclude { get; set; }
    }

    public class CustomerDetails
    {
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerABN { get; set; }
        public string CustomerAddressLine1 { get; set; }
        public string CustomerAddressLine2 { get; set; }
        public string CustomerAddressState { get; set; }
        public string CustomerAddressCity { get; set; }
        public string CustomerPostCode { get; set; }
        public string CustomerContact { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerCountry { get; set; }
    }

    public class EndUserDetails
    {
        public string EndCustomerName { get; set; }
        public string EndCustomerABN { get; set; }
        public string EndCustomerYearsTrading { get; set; }
        public string EndCustomerAddressLine1 { get; set; }
        public string EndCustomerAddressLine2 { get; set; }
        public string EndCustomerState { get; set; }
        public string EndCustomerCity { get; set; }
        public string EndCustomerPostCode { get; set; }
        public string EndCustomerContact { get; set; }
        public string EndCustomerEmail { get; set; }
        public string EndCustomerPhone { get; set; }
        public string AuthorisedSignatoryName { get; set; }
        public string AuthorisedSignatoryPosition { get; set; }
        public string EndCustomerCountry { get; set; }
    }

    public class QuoteLine
    {
        public string IMSKU { get; set; }
        public string VPN { get; set; }
        public string Description { get; set; }
        public int? LineNumber { get; set; }
        //public string LineNumber { get; set; }
        public string VSR { get; set; }
        public string Item { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public string CategoryName { get; set; }
        public decimal? SalePrice { get; set; }
        //public string SalePrice { get; set; }

        public decimal? CostPrice { get; set; }
        //public string CostPrice { get; set; }
        public int Qty { get; set; }
        //public string Qty { get; set; }

        public decimal? Margin { get; set; }
        //public string Margin { get; set; }
        public decimal? LineTotal { get; set; }
        //public string LineTotal { get; set; }

        public decimal? TotalGST { get; set; }
        public int? FinanceProductTypeID { get; set; }
    }

    public class RejectQuoteDetail
    {
        public string QuoteId { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
    }


}
