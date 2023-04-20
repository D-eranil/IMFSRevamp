using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("QuoteLines")]
    public class QuoteLines : BaseEntity
    {
        public int Id { get; set; }
        public Guid UniqueKey { get; set; }
        public int? QuoteLineNumber { get; set; }
        public int QuoteId { get; set; }
        public int? TypeID { get; set; }
        public int? CategoryID { get; set; }
        public string SKU { get; set; }
        public string VPN { get; set; }
        public int Version { get; set; }
        public decimal? ResellerSellPrice { get; set; }
        public decimal? IngramSellPrice { get; set; }
        public double? ResellerMarginPercent { get; set; }
        public decimal? ResellerMarginAmount { get; set; }
        public double? CommissionPercent { get; set; }
        public double? CommissionAmount { get; set; }
        public int? Qty { get; set; }
        public decimal? LineTotal { get; set; }
        public decimal? LineGST { get; set; }
        public string Description { get; set; }
        public int? FinanceProductTypeID { get; set; }

    }
}
