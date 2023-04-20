using IMFS.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMFS.BusinessLogic.FunderPlan
{
    public class FunderPlanManager : IFunderPlanManager
    {
        private readonly IRepository<Web.Models.DBModel.FunderPlan> _funderPlanRepository;

        public FunderPlanManager(IRepository<Web.Models.DBModel.FunderPlan> funderPlanRepository)
        {
            _funderPlanRepository = funderPlanRepository;
        }

        public List<Web.Models.DBModel.FunderPlan> GetFunderPlan()
        {
            var funderPlans = _funderPlanRepository.Table.ToList();            
            return funderPlans;
        }


    }
}
