using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.BusinessLogic.FinanceProductType
{
    public interface IFinanceProductTypeManager
    {
        ErrorModel SaveFinanceProductType(Web.Models.DBModel.FinanceProductType inputFinanceProductType);

        List<Web.Models.DBModel.FinanceProductType> GetFinanceProductType(bool includeInactive = false);
    }
}
