using CsvHelper;
using CsvHelper.Configuration;
using IMFS.BusinessLogic.Utility;
using IMFS.Core;
using IMFS.DataAccess.Repository;
using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Enums;
using IMFS.Web.Models.Misc;
using IMFS.Web.Models.QuotePercentRate;
using IMFS.Web.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IMFS.BusinessLogic.QuotePercentRate
{
    public class QuotePercentRateManager : IQuotePercentRateManager
    {

        private readonly IRepository<Web.Models.DBModel.QuoteBreakPercentRate> _quoteBreakPercentRateRepository;

        private readonly IRepository<Types> _typesRepository;

        private readonly IRepository<Categories> _categoriesRepository;

        private readonly IRepository<Web.Models.DBModel.Funder> _fundersRepository;
        private readonly IRepository<Web.Models.DBModel.FunderPlan> _funderPlanRepository;

        private readonly IRepository<Web.Models.DBModel.FinanceType> _financeTypeRepository;

        private readonly IRepository<Web.Models.DBModel.FinanceProductType> _financeProductTypeRepository;

        private readonly IRepository<Web.Models.DBModel.Vendor> _vendorsRepository;

        private readonly IRepository<Web.Models.DBModel.Product> _productsRepository;

        public QuotePercentRateManager(IRepository<Web.Models.DBModel.QuoteBreakPercentRate> quoteBreakRateRepository,
            IRepository<Types> typesRepository,
            IRepository<Categories> categoriesRepository,
            IRepository<Web.Models.DBModel.Funder> funderRepository,
            IRepository<Web.Models.DBModel.Vendor> vendorRepository,
            IRepository<Web.Models.DBModel.Product> productsRepository,
            IRepository<Web.Models.DBModel.FinanceType> financeTypeRepository,
            IRepository<Web.Models.DBModel.FinanceProductType> financeProductTypeRepository,
            IRepository<Web.Models.DBModel.FunderPlan> funderPlanRepository
            )
        {
            _quoteBreakPercentRateRepository = quoteBreakRateRepository;
            _typesRepository = typesRepository;
            _categoriesRepository = categoriesRepository;
            _fundersRepository = funderRepository;
            _vendorsRepository = vendorRepository;
            _productsRepository = productsRepository;
            _financeTypeRepository = financeTypeRepository;
            _financeProductTypeRepository = financeProductTypeRepository;
            _funderPlanRepository = funderPlanRepository;
        }

        public DownloadResponse ExportRates(QuotePercentRateModel inputModel)
        {
            var response = new DownloadResponse();
            var rates = GetRates(inputModel);
            StringBuilder fileText = new StringBuilder(1024 * 1024);// 1MB

            var funderCode = _fundersRepository.Table.Where(f => f.Id == inputModel.FunderId).FirstOrDefault().FunderCode;
            var financeTypeDescr = _financeTypeRepository.Table.Where(f => f.Id == inputModel.FinanceType).FirstOrDefault().Description;
            var productTypeDescr = _financeProductTypeRepository.Table.Where(f => f.Id == inputModel.ProductType).FirstOrDefault().Description;
            var funderPlan = _funderPlanRepository.Table.Where(f => f.PlanId == inputModel.FunderPlan).FirstOrDefault().PlanDescription;

            // write header
            fileText.Append(Tools.AppendCSV("MinPercent"));
            fileText.Append(Tools.AppendCSV("MaxPercent"));

            fileText.Append(Tools.AppendCSV("12 months monthly"));
            fileText.Append(Tools.AppendCSV("12 months quarterly"));
            fileText.Append(Tools.AppendCSV("12 months upfront"));
            fileText.Append(Tools.AppendCSV("24 months monthly"));
            fileText.Append(Tools.AppendCSV("24 months quarterly"));
            fileText.Append(Tools.AppendCSV("24 months upfront"));
            fileText.Append(Tools.AppendCSV("36 months monthly"));
            fileText.Append(Tools.AppendCSV("36 months quarterly"));
            fileText.Append(Tools.AppendCSV("36 months upfront"));
            fileText.Append(Tools.AppendCSV("48 months monthly"));
            fileText.Append(Tools.AppendCSV("48 months quarterly"));
            fileText.Append(Tools.AppendCSV("48 months upfront"));
            fileText.Append(Tools.AppendCSV("60 months monthly"));
            fileText.Append(Tools.AppendCSV("60 months quarterly"));
            fileText.AppendLine("60 months upfront");   // last header column

            foreach (var rate in rates)
            {
                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.MinPercent)));
                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.MaxPercent)));

                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months12Monthly)));
                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months12Quarterly)));
                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months12Upfront)));

                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months24Monthly)));
                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months24Quarterly)));
                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months24Upfront)));

                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months36Monthly)));
                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months36Quarterly)));
                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months36Upfront)));

                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months48Monthly)));
                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months48Quarterly)));
                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months48Upfront)));

                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months60Monthly)));
                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.months60Quarterly)));
                fileText.AppendLine(Tools.AppendCSV(Convert.ToString(rate.months60Upfront)));// last item column

            }

            response.DownloadFile = Encoding.ASCII.GetBytes(fileText.ToString());
            response.FileName = funderCode + " " + funderPlan + " " + productTypeDescr + " " + financeTypeDescr + " Quote Percent Rate Export.csv";
            return response;
        }

        public List<QuotePercentRateResponse> GetRates(QuotePercentRateModel inputModel)
        {
            List<QuotePercentRateResponse> quoteRates = new List<QuotePercentRateResponse>();
            List<QuotePercentRateResponse> updatedTypes = new List<QuotePercentRateResponse>();

            try
            {
                var financeType = (from fu in _financeTypeRepository.Table where fu.QuoteDurationType == inputModel.FinanceType select fu).FirstOrDefault().Description;

                quoteRates = (from qr in _quoteBreakPercentRateRepository.Table

                              where (
                              (inputModel.FunderId != 0 && inputModel.FunderId != null ? qr.FunderID == inputModel.FunderId : 1 == 1)
                              && (inputModel.FunderPlan != 0 && inputModel.FunderPlan != null ? qr.FunderPlanID == inputModel.FunderPlan : 1 == 1)
                              && (inputModel.FinanceType != 0 && inputModel.FinanceType != null ? qr.FinanceType == inputModel.FinanceType : 1 == 1)
                              && (inputModel.ProductType != 0 && inputModel.ProductType != null ? qr.ProductType == inputModel.ProductType : 1 == 1)
                              )
                              select new QuotePercentRateResponse
                              {
                                  Value = qr.Value,
                                  MinPercent = (double)qr.MinPercent,
                                  MaxPercent = (double)qr.MaxPercent,
                                  QuoteDurationID = qr.QuoteDurationID,
                                  FinanceType = qr.FinanceType,
                                  ProductType = qr.ProductType,
                                  PaymentType = qr.PaymentType,
                                  FunderID = qr.FunderID,
                                  FunderPlanID = qr.FunderPlanID
                              }).OrderBy(x => x.MinPercent).ToList();

                foreach (var item in quoteRates)
                {

                    if (updatedTypes.Where(r => r.FunderID == item.FunderID 
                    && r.FunderPlanID == item.FunderPlanID
                    && r.MinPercent == item.MinPercent && r.MaxPercent == item.MaxPercent).Count() > 0)
                    {
                        //Then update other fields, otherwise add a new item
                        var oldItem = updatedTypes.Where(r => r.FunderID == item.FunderID
                        && r.FunderPlanID == item.FunderPlanID
                        && r.MinPercent == item.MinPercent
                        && r.MaxPercent == item.MaxPercent).FirstOrDefault();
                        //For years 1,2,3,4,5
                        GetRateValue(item, oldItem);
                    }
                    else
                    {
                        var newItem = new QuotePercentRateResponse();

                        newItem.PaymentType = item.PaymentType;
                        newItem.FinanceTypeDescr = financeType;
                        newItem.FunderID = item.FunderID;
                        newItem.FunderPlanID = item.FunderPlanID;
                        newItem.MinPercent = item.MinPercent;
                        newItem.MaxPercent = item.MaxPercent;
                        newItem.FinanceType = item.FinanceType;
                        newItem.ProductType = item.ProductType;

                        //For years 1,2,3,4,5
                        GetRateValue(item, newItem);

                        updatedTypes.Add(newItem);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return updatedTypes;
        }

        /// <summary>
        /// this updates the value for the year like 12 months, 24 months etc
        /// </summary>
        /// <param name="item"></param>
        /// <param name="newItem"></param>
        private static void GetRateValue(QuotePercentRateResponse item, QuotePercentRateResponse newItem)
        {
            IMFSEnums.PaymentType paymentType;
            var payment = Enum.TryParse(item.PaymentType.ToLower().Trim(), out paymentType);

            if (item.QuoteDurationID == 1)
            {
                switch (paymentType)
                {
                    case IMFSEnums.PaymentType.monthly:
                        {
                            newItem.months12Monthly = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }
                    case IMFSEnums.PaymentType.quarterly:
                        {
                            newItem.months12Quarterly = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }

                    case IMFSEnums.PaymentType.upfront:
                        {
                            newItem.months12Upfront = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }
                }

            }
            if (item.QuoteDurationID == 2)
            {
                switch (paymentType)
                {
                    case IMFSEnums.PaymentType.monthly:
                        {
                            newItem.months24Monthly = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }
                    case IMFSEnums.PaymentType.quarterly:
                        {
                            newItem.months24Quarterly = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }

                    case IMFSEnums.PaymentType.upfront:
                        {
                            newItem.months24Upfront = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }
                }
            }

            if (item.QuoteDurationID == 3)
            {
                switch (paymentType)
                {
                    case IMFSEnums.PaymentType.monthly:
                        {
                            newItem.months36Monthly = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }
                    case IMFSEnums.PaymentType.quarterly:
                        {
                            newItem.months36Quarterly = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }

                    case IMFSEnums.PaymentType.upfront:
                        {
                            newItem.months36Upfront = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }
                }
            }

            if (item.QuoteDurationID == 4)
            {
                switch (paymentType)
                {
                    case IMFSEnums.PaymentType.monthly:
                        {
                            newItem.months48Monthly = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }
                    case IMFSEnums.PaymentType.quarterly:
                        {
                            newItem.months48Quarterly = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }

                    case IMFSEnums.PaymentType.upfront:
                        {
                            newItem.months48Upfront = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }
                }
            }

            if (item.QuoteDurationID == 5)
            {
                switch (paymentType)
                {
                    case IMFSEnums.PaymentType.monthly:
                        {
                            newItem.months60Monthly = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }
                    case IMFSEnums.PaymentType.quarterly:
                        {
                            newItem.months60Quarterly = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }

                    case IMFSEnums.PaymentType.upfront:
                        {
                            newItem.months60Upfront = double.Parse(item.Value.ToString("0.00000"));
                            break;
                        }
                }
            }

        }

        public ErrorModel SaveRate(QuotePercentRateResponse quotePercentRateResponse)
        {
            var response = new ErrorModel();
            var dbEntityList = new List<QuoteBreakPercentRate>();

            try
            {
                foreach (IMFSEnums.QuoteDuration serviceDuration in Enum.GetValues(typeof(IMFSEnums.QuoteDuration)))
                {
                    var dbEntity = new QuoteBreakPercentRate();
                    dbEntity.FunderID = quotePercentRateResponse.FunderID;
                    dbEntity.FunderPlanID = quotePercentRateResponse.FunderPlanID;
                    dbEntity.MinPercent = quotePercentRateResponse.MinPercent;
                    dbEntity.MaxPercent = quotePercentRateResponse.MaxPercent;
                    dbEntity.FinanceType = quotePercentRateResponse.FinanceType;
                    dbEntity.ProductType = quotePercentRateResponse.ProductType;

                    switch ((int)serviceDuration)
                    {
                        case 12:
                            AddItem(1, "Monthly", quotePercentRateResponse.months12Monthly, dbEntityList, dbEntity);
                            AddItem(1, "Quarterly", quotePercentRateResponse.months12Quarterly, dbEntityList, dbEntity);
                            AddItem(1, "Upfront", quotePercentRateResponse.months12Upfront, dbEntityList, dbEntity);

                            break;
                        case 24:
                            AddItem(2, "Monthly", quotePercentRateResponse.months24Monthly, dbEntityList, dbEntity);
                            AddItem(2, "Quarterly", quotePercentRateResponse.months24Quarterly, dbEntityList, dbEntity);
                            AddItem(2, "Upfront", quotePercentRateResponse.months24Upfront, dbEntityList, dbEntity);

                            break;
                        case 36:
                            AddItem(3, "Monthly", quotePercentRateResponse.months36Monthly, dbEntityList, dbEntity);
                            AddItem(3, "Quarterly", quotePercentRateResponse.months36Quarterly, dbEntityList, dbEntity);
                            AddItem(3, "Upfront", quotePercentRateResponse.months36Upfront, dbEntityList, dbEntity);

                            break;
                        case 48:
                            AddItem(4, "Monthly", quotePercentRateResponse.months48Monthly, dbEntityList, dbEntity);
                            AddItem(4, "Quarterly", quotePercentRateResponse.months48Quarterly, dbEntityList, dbEntity);
                            AddItem(4, "Upfront", quotePercentRateResponse.months48Upfront, dbEntityList, dbEntity);

                            break;
                        case 60:
                            AddItem(5, "Monthly", quotePercentRateResponse.months60Monthly, dbEntityList, dbEntity);
                            AddItem(5, "Quarterly", quotePercentRateResponse.months60Quarterly, dbEntityList, dbEntity);
                            AddItem(5, "Upfront", quotePercentRateResponse.months60Upfront, dbEntityList, dbEntity);

                            break;

                    }
                }

                foreach (var dbEntity in dbEntityList)
                {
                    var existingQuoteRate = _quoteBreakPercentRateRepository.Table.
                        Where(x => x.MinPercent == dbEntity.MinPercent && x.MaxPercent == dbEntity.MaxPercent &&
                        x.FunderID == dbEntity.FunderID &&
                        x.FunderPlanID == dbEntity.FunderPlanID &&
                        x.QuoteDurationID == dbEntity.QuoteDurationID &&
                        x.FinanceType == dbEntity.FinanceType &&
                        x.ProductType == dbEntity.ProductType &&
                        x.PaymentType == dbEntity.PaymentType).FirstOrDefault();

                    if (existingQuoteRate != null)
                    {
                        existingQuoteRate.Value = dbEntity.Value;
                        existingQuoteRate.ModifiedDate = DateTime.Now.ToLocalTime();

                        _quoteBreakPercentRateRepository.Update(dbEntity);
                    }
                    else
                    {
                        _quoteBreakPercentRateRepository.Insert(dbEntity);
                    }
                }

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        private static void AddItem(int quoteDurationId, string paymentType, double? value, List<QuoteBreakPercentRate> dbEntityList, QuoteBreakPercentRate dbEntity)
        {
            if (value != null)
            {
                var clonedEntity = Helper.DeepClone(dbEntity);
                clonedEntity.QuoteDurationID = quoteDurationId;
                clonedEntity.PaymentType = paymentType;
                clonedEntity.Value = (double)value;
                clonedEntity.ModifiedDate = DateTime.Now.ToLocalTime();
                dbEntityList.Add(clonedEntity);
            }
        }

        public List<ErrorModel> UploadRate(IFormFile file, string funder, string productType, string financeType, string funderPlan)
        {
            var result = readCsv(file, funder, productType, financeType, funderPlan);

            return result;
        }


        private List<ErrorModel> readCsv(IFormFile file, string funder, string productType, string financeType, string funderPlan)
        {
            var errorList = new List<ErrorModel>();
            try
            {
                long length = file.Length;
                if (length < 0)
                {
                    errorList.Add(new ErrorModel() { HasError = true, ErrorMessage = "File is empty" });
                }

                using var fileStream = file.OpenReadStream();
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    PrepareHeaderForMatch = args => Regex.Replace(args.Header, @"\s", string.Empty)
                };
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, config))
                {

                    csv.Context.RegisterClassMap<PercentRateInputExcelModelMap>();

                    var records = csv.GetRecords<QuotePercentRateInputExcelModel>();

                    var dbEntityList = new List<QuoteBreakPercentRate>();

                    if (records != null)
                    {
                        int counterLine = 2;//First line is for headers
                        foreach (var record in records)
                        {
                            var st = record;
                            var error = insertQuoteRate(record, dbEntityList, errorList, funder, productType, financeType, counterLine, funderPlan);
                            if (error.HasError)
                            {
                                errorList.Add(error);
                            }
                            counterLine++;
                        }
                    }

                    //Process Records in DB if there is no error in records
                    if (errorList.Count == 0)
                    {
                        foreach (var dbEntity in dbEntityList)
                        {

                            var existingQuoteRate = _quoteBreakPercentRateRepository.Table.
                        Where(x => x.MinPercent == dbEntity.MinPercent && x.MaxPercent == dbEntity.MaxPercent &&
                        x.FunderID == dbEntity.FunderID &&
                        x.FunderPlanID == dbEntity.FunderPlanID &&
                        x.QuoteDurationID == dbEntity.QuoteDurationID &&
                        x.FinanceType == dbEntity.FinanceType &&
                        x.ProductType == dbEntity.ProductType &&
                        x.PaymentType == dbEntity.PaymentType).FirstOrDefault();

                            if (existingQuoteRate != null)
                            {
                                existingQuoteRate.Value = dbEntity.Value;
                                existingQuoteRate.ModifiedDate = DateTime.Now.ToLocalTime();

                                _quoteBreakPercentRateRepository.Update(dbEntity);
                            }
                            else
                            {
                                _quoteBreakPercentRateRepository.Insert(dbEntity);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                errorList.Add(new ErrorModel() { HasError = true, ErrorMessage = ex.Message });
            }
            return errorList;
        }


        private ErrorModel insertQuoteRate(QuotePercentRateInputExcelModel data, List<QuoteBreakPercentRate> dbEntityList, List<ErrorModel> errorList, 
            string funder, string productType, string financeType, int counterLine, string funderPlan)
        {
            var errorModel = new ErrorModel();
            try
            {
                int counter = 1;
                foreach (IMFSEnums.QuoteDuration serviceDuration in Enum.GetValues(typeof(IMFSEnums.QuoteDuration)))
                {
                    var dbEntity = new QuoteBreakPercentRate();
                    dbEntity.MinPercent = data.MinPercent;
                    dbEntity.MaxPercent = data.MaxPercent;
                    dbEntity.FinanceType = Convert.ToInt32(financeType); //will come from client side for , table Finance Type : Leasing : 1, Rental:2, Instalment:4
                    dbEntity.FunderID = Convert.ToInt32(funder);
                    dbEntity.ProductType = Convert.ToInt32(productType);
                    dbEntity.ModifiedDate = DateTime.Now.ToLocalTime();
                    dbEntity.FunderPlanID = Convert.ToInt32(funderPlan);

                    switch ((int)serviceDuration)
                    {
                        case 12:

                            AddItem(1, "Monthly", data.months12Monthly, dbEntityList, dbEntity);
                            AddItem(1, "Quarterly", data.months12Quarterly, dbEntityList, dbEntity);
                            AddItem(1, "Upfront", data.months12Upfront, dbEntityList, dbEntity);
                            break;
                        case 24:

                            AddItem(2, "Monthly", data.months24Monthly, dbEntityList, dbEntity);
                            AddItem(2, "Quarterly", data.months24Quarterly, dbEntityList, dbEntity);
                            AddItem(2, "Upfront", data.months24Upfront, dbEntityList, dbEntity);
                            break;
                        case 36:

                            AddItem(3, "Monthly", data.months36Monthly, dbEntityList, dbEntity);
                            AddItem(3, "Quarterly", data.months36Quarterly, dbEntityList, dbEntity);
                            AddItem(3, "Upfront", data.months36Upfront, dbEntityList, dbEntity);
                            break;
                        case 48:

                            AddItem(4, "Monthly", data.months48Monthly, dbEntityList, dbEntity);
                            AddItem(4, "Quarterly", data.months48Quarterly, dbEntityList, dbEntity);
                            AddItem(4, "Upfront", data.months48Upfront, dbEntityList, dbEntity);
                            break;
                        case 60:

                            AddItem(5, "Monthly", data.months60Monthly, dbEntityList, dbEntity);
                            AddItem(5, "Quarterly", data.months60Quarterly, dbEntityList, dbEntity);
                            AddItem(5, "Upfront", data.months60Upfront, dbEntityList, dbEntity);
                            break;
                    }

                    counter++;
                }
            }
            catch (Exception ex)
            {
                errorModel.HasError = true;
                errorModel.ErrorMessage = JsonConvert.SerializeObject(ex);
            }
            return errorModel;
        }
    }
}
