using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Misc;
using IMFS.Web.Models.ResponseModel;
using System.Collections.Generic;


namespace IMFS.BusinessLogic.Vendor
{
    public interface IVendorManager
    {
        ErrorModel SaveVendor(Web.Models.DBModel.Vendor vendor);

        List<Web.Models.DBModel.Vendor> GetVendor(bool includeInactive = false);
        

    }
}
