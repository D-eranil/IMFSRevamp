using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Quote
{
    public class ORPCustomer
    {
        public Guid CustomerID { get; set; }
        public string DisplayLabel { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }        
        public string ABN { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }


    }
}
