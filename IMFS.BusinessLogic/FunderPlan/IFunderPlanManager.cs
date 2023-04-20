using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.BusinessLogic.FunderPlan
{
    public interface IFunderPlanManager
    {
        List<Web.Models.DBModel.FunderPlan> GetFunderPlan();
    }
}
