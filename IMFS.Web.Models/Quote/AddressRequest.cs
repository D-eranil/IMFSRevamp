using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Quote
{
    public class AddressRequest : BaseEntity
    {
        public Boolean geoCodeEnable { get; set; }
        public Boolean skipAddressLookup { get; set; }
        public AddressRequestInfo addressInfo { get; set; }
    }

    public class AddressRequestInfo
    {
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string companyName { get; set; }
        public string country { get; set; }
        public string postalCode { get; set; }
        public string state { get; set; }
    }
}