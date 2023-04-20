using IMFS.Web.Models.Misc;
using IMFS.Web.Models.Quote;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Customer
{
    public class ContactResponseModel : ErrorModel
    {
        public List<ORPContact> ContactResponse { get; set; }
        public ContactResponseModel()
        {
            ContactResponse = new List<ORPContact>();
        }
    }
}
