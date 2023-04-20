using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.BusinessLogic.FinanceType
{
    public interface IFinanceTypeManager
    {
        ErrorModel SaveFinanceType(Web.Models.DBModel.FinanceType inputFinanceType);

        List<Web.Models.DBModel.FinanceType> GetFinanceType(bool includeInactive = false);
    }
}
