using CsvHelper;
using CsvHelper.Configuration;
using IMFS.Core;
using IMFS.DataAccess.Repository;
using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Misc;
using IMFS.Web.Models.Rates;
using IMFS.Web.Models.ResponseModel;

using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IMFS.Web.Models.Enums;
using Newtonsoft.Json;
using System.Data.Entity.SqlServer;
using IMFS.BusinessLogic.Utility;

namespace IMFS.BusinessLogic.Rate
{

    public class RateManager : IRateManager
    {

        private readonly IRepository<QuoteRate> _quoteRateRepository;        

        private readonly IRepository<Types> _typesRepository;

        private readonly IRepository<Categories> _categoriesRepository;

        private readonly IRepository<Web.Models.DBModel.Funder> _fundersRepository;

        private readonly IRepository<Web.Models.DBModel.FinanceType> _financeTypeRepository;

        private readonly IRepository<Web.Models.DBModel.FinanceProductType> _financeProductTypeRepository;

        private readonly IRepository<Web.Models.DBModel.Vendor> _vendorsRepository;        

        private readonly IRepository<Web.Models.DBModel.Product> _productsRepository;

        public RateManager(IRepository<QuoteRate> quoteRateRepository,
            IRepository<Types> typesRepository,
            IRepository<Categories> categoriesRepository,
            IRepository<Web.Models.DBModel.Funder> funderRepository,
            IRepository<Web.Models.DBModel.Vendor> vendorRepository,                        
            IRepository<Web.Models.DBModel.Product> productsRepository,
            IRepository<Web.Models.DBModel.FinanceType> financeTypeRepository,
            IRepository<Web.Models.DBModel.FinanceProductType> financeProductTypeRepository
            )
        {
            _quoteRateRepository = quoteRateRepository;
            _typesRepository = typesRepository;
            _categoriesRepository = categoriesRepository;
            _fundersRepository = funderRepository;
            _vendorsRepository = vendorRepository;       
            _productsRepository = productsRepository;
            _financeTypeRepository = financeTypeRepository;
            _financeProductTypeRepository = financeProductTypeRepository;

        }

