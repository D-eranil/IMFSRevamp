using IMFS.DataAccess.Repository;
using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IMFS.BusinessLogic.Funder
{
    public class FunderManager : IFunderManager
    {
        private readonly IRepository<Web.Models.DBModel.Funder> _funderRepository;

        public FunderManager(IRepository<Web.Models.DBModel.Funder> funderRepository)
        {
            _funderRepository = funderRepository;
        }

        public List<Web.Models.DBModel.Funder> GetFunder(bool includeInactive = false)
        {
            var funders = _funderRepository.Table.ToList();
            if (!includeInactive)
            {
                funders = funders.Where(x => x.IsActive == true).ToList();
            }
            return funders;
        }

        public ErrorModel SaveFunder(Web.Models.DBModel.Funder inputFunder)
        {
            var response = new ErrorModel();
            var existingFunder = _funderRepository.GetById(inputFunder.Id);
            if (existingFunder != null)
            {
                var existingFunderCodeItem = _funderRepository.Table.Where(x => x.FunderCode == inputFunder.FunderCode && x.Id != existingFunder.Id).FirstOrDefault();
                if (existingFunderCodeItem != null)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Funder Code " + inputFunder.FunderCode + " already exist.";
                    return response;
                }
                existingFunder.FunderCode = inputFunder.FunderCode;
                existingFunder.FunderName = inputFunder.FunderName;
                existingFunder.API_URL = inputFunder.API_URL;
                existingFunder.ContactEmailAdd = inputFunder.ContactEmailAdd;
                existingFunder.IsActive = inputFunder.IsActive;
                existingFunder.ModifiedDate = DateTime.Now;
                _funderRepository.Update(existingFunder);

            }
            else
            {
                var existingFunderCodeItem = _funderRepository.Table.Where(x => x.FunderCode == inputFunder.FunderCode).FirstOrDefault();
                if (existingFunderCodeItem != null)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Funder Code " + inputFunder.FunderCode + " already exist.";
                    return response;
                }
                inputFunder.CreatedDate = DateTime.Now;
                inputFunder.ModifiedDate = DateTime.Now;
                _funderRepository.Insert(inputFunder);
            }
            return response;
        }
    }
}
