using IMFS.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Services.IMWebServices
{
    public interface IIMWebServicesManager
    {
        ProductSAPConfig _productSAPConfig { get; set; }
        ProductSAPEnquiryOutput ProductSAPEnquiry(string resellerNumber, List<ProductSAPInputItem> items);
    }
}