        public List<QuoteRateResponse> GetRates(RateInputModel inputModel)
        {           

            List<QuoteRateResponse> quoteRates = new List<QuoteRateResponse>();
            List<QuoteRateResponse> updatedTypes = new List<QuoteRateResponse>();

            try
            {
                var financeType = (from fu in _financeTypeRepository.Table where fu.QuoteDurationType == inputModel.FinanceType select fu).FirstOrDefault().Description;

                quoteRates = (from qr in _quoteRateRepository.Table
                              join types in _typesRepository.Table on qr.TypeID equals types.Id
                              join categories in _categoriesRepository.Table.DefaultIfEmpty() on qr.CategoryID equals categories.Id into cats
                              from categories in cats.DefaultIfEmpty()
                              join vendors in _vendorsRepository.Table.DefaultIfEmpty() on qr.VendorID equals vendors.Id into vens
                              from vendors in vens.DefaultIfEmpty()
                              join products in _productsRepository.Table.DefaultIfEmpty() on qr.ImSKUID equals products.ImSKUID into prods
                              from products in prods.DefaultIfEmpty()
                              where (
                              (inputModel.ProductType != 0 && inputModel.ProductType != null ? types.FinanceProductTypeCode == inputModel.ProductType : 1 == 1)
                              && (inputModel.FunderId != 0 && inputModel.FunderId != null ? qr.FunderID == inputModel.FunderId : 1 == 1)
                              && (inputModel.FinanceType != 0 && inputModel.FinanceType != null ? qr.QuoteDurationType == inputModel.FinanceType : 1 == 1)
                              && (inputModel.VendorId != 0 && inputModel.VendorId != null ? qr.VendorID == inputModel.VendorId : 1 == 1)                              
                              )
                              select new QuoteRateResponse
                              {
                                  TypeID = qr.TypeID,
                                  TypeDescription = types.Description,
                                  CategoryID = qr.CategoryID,
                                  CategoryDescription = categories.Description,
                                  ImSKUID = qr.ImSKUID,
                                  VendorSKUID = qr.VendorSKUID,
                                  Value = qr.Value,
                                  QuoteDurationID = qr.QuoteDurationID,
                                  QuoteDurationType = qr.QuoteDurationType,
                                  PaymentType = qr.PaymentType,
                                  FunderID = qr.FunderID,
                                  VendorID = qr.VendorID,
                                  VendorCode = vendors.VendorCode,
                                  VendorName = vendors.VendorName
                              }).OrderBy(x => x.TypeID).ToList();                

                foreach (var item in quoteRates)
                {
                    //if(item.VendorSKUID == "39M5894")
                    if (updatedTypes.Where(r => r.TypeID == item.TypeID && r.CategoryID == item.CategoryID &&
                    r.FunderID == item.FunderID && r.VendorID == item.VendorID &&
                    r.ImSKUID == item.ImSKUID && r.VendorSKUID == item.VendorSKUID
                    ).Count() > 0)
                    {
                        //Then update other fields, otherwise add a new item
                        var oldItem = updatedTypes.Where(r => r.TypeID == item.TypeID 
                        && r.CategoryID == item.CategoryID && r.FunderID == item.FunderID
                        && r.VendorID == item.VendorID
                        && r.ImSKUID == item.ImSKUID
                        && r.VendorSKUID == item.VendorSKUID
                        ).FirstOrDefault();
                        //For years 1,2,3,4,5
                        GetRateValue(item, oldItem);
                    }
                    else
                    {
                        var newItem = new QuoteRateResponse();
                        newItem.TypeID = item.TypeID;
                        newItem.TypeDescription = item.TypeDescription;
                        newItem.CategoryID = item.CategoryID;
                        newItem.CategoryDescription = item.CategoryDescription;
                        newItem.PaymentType = item.PaymentType;
                        newItem.QuoteDurationTypeDescr = financeType;
                        newItem.ImSKUID = item.ImSKUID;
                        newItem.VendorSKUID = item.VendorSKUID;
                        newItem.FunderID = item.FunderID;
                        newItem.VendorID = item.VendorID;
                        newItem.VendorCode = item.VendorCode;
                        newItem.VendorName = item.VendorName;

                        //For years 1,2,3,4,5
                        GetRateValue(item, newItem);

                        updatedTypes.Add(newItem);
                    }
                }
            }
            catch(Exception ex)
            {

            }
            
            return updatedTypes;
        }


        /// <summary>
        /// this updates the value for the year like 12 months, 24 months etc
        /// </summary>
        /// <param name="item"></param>
        /// <param name="newItem"></param>
        private static void GetRateValue(QuoteRateResponse item, QuoteRateResponse newItem)
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


            //value like lease, rental or instalment
            newItem.QuoteDurationType = item.QuoteDurationType;
        }
        
         

