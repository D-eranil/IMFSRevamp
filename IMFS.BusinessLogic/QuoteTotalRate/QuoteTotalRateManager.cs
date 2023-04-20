using CsvHelper;
using CsvHelper.Configuration;
using IMFS.BusinessLogic.Utility;
using IMFS.Core;
using IMFS.DataAccess.Repository;
using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Enums;
using IMFS.Web.Models.Misc;
using IMFS.Web.Models.QuoteTotalRate;
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

namespace IMFS.BusinessLogic.QuoteTotalRate
{
    public class QuoteTotalRateManager: IQuoteTotalRateManager
    {
        private readonly IRepository<Web.Models.DBModel.QuoteBreakTotalRate> _quoteBreakTotalRateRepository;

        private readonly IRepository<Types> _typesRepository;

        private readonly IRepository<Categories> _categoriesRepository;

        private readonly IRepository<Web.Models.DBModel.Funder> _fundersRepository;
        private readonly IRepository<Web.Models.DBModel.FunderPlan> _funderPlanRepository;

        private readonly IRepository<Web.Models.DBModel.FinanceType> _financeTypeRepository;

        private readonly IRepository<Web.Models.DBModel.FinanceProductType> _financeProductTypeRepository;

        private readonly IRepository<Web.Models.DBModel.Vendor> _vendorsRepository;

        private readonly IRepository<Web.Models.DBModel.Product> _productsRepository;

        public QuoteTotalRateManager(IRepository<Web.Models.DBModel.QuoteBreakTotalRate> quoteBreakRateRepository,
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
            _quoteBreakTotalRateRepository = quoteBreakRateRepository;
            _typesRepository = typesRepository;
            _categoriesRepository = categoriesRepository;
            _fundersRepository = funderRepository;
            _vendorsRepository = vendorRepository;
            _productsRepository = productsRepository;
            _financeTypeRepository = financeTypeRepository;
            _financeProductTypeRepository = financeProductTypeRepository;
            _funderPlanRepository = funderPlanRepository;

        }


