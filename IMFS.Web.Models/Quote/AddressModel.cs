using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;


namespace IMFS.Web.Models.Quote
{
    public class AddressModel : BaseEntity
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
    }
}
