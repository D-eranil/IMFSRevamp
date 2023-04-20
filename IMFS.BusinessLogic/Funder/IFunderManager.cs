using IMFS.Web.Models.Misc;
using System.Collections.Generic;

namespace IMFS.BusinessLogic.Funder
{
    public interface IFunderManager
    {
        ErrorModel SaveFunder(Web.Models.DBModel.Funder inputFunder);

        List<Web.Models.DBModel.Funder> GetFunder(bool includeInactive = false);
    }
}
