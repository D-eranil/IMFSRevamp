using IMFS.Web.Models.Misc;
using IMFS.Web.Models.Quote;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Customer
{
    public class CustomerResponseModel : ErrorModel
    {
        public List<ORPCustomer> CustomerResponse { get; set; }

       
        public CustomerResponseModel( )
        {
            CustomerResponse = new List<ORPCustomer>();
        }

    }
}
