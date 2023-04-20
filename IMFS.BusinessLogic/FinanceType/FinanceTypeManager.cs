using IMFS.DataAccess.Repository;
using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMFS.BusinessLogic.FinanceType
{
    public class FinanceTypeManager : IFinanceTypeManager
    {

        private readonly IRepository<Web.Models.DBModel.FinanceType> _financeTypeRepository;

        public FinanceTypeManager(IRepository<Web.Models.DBModel.FinanceType> financeTypeRepository)
        {
            _financeTypeRepository = financeTypeRepository;
        }


        public List<Web.Models.DBModel.FinanceType> GetFinanceType(bool includeInactive = false)
        {
            var financeTypes = _financeTypeRepository.Table.ToList();
            if (!includeInactive)
            {
                financeTypes = financeTypes.Where(x => x.IsActive == true).ToList();
            }
            return financeTypes;
        }

       

        public ErrorModel SaveFinanceType(Web.Models.DBModel.FinanceType inputFinanceType)
        {
            var response = new ErrorModel();
            var existingFinanceType = _financeTypeRepository.GetById(inputFinanceType.Id);
            if (existingFinanceType != null)
            {
                var existingFinanceTypeCodeItem = _financeTypeRepository.Table.Where(x => x.QuoteDurationType == inputFinanceType.QuoteDurationType && x.Id != existingFinanceType.Id).FirstOrDefault();
                if (existingFinanceTypeCodeItem != null)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Finance Type " + inputFinanceType.QuoteDurationType + " already exist.";
                    return response;
                }
                existingFinanceType.QuoteDurationType = inputFinanceType.QuoteDurationType;
                existingFinanceType.Description = inputFinanceType.Description;               
                existingFinanceType.ModifiedDate = DateTime.Now;
                existingFinanceType.IsActive = inputFinanceType.IsActive;

                //Update in the repo
                _financeTypeRepository.Update(existingFinanceType);

            }
            else
            {
                var existingFinanceTypeCodeItem = _financeTypeRepository.Table.Where(x => x.QuoteDurationType == inputFinanceType.QuoteDurationType).FirstOrDefault();
                if (existingFinanceTypeCodeItem != null)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Finance Type " + inputFinanceType.QuoteDurationType + " already exist.";
                    return response;
                }
                inputFinanceType.CreatedDate = DateTime.Now;
                inputFinanceType.ModifiedDate = DateTime.Now;

                //Insert record in the repo
                _financeTypeRepository.Insert(inputFinanceType);
            }
            return response;
        }
    }
}
