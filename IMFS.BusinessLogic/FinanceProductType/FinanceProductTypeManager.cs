using IMFS.DataAccess.Repository;
using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IMFS.BusinessLogic.FinanceProductType
{
    public class FinanceProductTypeManager : IFinanceProductTypeManager
    {
        private readonly IRepository<Web.Models.DBModel.FinanceProductType> _financeProductTypeRepository;


        public FinanceProductTypeManager(IRepository<Web.Models.DBModel.FinanceProductType> financeProductTypeRepository)
        {
            _financeProductTypeRepository = financeProductTypeRepository;
        }

        public List<Web.Models.DBModel.FinanceProductType> GetFinanceProductType(bool includeInactive = false)
        {
            var financeProductTypes = _financeProductTypeRepository.Table.ToList();
            if (!includeInactive)
            {
                financeProductTypes = financeProductTypes.Where(x => x.IsActive == true).ToList();
            }
            return financeProductTypes;
        }

        public ErrorModel SaveFinanceProductType(Web.Models.DBModel.FinanceProductType inputFinanceProductType)
        {
            var response = new ErrorModel();
            var existingFinanceProductType = _financeProductTypeRepository.GetById(inputFinanceProductType.Id);
            if (existingFinanceProductType != null)
            {
                var existingFinanceProductTypeCodeItem = _financeProductTypeRepository.Table.Where(x => x.Code == inputFinanceProductType.Code && x.Id != existingFinanceProductType.Id).FirstOrDefault();
                if (existingFinanceProductTypeCodeItem != null)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Finance Product Type " + inputFinanceProductType.Code + " already exist.";
                    return response;
                }
                existingFinanceProductType.Code = inputFinanceProductType.Code;
                existingFinanceProductType.Description = inputFinanceProductType.Description;
                existingFinanceProductType.ModifiedBy = inputFinanceProductType.ModifiedBy;
                existingFinanceProductType.ModifedDate = DateTime.Now;
                existingFinanceProductType.IsActive = inputFinanceProductType.IsActive;
                
                _financeProductTypeRepository.Update(existingFinanceProductType);

            }
            else
            {
                var existingFinanceProductTypeCodeItem = _financeProductTypeRepository.Table.Where(x => x.Code == inputFinanceProductType.Code).FirstOrDefault();
                if (existingFinanceProductTypeCodeItem != null)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Finance Product Type " + inputFinanceProductType.Code + " already exist.";
                    return response;
                }
                inputFinanceProductType.CreatedDate = DateTime.Now;
                inputFinanceProductType.ModifedDate = DateTime.Now;
                _financeProductTypeRepository.Insert(inputFinanceProductType);
            }
            return response;
        }
    }
}