        public List<QuoteTotalRateResponse> GetRates(QuoteTotalRateModel inputModel)
        {

            List<QuoteTotalRateResponse> quoteRates = new List<QuoteTotalRateResponse>();
            List<QuoteTotalRateResponse> updatedTypes = new List<QuoteTotalRateResponse>();

            try
            {
                var financeType = (from fu in _financeTypeRepository.Table where fu.QuoteDurationType == inputModel.FinanceType select fu).FirstOrDefault().Description;

                quoteRates = (from qr in _quoteBreakTotalRateRepository.Table
                              
                              where (
                              (inputModel.FunderId != 0 && inputModel.FunderId != null ? qr.FunderID == inputModel.FunderId : 1 == 1)
                              && (inputModel.FunderPlan != 0 && inputModel.FunderPlan != null ? qr.FunderPlanID == inputModel.FunderPlan : 1 == 1)
                              && (inputModel.FinanceType != 0 && inputModel.FinanceType != null ? qr.FinanceType == inputModel.FinanceType : 1 == 1)                              
                              )
                              select new QuoteTotalRateResponse
                              {
                                  Value = qr.Value,
                                  Min = (double)qr.Min,
                                  Max = (double)qr.Max,
                                  QuoteDurationID = qr.QuoteDurationID,
                                  FinanceType = qr.FinanceType,
                                  PaymentType = qr.PaymentType,
                                  FunderID = qr.FunderID,
                                  FunderPlanID = qr.FunderPlanID                                   
                              }).OrderBy(x => x.Min).ToList();

                foreach (var item in quoteRates)
                {

                    if (updatedTypes.Where(r => r.FunderID == item.FunderID
                        && r.FunderPlanID == item.FunderPlanID
                        && r.Min == item.Min
                        && r.Max == item.Max).Count() > 0)
                    {
                        //Then update other fields, otherwise add a new item
                        var oldItem = updatedTypes.Where(r => r.FunderID == item.FunderID
                        && r.FunderPlanID == item.FunderPlanID
                        && r.Min == item.Min
                        && r.Max == item.Max).FirstOrDefault();
                        //For years 1,2,3,4,5
                        GetRateValue(item, oldItem);
                    }
                    else
                    {
                        var newItem = new QuoteTotalRateResponse();

                        newItem.PaymentType = item.PaymentType;
                        newItem.FinanceTypeDescr = financeType;
                        newItem.FunderID = item.FunderID;
                        newItem.Min = item.Min;
                        newItem.Max = item.Max;
                        newItem.FinanceType = item.FinanceType;
                        newItem.FunderPlanID = item.FunderPlanID;
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
        private static void GetRateValue(QuoteTotalRateResponse item, QuoteTotalRateResponse newItem)
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

       

        public ErrorModel SaveRate(QuoteTotalRateResponse quoteTotalRateResponse)
        {
            var response = new ErrorModel();
            var dbEntityList = new List<QuoteBreakTotalRate>();

            try
            {
                foreach (IMFSEnums.QuoteDuration serviceDuration in Enum.GetValues(typeof(IMFSEnums.QuoteDuration)))
                {
                    var dbEntity = new QuoteBreakTotalRate();
                    dbEntity.FunderID = quoteTotalRateResponse.FunderID;
                    dbEntity.Min = quoteTotalRateResponse.Min;
                    dbEntity.Max = quoteTotalRateResponse.Max;
                    dbEntity.FinanceType = quoteTotalRateResponse.FinanceType;
                    dbEntity.FunderPlanID = quoteTotalRateResponse.FunderPlanID;

                    switch ((int)serviceDuration)
                    {
                        case 12:
                            AddItem(1, "Monthly", quoteTotalRateResponse.months12Monthly, dbEntityList, dbEntity);
                            AddItem(1, "Quarterly", quoteTotalRateResponse.months12Quarterly, dbEntityList, dbEntity);
                            AddItem(1, "Upfront", quoteTotalRateResponse.months12Upfront, dbEntityList, dbEntity);

                            break;
                        case 24:
                            AddItem(2, "Monthly", quoteTotalRateResponse.months24Monthly, dbEntityList, dbEntity);
                            AddItem(2, "Quarterly", quoteTotalRateResponse.months24Quarterly, dbEntityList, dbEntity);
                            AddItem(2, "Upfront", quoteTotalRateResponse.months24Upfront, dbEntityList, dbEntity);

                            break;
                        case 36:
                            AddItem(3, "Monthly", quoteTotalRateResponse.months36Monthly, dbEntityList, dbEntity);
                            AddItem(3, "Quarterly", quoteTotalRateResponse.months36Quarterly, dbEntityList, dbEntity);
                            AddItem(3, "Upfront", quoteTotalRateResponse.months36Upfront, dbEntityList, dbEntity);

                            break;
                        case 48:
                            AddItem(4, "Monthly", quoteTotalRateResponse.months48Monthly, dbEntityList, dbEntity);
                            AddItem(4, "Quarterly", quoteTotalRateResponse.months48Quarterly, dbEntityList, dbEntity);
                            AddItem(4, "Upfront", quoteTotalRateResponse.months48Upfront, dbEntityList, dbEntity);

                            break;
                        case 60:
                            AddItem(5, "Monthly", quoteTotalRateResponse.months60Monthly, dbEntityList, dbEntity);
                            AddItem(5, "Quarterly", quoteTotalRateResponse.months60Quarterly, dbEntityList, dbEntity);
                            AddItem(5, "Upfront", quoteTotalRateResponse.months60Upfront, dbEntityList, dbEntity);

                            break;

                    }
                }

                foreach (var dbEntity in dbEntityList)
                {
                    var existingQuoteRate = _quoteBreakTotalRateRepository.Table.
                        Where(x => x.Min == dbEntity.Min && x.Max == dbEntity.Max &&
                        x.FunderID == dbEntity.FunderID &&
                        x.FunderPlanID == dbEntity.FunderPlanID &&
                        x.QuoteDurationID == dbEntity.QuoteDurationID &&
                        x.FinanceType == dbEntity.FinanceType &&
                        x.PaymentType == dbEntity.PaymentType).FirstOrDefault();

                    if (existingQuoteRate != null)
                    {
                        existingQuoteRate.Value = dbEntity.Value;
                        existingQuoteRate.ModifiedDate = DateTime.Now.ToLocalTime();

                        _quoteBreakTotalRateRepository.Update(dbEntity);
                    }
                    else
                    {
                        _quoteBreakTotalRateRepository.Insert(dbEntity);
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

        private static void AddItem(int quoteDurationId, string paymentType, double? value, List<QuoteBreakTotalRate> dbEntityList, QuoteBreakTotalRate dbEntity)
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


        public DownloadResponse ExportRates(QuoteTotalRateModel inputModel)
        {
            var response = new DownloadResponse();
            var rates = GetRates(inputModel);
            StringBuilder fileText = new StringBuilder(1024 * 1024);// 1MB

            var funderCode = _fundersRepository.Table.Where(f => f.Id == inputModel.FunderId).FirstOrDefault().FunderCode;
            var financeTypeDescr = _financeTypeRepository.Table.Where(f => f.Id == inputModel.FinanceType).FirstOrDefault().Description;
            var funderPlan = _funderPlanRepository.Table.Where(f => f.PlanId == inputModel.FunderPlan).FirstOrDefault().PlanDescription;

            // write header
            fileText.Append(Tools.AppendCSV("Min"));
            fileText.Append(Tools.AppendCSV("Max"));           
            
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
                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.Min)));
                fileText.Append(Tools.AppendCSV(Convert.ToString(rate.Max)));
                
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
            response.FileName = funderCode + " " + funderPlan + " " + financeTypeDescr + " Quote Total Rate Export.csv";
            return response;
        }

        public List<ErrorModel> UploadRate(IFormFile file, string funder, string financeType, string funderPlan)
        {
            var result = readCsv(file, funder, financeType, funderPlan);

            return result;
        }


        private List<ErrorModel> readCsv(IFormFile file, string funder, string financeType, string funderPlan)
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

                    csv.Context.RegisterClassMap<TotalRateInputExcelModelMap>();

                    var records = csv.GetRecords<QuoteTotalRateInputExcelModel>();

                    var dbEntityList = new List<QuoteBreakTotalRate>();

                    if (records != null)
                    {
                        int counterLine = 2;//First line is for headers
                        foreach (var record in records)
                        {
                            var st = record;
                            var error = insertQuoteRate(record, dbEntityList, errorList, funder, financeType, counterLine, funderPlan);
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
                            
                            var existingQuoteRate = _quoteBreakTotalRateRepository.Table.
                        Where(x => x.Min == dbEntity.Min && x.Max == dbEntity.Max &&
                        x.FunderID == dbEntity.FunderID &&
                        x.FunderPlanID == dbEntity.FunderPlanID &&
                        x.QuoteDurationID == dbEntity.QuoteDurationID &&
                        x.FinanceType == dbEntity.FinanceType &&
                        x.PaymentType == dbEntity.PaymentType).FirstOrDefault();

                            if (existingQuoteRate != null)
                            {
                                existingQuoteRate.Value = dbEntity.Value;
                                existingQuoteRate.ModifiedDate = DateTime.Now.ToLocalTime();

                                _quoteBreakTotalRateRepository.Update(dbEntity);
                            }
                            else
                            {
                                _quoteBreakTotalRateRepository.Insert(dbEntity);
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


        private ErrorModel insertQuoteRate(QuoteTotalRateInputExcelModel data, List<QuoteBreakTotalRate> dbEntityList, List<ErrorModel> errorList, 
            string funder, string financeType, int counterLine, string funderPlan)
        {
            var errorModel = new ErrorModel();
            try
            {
                int counter = 1;
                foreach (IMFSEnums.QuoteDuration serviceDuration in Enum.GetValues(typeof(IMFSEnums.QuoteDuration)))
                {
                    var dbEntity = new QuoteBreakTotalRate();
                    dbEntity.Min = data.Min;
                    dbEntity.Max = data.Max;
                    dbEntity.FinanceType = Convert.ToInt32(financeType); //will come from client side for , table Finance Type : Leasing : 1, Rental:2, Instalment:4
                    dbEntity.FunderID = Convert.ToInt32(funder);
                    dbEntity.FunderPlanID = Convert.ToInt32(funderPlan);

                    dbEntity.ModifiedDate = DateTime.Now.ToLocalTime();

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
