using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Product
{
    public class ProductAutocomplete
    {
        public string ProductID
        {
            get; set;
        }
        public string SKU
        {
            get; set;
        }
        public string VPN
        {
            get; set;
        }
        public string PurchasingBlock
        {
            get; set;
        }
        public string SalesBlock
        {
            get; set;
        }
        public string Description
        {
            get; set;
        }
        public string DisplayLabel
        {
            get; set;
        }
    }
}
