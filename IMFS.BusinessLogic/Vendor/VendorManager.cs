using IMFS.DataAccess.Repository;
using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using IMFS.Web.Models.Misc;
using System.Text;


namespace IMFS.BusinessLogic.Vendor
{
    public class VendorManager : IVendorManager
    {
        private readonly IRepository<Web.Models.DBModel.Vendor> _vendorRepository;

        public VendorManager(IRepository<Web.Models.DBModel.Vendor> vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }


        public List<Web.Models.DBModel.Vendor> GetVendor(bool includeInactive = false)
        {
            var vendors = _vendorRepository.Table.ToList();
            if (!includeInactive)
            {
                vendors = vendors.Where(x => x.Active == true).ToList();
            }
            return vendors;
        }


        public ErrorModel SaveVendor(Web.Models.DBModel.Vendor inputVendor)
        {
            var response = new ErrorModel();
            var existingVendor = _vendorRepository.GetById(inputVendor.Id);
            if (existingVendor != null)
            {
                var existingVendorCodeItem = _vendorRepository.Table.Where(x => x.VendorCode == inputVendor.VendorCode && x.Id != existingVendor.Id).FirstOrDefault();
                if (existingVendorCodeItem != null)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Vendor Code " + inputVendor.VendorCode + " already exist.";
                    return response;
                }
                existingVendor.VendorCode = inputVendor.VendorCode;
                existingVendor.VendorName = inputVendor.VendorName;
                existingVendor.SupplierNumber = inputVendor.SupplierNumber;
                existingVendor.ManufacturerNumber = inputVendor.ManufacturerNumber;
                existingVendor.SapId = inputVendor.SapId;
                existingVendor.CompanyCode = inputVendor.CompanyCode;
                existingVendor.Currency = inputVendor.Currency;
                existingVendor.Active = inputVendor.Active;
                existingVendor.LastUpdatedDate = DateTime.Now;
                _vendorRepository.Update(existingVendor);

            }
            else
            {
                var existingVendorCodeItem = _vendorRepository.Table.Where(x => x.VendorCode == inputVendor.VendorCode).FirstOrDefault();
                if (existingVendorCodeItem != null)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Vendor Code " + inputVendor.VendorCode + " already exist.";
                    return response;
                }                
                inputVendor.LastUpdatedDate = DateTime.Now;
                _vendorRepository.Insert(inputVendor);
            }
            return response;
        }
    }
}