        public ErrorModel SaveRate(QuoteRateResponse quoteRateResponse)
        {
            var response = new ErrorModel();
            var dbEntityList = new List<QuoteRate>();

            try
            {
                foreach (IMFSEnums.QuoteDuration serviceDuration in Enum.GetValues(typeof(IMFSEnums.QuoteDuration)))
                {
                    var dbEntity = new QuoteRate();
                    dbEntity.TypeID = quoteRateResponse.TypeID;
                    dbEntity.CategoryID = quoteRateResponse.CategoryID;
                    dbEntity.ImSKUID = quoteRateResponse.ImSKUID;
                    dbEntity.VendorSKUID = quoteRateResponse.VendorSKUID;
                    dbEntity.FunderID = quoteRateResponse.FunderID;
                    dbEntity.VendorID = quoteRateResponse.VendorID;
                    dbEntity.QuoteDurationType = quoteRateResponse.QuoteDurationType;

                    switch ((int)serviceDuration)
                    {
                        case 12:
                            AddItem(1, "Monthly", quoteRateResponse.months12Monthly, dbEntityList, dbEntity);
                            AddItem(1, "Quarterly", quoteRateResponse.months12Quarterly, dbEntityList, dbEntity);
                            AddItem(1, "Upfront", quoteRateResponse.months12Upfront, dbEntityList, dbEntity);

                            break;
                        case 24:
                            AddItem(2, "Monthly", quoteRateResponse.months24Monthly, dbEntityList, dbEntity);
                            AddItem(2, "Quarterly", quoteRateResponse.months24Quarterly, dbEntityList, dbEntity);
                            AddItem(2, "Upfront", quoteRateResponse.months24Upfront, dbEntityList, dbEntity);

                            break;
                        case 36:
                            AddItem(3, "Monthly", quoteRateResponse.months36Monthly, dbEntityList, dbEntity);
                            AddItem(3, "Quarterly", quoteRateResponse.months36Quarterly, dbEntityList, dbEntity);
                            AddItem(3, "Upfront", quoteRateResponse.months36Upfront, dbEntityList, dbEntity);

                            break;
                        case 48:
                            AddItem(4, "Monthly", quoteRateResponse.months48Monthly, dbEntityList, dbEntity);
                            AddItem(4, "Quarterly", quoteRateResponse.months48Quarterly, dbEntityList, dbEntity);
                            AddItem(4, "Upfront", quoteRateResponse.months48Upfront, dbEntityList, dbEntity);

                            break;
                        case 60:
                            AddItem(5, "Monthly", quoteRateResponse.months60Monthly, dbEntityList, dbEntity);
                            AddItem(5, "Quarterly", quoteRateResponse.months60Quarterly, dbEntityList, dbEntity);
                            AddItem(5, "Upfront", quoteRateResponse.months60Upfront, dbEntityList, dbEntity);

                            break;

                    }
                }

                foreach (var dbEntity in dbEntityList)
                {
                    var existingQuoteRate = _quoteRateRepository.Table.
                        Where(x => x.TypeID == dbEntity.TypeID &&
                        (x.CategoryID == dbEntity.CategoryID || x.CategoryID == 0) &&
                        x.FunderID == dbEntity.FunderID &&
                        x.VendorID == dbEntity.VendorID &&
                        x.ImSKUID == dbEntity.ImSKUID &&
                        x.VendorSKUID == dbEntity.VendorSKUID &&
                        x.QuoteDurationID == dbEntity.QuoteDurationID &&
                        x.QuoteDurationType == dbEntity.QuoteDurationType &&
                        x.PaymentType == dbEntity.PaymentType).FirstOrDefault();

                    if (existingQuoteRate != null)
                    {
                        existingQuoteRate.Value = dbEntity.Value;
                        existingQuoteRate.ModifiedDate = DateTime.Now.ToLocalTime();

                        _quoteRateRepository.Update(dbEntity);
                    }
                    else
                    {
                        _quoteRateRepository.Insert(dbEntity);
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

        private static void AddItem(int quoteDurationId, string paymentType, double? value, List<QuoteRate> dbEntityList, QuoteRate dbEntity)
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

        public DownloadResponse ExportRates(RateInputModel inputModel)
        {
            var response = new DownloadResponse();
            var rates = GetRates(inputModel);
            StringBuilder fileText = new StringBuilder(1024 * 1024);// 1MB

            var funderCode = _fundersRepository.Table.Where(f => f.Id == inputModel.FunderId).FirstOrDefault().FunderCode;
            var financeTypeDescr = _financeTypeRepository.Table.Where(f => f.Id == inputModel.FinanceType).FirstOrDefault().Description;
            var financeProductTypeDescr = _financeProductTypeRepository.Table.Where(f => f.Id == inputModel.ProductType).FirstOrDefault().Description;

            // write header
            fileText.Append(Tools.AppendCSV("Type"));
            fileText.Append(Tools.AppendCSV("Category"));
            fileText.Append(Tools.AppendCSV("Vendor"));
            fileText.Append(Tools.AppendCSV("IM SKU"));
            fileText.Append(Tools.AppendCSV("Vendor SKU"));
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
                fileText.Append(Tools.AppendCSV(rate.TypeDescription));
                fileText.Append(Tools.AppendCSV(rate.CategoryDescription));
                fileText.Append(Tools.AppendCSV((rate.VendorID != null && rate.VendorID !=0) ? _vendorsRepository.Table.Where(v => v.Id == rate.VendorID).FirstOrDefault().VendorName : ""));
                fileText.Append(Tools.AppendCSV(rate.ImSKUID));
                fileText.Append(Tools.AppendCSV(rate.VendorSKUID));
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
            response.FileName = funderCode + " " + financeProductTypeDescr + " " + financeTypeDescr + " Rate Export.csv";
            return response;
        }


        public List<ErrorModel> UploadRate(IFormFile file, string funder, string productType, string financeType)
        {
            var result = readCsv(file, funder, productType, financeType);

            return result;
        }

        /// <summary>
        /// Not in Use,for excel
        /// </summary>
        /// <param name="file"></param>
        private void readExcel(IFormFile file)
        {
            long length = file.Length;
            if (length < 0)
                return;

            using var fileStream = file.OpenReadStream();
            //byte[] bytes = new byte[length];
            //fileStream.Read(bytes, 0, (int)file.Length);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPackage = new ExcelPackage(fileStream))
            {
                var myRecordsWorkbook = excelPackage.Workbook.Worksheets;

                foreach (var workBook in myRecordsWorkbook)
                {
                    var quoteRateUpload = new QuoteRate();
                    int rows = workBook.Dimension.Rows;
                    int columns = workBook.Dimension.Columns;

                    for (int i = 2; i <= rows; i++)
                    {
                        for (int j = 1; j <= columns; j++)
                        {
                            string content = workBook.Cells[i, j].Value.ToString();
                            /* Do something ...*/
                        }
                    }
                }
                var test = myRecordsWorkbook[0].Cells[1, 1].Value.ToString();
            }               
            
        }

        private List<ErrorModel> readCsv(IFormFile file, string funder, string productType, string financeType)
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
                    
                    csv.Context.RegisterClassMap<RateInputExcelModelMap>();

                    var records = csv.GetRecords<RateInputExcelModel>();

                    var dbEntityList = new List<QuoteRate>();

                    if (records != null)
                    {
                        int counterLine = 2;//First line is for headers
                        foreach (var record in records)
                        {
                            var st = record;
                            var error = insertQuoteRate(record, dbEntityList, errorList, funder, productType, financeType, counterLine);
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
                            var existingQuoteRate = _quoteRateRepository.Table.
                                Where(x => x.TypeID == dbEntity.TypeID &&
                                (x.CategoryID == dbEntity.CategoryID || x.CategoryID == 0) &&
                                x.FunderID == dbEntity.FunderID &&
                                x.VendorID == dbEntity.VendorID &&
                                x.ImSKUID == dbEntity.ImSKUID &&
                                x.VendorSKUID == dbEntity.VendorSKUID &&
                                x.QuoteDurationID == dbEntity.QuoteDurationID &&
                                x.QuoteDurationType == dbEntity.QuoteDurationType &&
                                x.PaymentType == dbEntity.PaymentType).FirstOrDefault();

                            if (existingQuoteRate != null)
                            {
                                existingQuoteRate.Value = dbEntity.Value;
                                existingQuoteRate.ModifiedDate = DateTime.Now.ToLocalTime();

                                _quoteRateRepository.Update(dbEntity);
                            }
                            else
                            {
                                _quoteRateRepository.Insert(dbEntity);
                            }
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                errorList.Add(new ErrorModel() { HasError = true, ErrorMessage = ex.Message });
            }
            return errorList;
        }


        private ErrorModel insertQuoteRate(RateInputExcelModel data, List<QuoteRate> dbEntityList, List<ErrorModel> errorList, string funder, string productType, string financeType, int counterLine)
        {
            var errorModel = new ErrorModel();
            try
            {
                int counter = 1;
                foreach (IMFSEnums.QuoteDuration serviceDuration in Enum.GetValues(typeof(IMFSEnums.QuoteDuration)))
                {
                    var dbEntity = new QuoteRate();

                    //Check Type record, type is mandatory
                    if (!string.IsNullOrEmpty(data.Type))
                    {
                        var typeTrimmed = data.Type.Trim();

                        var typeRecord = _typesRepository.Table.Where(t => t.Name == typeTrimmed && t.FinanceProductTypeCode.ToString() == productType).FirstOrDefault();
                        if (typeRecord != null && typeRecord.Id > 0)
                        {
                            dbEntity.TypeID = typeRecord.Id;
                        }
                        else
                        {
                            if (counter == 1)
                            {
                                errorList.Add(new ErrorModel()
                                {
                                    HasError = true,
                                    ErrorMessage = string.Format("Type: {0} not found in DB for FinanceProductType: {1} at line:{2}", typeTrimmed, productType, counterLine)
                                });
                            }
                        }
                    }
                    else
                    {
                        if (counter == 1)
                        {
                            errorList.Add(new ErrorModel()
                            {
                                HasError = true,
                                ErrorMessage = string.Format("Type cannot be null or blank at line:{0}", counterLine)
                            });
                        }
                    }

                    //Check if category exists in upload file then validate it
                    if (!string.IsNullOrEmpty(data.Category))
                    {
                        var categoryTrimmed = data.Category.Trim();

                        var categoryRecord = _categoriesRepository.Table.Where(c => c.Name == categoryTrimmed && c.TypeID == dbEntity.TypeID).FirstOrDefault();

                        if (categoryRecord != null && categoryRecord.Id > 0)
                        {
                            dbEntity.CategoryID = categoryRecord.Id;
                        }
                        else
                        {
                            if (counter == 1)
                            {
                                errorList.Add(new ErrorModel()
                                {
                                    HasError = true,
                                    ErrorMessage = string.Format("Category: {0} not found in DB at line:{1}", categoryTrimmed, counterLine)
                                });
                            }
                        }
                    }

                    //Check if vendor code is valid
                    if (!string.IsNullOrEmpty(data.Vendor))
                    {
                        var vendorRecord = _vendorsRepository.Table.Where(v => v.VendorCode == data.Vendor.Trim()).FirstOrDefault();

                        if (vendorRecord != null && vendorRecord.Id > 0)
                        {
                            dbEntity.VendorID = vendorRecord.Id;
                        }
                        else
                        {
                            if (counter == 1)
                            {
                                errorList.Add(new ErrorModel()
                                {
                                    HasError = true,
                                    ErrorMessage = string.Format("Vendor: {0} not found in DB at line:{1}", data.Vendor, counterLine)
                                });
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(data.ImSKU))
                        dbEntity.ImSKUID = data.ImSKU;

                    if (!string.IsNullOrEmpty(data.VendorSKU))
                        dbEntity.VendorSKUID = data.VendorSKU;

                    dbEntity.QuoteDurationType = Convert.ToInt32(financeType); //will come from client side for , table Finance Type : Leasing : 1, Rental:2, Instalment:4
                    dbEntity.FunderID = Convert.ToInt32(funder);

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
            catch(Exception ex)
            {
                errorModel.HasError = true;
                errorModel.ErrorMessage = JsonConvert.SerializeObject(ex);
            }
            return errorModel;
        }
    }  

    
}
