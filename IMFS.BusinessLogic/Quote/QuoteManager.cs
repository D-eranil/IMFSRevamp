using IMFS.BusinessLogic.Helpers;
using IMFS.BusinessLogic.QuotePercentRate;
using IMFS.BusinessLogic.Rate;
using IMFS.BusinessLogic.UserManagement;
using IMFS.DataAccess.DBContexts;
using IMFS.DataAccess.Models.StoreProcedures;
using IMFS.DataAccess.Repository;
using IMFS.Web.Models.BestRates;
using IMFS.Web.Models.Customer;
using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Misc;
using IMFS.Web.Models.Quote;
using IMFS.Web.Models.QuoteRateCalculation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IMFS.BusinessLogic.Quote
{
    public class QuoteManager : IQuoteManager
    {
        private readonly IRepository<Web.Models.DBModel.QuoteRate> _quoteRateRepository;
        private readonly IRepository<Web.Models.DBModel.QuoteOrigins> _quoteOriginsRateRepository;
        private readonly IRepository<Web.Models.DBModel.Funder> _funderRepository;
        private readonly IRepository<Web.Models.DBModel.FunderPlan> _funderPlanRepository;
        private readonly IRepository<Web.Models.DBModel.FinanceType> _financeTypeRepository;
        private readonly IRepository<Web.Models.DBModel.FinanceProductType> _financeProductTypeRepository;
        private readonly IRepository<Web.Models.DBModel.QuoteDuration> _quoteDurationRepository;
        private readonly IRepository<Web.Models.DBModel.QuoteBreakPercentRate> _quotePercentRateRepository;
        private readonly IRepository<Web.Models.DBModel.QuoteBreakTotalRate> _quoteTotalRateRepository;
        private readonly IRepository<Web.Models.DBModel.VSRProductType> _VSRProductTypeRateRepository;
        private readonly IGenericORPrepository<Web.Models.OPRDBModel.mProduct> _mProductRepository;
        private readonly IGenericORPrepository<Web.Models.OPRDBModel.mCustomer> _mCustomerRepository;
        private readonly IGenericORPrepository<Web.Models.OPRDBModel.CustomerAddresses> _mCustomerAddressesRepository;
        private readonly IRepository<IMFS.Web.Models.DBModel.Attachments> _mQuoteAttachmentsRepository;
        private readonly IRepository<Web.Models.DBModel.Quotes> _quotesRepository;
        private readonly IRepository<Web.Models.DBModel.Types> _typesRepository;
        private readonly IRepository<Web.Models.DBModel.Categories> _categoriesRepository;

        private readonly IRepository<Web.Models.DBModel.QuoteLines> _quoteLinesRepository;
        private readonly IRepository<Web.Models.DBModel.QuoteLinesVersion> _quoteLinesVersionRepository;
        private readonly IRepository<Web.Models.DBModel.Status> _statusRepository;
        private readonly IRepository<Web.Models.DBModel.QuoteLog> _quoteLogRepository;
        private readonly IRepository<Web.Models.DBModel.Config> _configRepository;

        private readonly IRateManager _rateManager;
        private readonly IUserManager _userManager;
        private readonly RestClient _client;

        public QuoteManager(IRepository<Web.Models.DBModel.QuoteRate> quoteRateRepository,
            IRepository<Web.Models.DBModel.QuoteOrigins> quoteOriginsRepository,
            IRateManager rateManager,
            IUserManager userManager,
            IQuotePercentRateManager quotePercentRateManager,
            IQuotePercentRateManager quoteTotalRateManager,
            IRepository<Web.Models.DBModel.Funder> funderRepository,
            IRepository<Web.Models.DBModel.FunderPlan> funderPlanRepository,
            IRepository<Web.Models.DBModel.FinanceType> financeTypeRepository,
            IRepository<Web.Models.DBModel.QuoteDuration> quoteDurationRepository,
            IRepository<Web.Models.DBModel.QuoteBreakTotalRate> quoteTotalRateRepository,
            IRepository<Web.Models.DBModel.VSRProductType> VSRProductTypeRateRepository,
            IRepository<Web.Models.DBModel.QuoteBreakPercentRate> quotePercentRateRepository,
            IRepository<Web.Models.DBModel.FinanceProductType> financeProductTypeRepository,
            IGenericORPrepository<Web.Models.OPRDBModel.mProduct> mProductRepository,
            IGenericORPrepository<Web.Models.OPRDBModel.mCustomer> mCustomerRepository,
            IGenericORPrepository<Web.Models.OPRDBModel.CustomerAddresses> mCustomerAddressesRepository,
            IRepository<IMFS.Web.Models.DBModel.Attachments> mQuoteAttachmentsRepository,
            IRepository<Web.Models.DBModel.Quotes> quotesRepository,
            IRepository<Web.Models.DBModel.QuoteLines> quoteLinesRepository,
            IRepository<Web.Models.DBModel.QuoteLinesVersion> quoteLinesVersionRepository,
            IRepository<Web.Models.DBModel.Status> statusRepository,
            IRepository<Web.Models.DBModel.QuoteLog> quoteLogRepository,
            IRepository<Web.Models.DBModel.Types> typesRepository,
            IRepository<Web.Models.DBModel.Categories> categoriesRepository,
            IRepository<Web.Models.DBModel.Config> configRepository
            )
        {
            _quoteRateRepository = quoteRateRepository;
            _quoteOriginsRateRepository = quoteOriginsRepository;
            _rateManager = rateManager;
            _userManager = userManager;
            _funderRepository = funderRepository;
            _financeTypeRepository = financeTypeRepository;
            _quoteDurationRepository = quoteDurationRepository;
            _quoteTotalRateRepository = quoteTotalRateRepository;
            _VSRProductTypeRateRepository = VSRProductTypeRateRepository;
            _quotePercentRateRepository = quotePercentRateRepository;
            _financeProductTypeRepository = financeProductTypeRepository;
            _mProductRepository = mProductRepository;
            _mCustomerRepository = mCustomerRepository;
            _mCustomerAddressesRepository = mCustomerAddressesRepository;
            _mQuoteAttachmentsRepository = mQuoteAttachmentsRepository;
            _quotesRepository = quotesRepository;
            _quoteLinesRepository = quoteLinesRepository;
            _quoteLinesVersionRepository = quoteLinesVersionRepository;
            _statusRepository = statusRepository;
            _funderPlanRepository = funderPlanRepository;
            _quoteLogRepository = quoteLogRepository;
            _typesRepository = typesRepository;
            _categoriesRepository = categoriesRepository;
            _configRepository = configRepository;
        }


        public QuoteRateCalculationResponseModel CalculateRate(QuoteRateCalculationModel quoteModel, string defaultFrequency, string defaultDuration, string defaultFinanceType)
        {
            var response = new QuoteRateCalculationResponseModel();

            try
            {
                var quoteOrigin = (from qo in _quoteOriginsRateRepository.Table where qo.Description == quoteModel.Source select qo).FirstOrDefault();

                var rates = new List<QuoteResponseModel>();

                var funder = (from fu in _funderRepository.Table where fu.FunderCode == quoteModel.Funder select fu).FirstOrDefault();

                var funderPlan = new IMFS.Web.Models.DBModel.FunderPlan();
                if (!string.IsNullOrEmpty(quoteModel.FunderPlan))
                {
                    int funderPlanId = Convert.ToInt32(quoteModel.FunderPlan);
                    funderPlan = (from fu in _funderPlanRepository.Table where fu.PlanId == funderPlanId select fu).FirstOrDefault();
                }

                var financeType = new IMFS.Web.Models.DBModel.FinanceType();

                //Update Finance Type value if null with default 
                if (string.IsNullOrEmpty(quoteModel.FinanceType))
                {
                    financeType = (from ft in _financeTypeRepository.Table where ft.Description == defaultFinanceType select ft).FirstOrDefault();
                    quoteModel.FinanceType = financeType.QuoteDurationType.ToString();
                }
                else
                {
                    //if(quoteModel.FinanceType == "Leasing")
                    var tempFinanceType = Convert.ToInt32(quoteModel.FinanceType);
                    financeType = (from ft in _financeTypeRepository.Table where ft.QuoteDurationType == tempFinanceType select ft).FirstOrDefault();
                }

                //Update frequency value if null with default 
                if (quoteModel.Frequency == null || quoteModel.Frequency.Count() == 0)
                {
                    quoteModel.Frequency = new string[] { defaultFrequency };
                    //quoteModel.Frequency = new string[] { "monthly", "quarterly", "upfront" };
                }

                //Update duration value if null with default
                if (quoteModel.Duration == null || quoteModel.Duration.Count() == 0)
                {
                    quoteModel.Duration = new string[] { defaultDuration };
                }




                //GST Calculation for each line and quote total
                if (quoteModel.IncludeTax)
                {
                    //Update line total
                    if (quoteModel.TaxRate > 0)
                    {
                        foreach (var quoteLine in quoteModel.QuoteLines)
                        {
                            var lineGSTTotal = Convert.ToDouble(quoteLine.LineTotal) + ((quoteModel.TaxRate * Convert.ToDouble(quoteLine.LineTotal)) / 100);
                            quoteLine.LineTotal = lineGSTTotal;
                        }

                        double quoteGSTTotalValue = 0;

                        //If GST include
                        if (quoteModel.GstInclude != null)
                        {
                            if (quoteModel.GstInclude == 1)
                            {
                                quoteGSTTotalValue = Convert.ToDouble(quoteModel.QuoteTotal) + ((quoteModel.TaxRate * Convert.ToDouble(quoteModel.QuoteTotal)) / 100);
                            }
                            else
                            {
                                quoteGSTTotalValue = Convert.ToDouble(quoteModel.QuoteTotal);
                            }
                        }
                        else
                        {
                            quoteGSTTotalValue = Convert.ToDouble(quoteModel.QuoteTotal) + ((quoteModel.TaxRate * Convert.ToDouble(quoteModel.QuoteTotal)) / 100);
                        }

                        quoteModel.QuoteTotal = (decimal)quoteGSTTotalValue;
                    }
                }

                //Calculate rate by SKU
                //var typeCount = (from ql in quoteModel.QuoteLines where !string.IsNullOrEmpty(ql.Type) select ql).Count();
                var typeCount = (from ql in quoteModel.QuoteLines where !string.IsNullOrEmpty(ql.Type) select ql).Count();
                if (typeCount > 0)
                {
                    var quoteBySKU = CalculateQuotePriceBySKU(response, quoteModel, financeType, funder);
                    if (response.HasError)
                    {
                        return response;
                    }
                    else
                    {
                        rates.AddRange(quoteBySKU);
                    }
                }

                //Calculate rate by Total
                var quoteByTotal = CalculateQuotePriceByTotalValue(response, quoteModel, financeType, funder, funderPlan);
                if (response.HasError)
                {
                    return response;
                }
                else
                {
                    rates.AddRange(quoteByTotal);
                }

                //Calculate rate by Percent
                var quoteByPercent = CalculateQuotePriceByPercentValue(response, quoteModel, financeType, funder, funderPlan);
                if (response.HasError)
                {
                    return response;
                }
                else
                {
                    rates.AddRange(quoteByPercent);
                }


                //Get the best quote
                var finalRates = new List<QuoteResponseModel>();
                foreach (var duration in quoteModel.Duration)
                {
                    foreach (var frequency in quoteModel.Frequency)
                    {
                        var t = rates.Where(r => r.Duration == duration && r.Frequency == frequency).OrderByDescending(r => r.FinanceTotal).FirstOrDefault();

                        finalRates.Add(t);
                    }
                }

                //Prepare the response
                if (finalRates != null && finalRates.Count > 0)
                {
                    response.QuoteResponse = finalRates;
                }
                else
                {
                    response.ErrorMessage = MessageHelper.MSG_NO_OPTION_FOUND;
                    response.QuoteResponse = finalRates;
                }

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;

        }
        private List<QuoteResponseModel> CalculateQuotePriceByTotalValue(ErrorModel response, QuoteRateCalculationModel quoteModel,
            Web.Models.DBModel.FinanceType financeType,
            Web.Models.DBModel.Funder funder,
            Web.Models.DBModel.FunderPlan funderPlan)
        {
            var rateDetails = new List<QuoteResponseModel>();
            try
            {
                //First Step Get rates for supplied Quote total
                foreach (var duration in quoteModel.Duration)
                {
                    foreach (var frequency in quoteModel.Frequency)
                    {
                        var bestRateByTotal = getBestRateByTotal(quoteModel.QuoteTotal, Convert.ToInt32(duration), frequency, Convert.ToInt32(quoteModel.FinanceType), funderPlan.PlanId);
                        if (bestRateByTotal != null)
                        {
                            var quoteRate = new QuoteResponseModel();
                            quoteRate.Id = bestRateByTotal.Id;
                            quoteRate.Rate = Convert.ToDecimal(bestRateByTotal.Value);
                            quoteRate.CalculationType = Web.Models.Enums.IMFSEnums.QuotePriceType.ByTotal.ToString();
                            quoteRate.Duration = duration;
                            quoteRate.FinanceType = financeType.Description;
                            quoteRate.FinanceTypeID = quoteModel.FinanceType;
                            quoteRate.Frequency = frequency;

                            if (bestRateByTotal.FunderID != null)
                            {
                                quoteRate.Funder = funder != null ? funder.FunderName : (from fu in _funderRepository.Table where fu.Id == bestRateByTotal.FunderID select fu).FirstOrDefault().FunderCode;
                                quoteRate.FunderID = Convert.ToString(bestRateByTotal.FunderID);
                            }
                            if (bestRateByTotal.FunderPlanID != null)
                            {
                                quoteRate.FunderPlanDescription = !string.IsNullOrEmpty(funderPlan.PlanDescription) ?
                                    funderPlan.PlanDescription :
                                    (from fu in _funderPlanRepository.Table where fu.PlanId == bestRateByTotal.FunderPlanID select fu).FirstOrDefault().PlanDescription;
                                quoteRate.FunderPlanID = Convert.ToString(bestRateByTotal.FunderPlanID);
                            }
                            var instalment = Convert.ToDecimal(quoteModel.QuoteTotal) * Convert.ToDecimal(bestRateByTotal.Value);
                            quoteRate.FinanceTotal = decimal.Parse(instalment.ToString("0.00"));

                            rateDetails.Add(quoteRate);
                        }
                    }//frequency loop close
                }//duration loop close

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return rateDetails;

        }

        private BestRateByTotal getBestRateByTotal(decimal quoteTotal, int duration, string frequency, int financeType, int funderPlanId = 0)
        {
            var bestFunderRate = new BestRateByTotal();
            try
            {
                var tempQuoteTotal = Convert.ToDouble(quoteTotal);
                var durationId = _quoteDurationRepository.Table.Where(d => d.Value == duration && d.IsActive == true).FirstOrDefault().Id;
                var funderRates = (from qr in _quoteTotalRateRepository.Table
                                   where ((qr.PaymentType == frequency)
                                   && (qr.FinanceType == financeType)
                                   && (funderPlanId != 0 ? qr.FunderPlanID == funderPlanId : 1 == 1)
                                   && (qr.QuoteDurationID == durationId)
                                   && (tempQuoteTotal >= qr.Min && tempQuoteTotal <= qr.Max))
                                   select new BestRateByTotal
                                   {
                                       Id = qr.Id,
                                       Min = qr.Min,
                                       Max = qr.Max,
                                       Value = qr.Value,
                                       QuoteDurationID = qr.QuoteDurationID,
                                       FinanceType = qr.FinanceType,
                                       PaymentType = qr.PaymentType,
                                       FunderID = qr.FunderID,
                                       FunderPlanID = qr.FunderPlanID
                                   }).OrderBy(x => x.Min).ToList();

                bestFunderRate = funderRates.OrderByDescending(f => f.Value).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bestFunderRate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="category"></param>
        /// <param name="duration"></param>
        /// <param name="frequency"></param>
        /// <param name="financeType"></param>
        /// <returns></returns>
        private BestRateBySKU getBestRateByTypeCategory(string type, string category, int duration, string frequency, int financeType)
        {
            var bestFunderRate = new BestRateBySKU();
            try
            {
                var durationId = _quoteDurationRepository.Table.Where(d => d.Value == duration && d.IsActive == true).FirstOrDefault().Id;
                var funderRates = (from qr in _quoteRateRepository.Table
                                   where ((qr.PaymentType == frequency)
                                   && (qr.QuoteDurationType == financeType)
                                   && (qr.QuoteDurationID == durationId)
                                   && (!string.IsNullOrEmpty(type) ? qr.TypeID.ToString() == type : 1 == 1)
                                   && (!string.IsNullOrEmpty(category) ? qr.CategoryID.ToString() == category : 1 == 1))
                                   select new BestRateBySKU
                                   {
                                       Id = qr.Id,
                                       TypeID = qr.TypeID,
                                       CategoryID = qr.CategoryID,
                                       ImSKUID = qr.ImSKUID,
                                       VendorSKUID = qr.VendorSKUID,
                                       Value = qr.Value,
                                       QuoteDurationID = qr.QuoteDurationID,
                                       QuoteDurationType = qr.QuoteDurationType,
                                       PaymentType = qr.PaymentType,
                                       FunderID = qr.FunderID
                                   }).OrderBy(x => x.TypeID).ToList();

                bestFunderRate = funderRates.OrderByDescending(f => f.Value).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bestFunderRate;

        }

        private List<QuoteResponseModel> CalculateQuotePriceBySKU(ErrorModel response, QuoteRateCalculationModel quoteModel, Web.Models.DBModel.FinanceType financeType, Web.Models.DBModel.Funder funder)
        {

            var rateDetails = new List<QuoteResponseModel>();

            try
            {

                //First Step Get rates for quote lines for supplied Funder(if any), Finance Type and Product Type
                foreach (var quoteLine in quoteModel.QuoteLines)
                {
                    foreach (var duration in quoteModel.Duration)
                    {
                        foreach (var frequency in quoteModel.Frequency)
                        {
                            var bestRateBySKU = getBestRateByTypeCategory(quoteLine.Type, quoteLine.Category, Convert.ToInt32(duration), frequency, Convert.ToInt32(quoteModel.FinanceType));
                            if (bestRateBySKU != null)
                            {
                                var quoteRate = new QuoteResponseModel();
                                quoteRate.Id = bestRateBySKU.Id;
                                quoteRate.Rate = Convert.ToDecimal(bestRateBySKU.Value);
                                quoteRate.CalculationType = Web.Models.Enums.IMFSEnums.QuotePriceType.BySKU.ToString();
                                quoteRate.Duration = duration;
                                quoteRate.FinanceType = financeType.Description;
                                quoteRate.FinanceTypeID = quoteModel.FinanceType;
                                quoteRate.Frequency = frequency;

                                if (bestRateBySKU.FunderID != null)
                                {
                                    quoteRate.Funder = funder != null ? funder.FunderName : (from fu in _funderRepository.Table where fu.Id == bestRateBySKU.FunderID select fu).FirstOrDefault().FunderCode;
                                    quoteRate.FunderID = Convert.ToString(bestRateBySKU.FunderID);
                                }
                                var instalment = Convert.ToDecimal(quoteLine.LineTotal) * Convert.ToDecimal(bestRateBySKU.Value);
                                quoteRate.FinanceTotal = decimal.Parse(instalment.ToString("0.00"));

                                rateDetails.Add(quoteRate);
                            }
                        }//frequency loop close
                    }//duration loop close


                }//quote line loop close               

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return rateDetails;
        }

        private List<QuoteResponseModel> CalculateQuotePriceByPercentValue(ErrorModel response, QuoteRateCalculationModel quoteModel,
            Web.Models.DBModel.FinanceType financeType, Web.Models.DBModel.Funder funder, Web.Models.DBModel.FunderPlan funderPlan)
        {
            var rateDetails = new List<QuoteResponseModel>();

            try
            {

                foreach (var quoteLine in quoteModel.QuoteLines)
                {
                    //For calculating percent we should have Product Type i.e. Hardware or (Device), Software and Service
                    if (string.IsNullOrEmpty(quoteLine.ProductType))
                    {
                        //if VSR is null then try to get it from IMSKU
                        if (string.IsNullOrEmpty(quoteLine.VSR))
                        {
                            if (!string.IsNullOrEmpty(quoteLine.IMSKU))
                            {
                                var mprodRow = (from mproduct in _mProductRepository.Table where mproduct.InternalSKUID == quoteLine.IMSKU select mproduct).FirstOrDefault();
                                if (mprodRow != null)
                                {
                                    quoteLine.VSR = Convert.ToString(mprodRow.VSRID);
                                }
                            }
                        }

                        //Get Product ID from vsr
                        var vsrProductRow = (from vsr in _VSRProductTypeRateRepository.Table where vsr.VSRID == quoteLine.VSR select vsr).FirstOrDefault();
                        if (vsrProductRow != null)
                        {
                            //Change the string to compare from Hardware to Device
                            if (vsrProductRow.ProductType == "Hardware")
                            {
                                var productCode = (from fp in _financeProductTypeRepository.Table where fp.Description == "Device" select fp).FirstOrDefault().Code;
                                quoteLine.ProductType = Convert.ToString(productCode);
                            }
                            else
                            {
                                var productCode = (from fp in _financeProductTypeRepository.Table where fp.Description == vsrProductRow.ProductType select fp).FirstOrDefault().Code;
                                quoteLine.ProductType = Convert.ToString(productCode);
                            }

                        }
                    }

                    foreach (var duration in quoteModel.Duration)
                    {
                        foreach (var frequency in quoteModel.Frequency)
                        {
                            double percent = Convert.ToDouble(Convert.ToDecimal(quoteLine.LineTotal) / quoteModel.QuoteTotal) * 100;

                            if (!string.IsNullOrEmpty(quoteLine.ProductType))
                            {
                                var bestRateByPercent = getBestRateByPercent(Convert.ToInt32(duration), frequency,
                                    Convert.ToInt32(quoteModel.FinanceType), Convert.ToInt32(quoteLine.ProductType), percent,
                                    funderPlan.PlanId);
                                if (bestRateByPercent != null)
                                {
                                    var quoteRate = new QuoteResponseModel();
                                    quoteRate.Id = bestRateByPercent.Id;
                                    quoteRate.Rate = Convert.ToDecimal(bestRateByPercent.Value);
                                    quoteRate.CalculationType = Web.Models.Enums.IMFSEnums.QuotePriceType.ByPercent.ToString();
                                    quoteRate.Duration = duration;
                                    quoteRate.FinanceType = financeType.Description;
                                    quoteRate.FinanceTypeID = quoteModel.FinanceType;
                                    quoteRate.Frequency = frequency;

                                    if (bestRateByPercent.FunderID != null)
                                    {
                                        quoteRate.Funder = funder != null ? funder.FunderName : (from fu in _funderRepository.Table where fu.Id == bestRateByPercent.FunderID select fu).FirstOrDefault().FunderCode;
                                        quoteRate.FunderID = Convert.ToString(bestRateByPercent.FunderID);
                                    }

                                    if (bestRateByPercent.FunderPlanID != null)
                                    {
                                        quoteRate.FunderPlanDescription = !string.IsNullOrEmpty(funderPlan.PlanDescription) ?
                                            funderPlan.PlanDescription :
                                            (from fu in _funderPlanRepository.Table where fu.PlanId == bestRateByPercent.FunderPlanID select fu).FirstOrDefault().PlanDescription;
                                        quoteRate.FunderPlanID = Convert.ToString(bestRateByPercent.FunderPlanID);
                                    }
                                    var instalment = Convert.ToDecimal(quoteLine.LineTotal) * Convert.ToDecimal(bestRateByPercent.Value);
                                    quoteRate.FinanceTotal = decimal.Parse(instalment.ToString("0.00"));

                                    rateDetails.Add(quoteRate);
                                }
                            }
                        }//frequency loop close
                    }//duration loop close
                }//quote line loop close   
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return rateDetails;
        }

        private BestRateByPercent getBestRateByPercent(int duration, string frequency, int financeType, int productType, double percent, int funderPlanId = 0)
        {
            var bestFunderRate = new BestRateByPercent();
            try
            {

                var durationId = _quoteDurationRepository.Table.Where(d => d.Value == duration && d.IsActive == true).FirstOrDefault().Id;
                var funderRates = (from qr in _quotePercentRateRepository.Table
                                   where ((qr.PaymentType == frequency)
                                   && (qr.FinanceType == financeType)
                                   && (qr.QuoteDurationID == durationId)
                                   && (funderPlanId != 0 ? qr.FunderPlanID == funderPlanId : 1 == 1)
                                   && (qr.ProductType == productType)
                                   && ((percent >= qr.MinPercent && percent <= qr.MaxPercent) || (percent >= qr.MinPercent && qr.MaxPercent == null)))
                                   select new BestRateByPercent
                                   {
                                       Id = qr.Id,
                                       MinPercent = qr.MinPercent,
                                       MaxPercent = qr.MaxPercent,
                                       Value = qr.Value,
                                       QuoteDurationID = qr.QuoteDurationID,
                                       FinanceType = qr.FinanceType,
                                       PaymentType = qr.PaymentType,
                                       FunderID = qr.FunderID,
                                       FunderPlanID = qr.FunderPlanID,
                                       ProductType = qr.ProductType
                                   }).OrderBy(x => x.MinPercent).ToList();

                bestFunderRate = funderRates.OrderByDescending(f => f.Value).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bestFunderRate;
        }


        private int UpdateQuote(QuoteDetailsModel quoteDetailsModel)
        {
            int quoteId = 0;
            try
            {
                var existingQuote = _quotesRepository.GetById(Convert.ToInt32(quoteDetailsModel.QuoteHeader.QuoteNumber));

                if (existingQuote != null)
                {
                    existingQuote.QuoteName = quoteDetailsModel.QuoteHeader.QuoteName;
                    existingQuote.QuoteStatus = (int)quoteDetailsModel.QuoteHeader.Status;
                    existingQuote.LatestVersion += 1;
                    existingQuote.QuoteExpiryDate = quoteDetailsModel.QuoteHeader.ExpiryDate.ToLocalTime();
                    existingQuote.QuoteTotal = (decimal)quoteDetailsModel.QuoteHeader.QuoteTotal;

                    existingQuote.QuoteLastModified = DateTime.Now.ToLocalTime();
                    existingQuote.QuoteFinanceType = quoteDetailsModel.QuoteHeader.FinanceType;  // "Leasing";
                    existingQuote.QuoteFinanceLineType = quoteDetailsModel.QuoteHeader.QuoteType; // "SKU/VPN";  // or Type/Category

                    existingQuote.FinanceDuration = quoteDetailsModel.QuoteHeader.QuoteDuration;
                    existingQuote.FinanceFrequency = quoteDetailsModel.QuoteHeader.Frequency;
                    existingQuote.FinanceFunder = quoteDetailsModel.QuoteHeader.FunderId;

                    //Update new columns 
                    existingQuote.FunderCode = quoteDetailsModel.QuoteHeader.FunderCode;
                    existingQuote.GstInclude = quoteDetailsModel.QuoteHeader.GstInclude;

                    if (quoteDetailsModel.QuoteHeader.FinanceValue != null)
                    {
                        existingQuote.FinanceValue = (decimal)quoteDetailsModel.QuoteHeader.FinanceValue;
                    }

                    if (!string.IsNullOrEmpty(quoteDetailsModel.QuoteHeader.FunderPlan))
                    {
                        existingQuote.FunderPlan = Convert.ToInt32(quoteDetailsModel.QuoteHeader.FunderPlan);
                    }


                    //Reseller/Customer Details
                    existingQuote.ResellerAccount = quoteDetailsModel.CustomerDetails.CustomerNumber;
                    existingQuote.ResellerName = quoteDetailsModel.CustomerDetails.CustomerName;
                    existingQuote.ResellerABN = quoteDetailsModel.CustomerDetails.CustomerABN;
                    existingQuote.ResellerAddressLine1 = quoteDetailsModel.CustomerDetails.CustomerAddressLine1;
                    existingQuote.ResellerAddressLine2 = quoteDetailsModel.CustomerDetails.CustomerAddressLine2;
                    existingQuote.ResellerCity = quoteDetailsModel.CustomerDetails.CustomerAddressCity;
                    existingQuote.ResellerState = quoteDetailsModel.CustomerDetails.CustomerAddressState;
                    existingQuote.ResellerPostcode = quoteDetailsModel.CustomerDetails.CustomerPostCode;
                    existingQuote.ResellerContactName = quoteDetailsModel.CustomerDetails.CustomerContact;
                    existingQuote.ResellerContactEmail = quoteDetailsModel.CustomerDetails.CustomerEmail;
                    existingQuote.ResellerContactPhone = quoteDetailsModel.CustomerDetails.CustomerPhone;
                    existingQuote.ResellerCountry = quoteDetailsModel.CustomerDetails.CustomerCountry;

                    //EndCustomer Details               

                    existingQuote.EndCustomerName = quoteDetailsModel.EndUserDetails.EndCustomerName;
                    existingQuote.EndCustomerContactName = quoteDetailsModel.EndUserDetails.EndCustomerContact;
                    existingQuote.EndCustomerContactEmail = quoteDetailsModel.EndUserDetails.EndCustomerEmail;
                    existingQuote.EndCustomerContactPhone = quoteDetailsModel.EndUserDetails.EndCustomerPhone;
                    existingQuote.EndCustomerABN = quoteDetailsModel.EndUserDetails.EndCustomerABN;
                    existingQuote.EndCustomerYearsTrading = quoteDetailsModel.EndUserDetails.EndCustomerYearsTrading;
                    existingQuote.EndCustomerPrimaryAddressLine1 = quoteDetailsModel.EndUserDetails.EndCustomerAddressLine1;
                    existingQuote.EndCustomerPrimaryAddressLine2 = quoteDetailsModel.EndUserDetails.EndCustomerAddressLine2;
                    existingQuote.EndCustomerPrimaryCity = quoteDetailsModel.EndUserDetails.EndCustomerCity;
                    existingQuote.EndCustomerPrimaryState = quoteDetailsModel.EndUserDetails.EndCustomerState;
                    existingQuote.EndCustomerPrimaryPostcode = quoteDetailsModel.EndUserDetails.EndCustomerPostCode;
                    existingQuote.EndCustomerPrimaryCountry = quoteDetailsModel.EndUserDetails.EndCustomerCountry;
                    existingQuote.EndCustomerSignatoryName = quoteDetailsModel.EndUserDetails.AuthorisedSignatoryName;
                    existingQuote.EndCustomerSignatoryPosition = quoteDetailsModel.EndUserDetails.AuthorisedSignatoryPosition;

                    _quotesRepository.Update(existingQuote);

                    //QuoteID
                    quoteId = existingQuote.Id;


                    //Get QuoteLines from DB                    
                    var existingLines = (from quoteLines in _quoteLinesRepository.Table
                                         where quoteLines.QuoteId == quoteId
                                         select quoteLines).ToList();

                    foreach (var line in existingLines)
                    {
                        var quoteLinesVersion = new QuoteLinesVersion();
                        quoteLinesVersion.QuoteId = line.QuoteId;
                        quoteLinesVersion.Version = line.Version;
                        quoteLinesVersion.QuoteLineNumber = line.QuoteLineNumber;
                        quoteLinesVersion.SKU = string.IsNullOrEmpty(line.SKU) ? "" : line.SKU;
                        quoteLinesVersion.VPN = line.VPN;
                        //If ByCategory then add Type and Category
                        quoteLinesVersion.TypeID = line.TypeID;
                        quoteLinesVersion.CategoryID = line.CategoryID;
                        quoteLinesVersion.Qty = line.Qty;
                        quoteLinesVersion.IngramSellPrice = line.IngramSellPrice;
                        quoteLinesVersion.ResellerMarginPercent = (float?)line.ResellerMarginPercent;
                        quoteLinesVersion.ResellerSellPrice = line.ResellerSellPrice;
                        quoteLinesVersion.LineTotal = line.LineTotal;
                        quoteLinesVersion.LineGST = line.LineGST;
                        quoteLinesVersion.Description = line.Description;
                        quoteLinesVersion.FinanceProductTypeID = line.FinanceProductTypeID;

                        //Move this line to another table once its available
                        _quoteLinesVersionRepository.Insert(quoteLinesVersion);
                        //After moving delete the lines from QuoteLines table
                        _quoteLinesRepository.Delete(line);
                    }

                    int counter = 1;
                    //Insert Quote Lines
                    foreach (var quoteLine in quoteDetailsModel.QuoteLines)
                    {
                        var newQuoteLine = new Web.Models.DBModel.QuoteLines();
                        newQuoteLine.QuoteId = quoteId;
                        newQuoteLine.Version = existingQuote.LatestVersion;
                        newQuoteLine.QuoteLineNumber = counter;
                        newQuoteLine.SKU = string.IsNullOrEmpty(quoteLine.IMSKU) ? "" : quoteLine.IMSKU;
                        newQuoteLine.VPN = quoteLine.VPN;
                        //If ByCategory then add Type and Category
                        if (!string.IsNullOrEmpty(quoteLine.Item))
                        {
                            newQuoteLine.TypeID = int.Parse(quoteLine.Item);
                        }

                        if (!string.IsNullOrEmpty(quoteLine.Category))
                        {
                            newQuoteLine.CategoryID = int.Parse(quoteLine.Category);
                        }
                        newQuoteLine.Qty = quoteLine.Qty;
                        newQuoteLine.IngramSellPrice = quoteLine.CostPrice;
                        newQuoteLine.ResellerMarginPercent = (float?)quoteLine.Margin;
                        newQuoteLine.ResellerSellPrice = quoteLine.SalePrice;
                        newQuoteLine.LineTotal = quoteLine.LineTotal;
                        newQuoteLine.LineGST = quoteLine.TotalGST;
                        newQuoteLine.Description = quoteLine.Description;
                        newQuoteLine.FinanceProductTypeID = quoteLine.FinanceProductTypeID;

                        _quoteLinesRepository.Insert(newQuoteLine);
                        counter++;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return quoteId;
        }

        //Reject Quote Details using for post method and model
        public bool RejectQuoteDetails(RejectQuoteDetail quoteDetailsModel)
        {
            bool result = false;
            try
            {
                var existingQuote = _quotesRepository.GetById(Convert.ToInt32(quoteDetailsModel.QuoteId));

                if (existingQuote != null)
                {

                    //Update new columns for RejectQuote method
                    existingQuote.Comment = quoteDetailsModel.Comment;
                    existingQuote.Reason = quoteDetailsModel.Reason;

                    _quotesRepository.Update(existingQuote);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }



        public QuoteSaveResponseModel SaveQuote(QuoteDetailsModel quoteDetailsModel)
        {
            var response = new QuoteSaveResponseModel();
            try
            {
                if (!string.IsNullOrEmpty(quoteDetailsModel.QuoteHeader.QuoteNumber) && quoteDetailsModel.QuoteHeader.QuoteNumber != "New")
                {
                    response.QuoteId = UpdateQuote(quoteDetailsModel);
                }
                else
                {
                    var newQuote = new Web.Models.DBModel.Quotes();
                    newQuote.QuoteName = quoteDetailsModel.QuoteHeader.QuoteName;
                    newQuote.LatestVersion = 1;
                    newQuote.QuoteExpiryDate = new DateTime(quoteDetailsModel.QuoteHeader.ExpiryDate.Year,
                        quoteDetailsModel.QuoteHeader.ExpiryDate.Month,
                        quoteDetailsModel.QuoteHeader.ExpiryDate.Day);

                    newQuote.QuoteTotal = (decimal)quoteDetailsModel.QuoteHeader.QuoteTotal;

                    if (quoteDetailsModel.QuoteHeader.FinanceValue != null)
                    {
                        newQuote.FinanceValue = (decimal)quoteDetailsModel.QuoteHeader.FinanceValue;
                    }

                    newQuote.FinanceDuration = quoteDetailsModel.QuoteHeader.QuoteDuration;
                    newQuote.FinanceFrequency = quoteDetailsModel.QuoteHeader.Frequency;
                    newQuote.FinanceFunder = quoteDetailsModel.QuoteHeader.FunderId;
                    //Added new columns 
                    newQuote.FunderCode = quoteDetailsModel.QuoteHeader.FunderCode;
                    newQuote.GstInclude = quoteDetailsModel.QuoteHeader.GstInclude;

                    newQuote.QuoteStatus = (int)quoteDetailsModel.QuoteHeader.Status; // "Open":1;

                    newQuote.QuoteCreatedDate = DateTime.Now.ToLocalTime();
                    newQuote.LastViewedDate = DateTime.Now.ToLocalTime();
                    newQuote.QuoteCreatedBy = quoteDetailsModel.QuoteHeader.QuoteCreatedBy;
                    newQuote.QuoteOrigin = "IMFS Portal";
                    newQuote.LatestVersion = 1;
                    newQuote.QuoteFinanceType = quoteDetailsModel.QuoteHeader.FinanceType;  // "Leasing";
                    newQuote.QuoteFinanceLineType = quoteDetailsModel.QuoteHeader.QuoteType; // "SKU/VPN";  // or Type/Category
                    if (!string.IsNullOrEmpty(quoteDetailsModel.QuoteHeader.FunderPlan))
                    {
                        newQuote.FunderPlan = Convert.ToInt32(quoteDetailsModel.QuoteHeader.FunderPlan);
                    }

                    //Reseller/Customer Details
                    newQuote.ResellerAccount = quoteDetailsModel.CustomerDetails.CustomerNumber;
                    newQuote.ResellerName = quoteDetailsModel.CustomerDetails.CustomerName;
                    newQuote.ResellerABN = quoteDetailsModel.CustomerDetails.CustomerABN;
                    newQuote.ResellerAddressLine1 = quoteDetailsModel.CustomerDetails.CustomerAddressLine1;
                    newQuote.ResellerAddressLine2 = quoteDetailsModel.CustomerDetails.CustomerAddressLine2;
                    newQuote.ResellerCity = quoteDetailsModel.CustomerDetails.CustomerAddressCity;
                    newQuote.ResellerState = quoteDetailsModel.CustomerDetails.CustomerAddressState;
                    newQuote.ResellerPostcode = quoteDetailsModel.CustomerDetails.CustomerPostCode;
                    newQuote.ResellerContactName = quoteDetailsModel.CustomerDetails.CustomerContact;
                    newQuote.ResellerContactEmail = quoteDetailsModel.CustomerDetails.CustomerEmail;
                    newQuote.ResellerContactPhone = quoteDetailsModel.CustomerDetails.CustomerPhone;
                    newQuote.ResellerCountry = quoteDetailsModel.CustomerDetails.CustomerCountry;

                    //EndCustomer Details               

                    newQuote.EndCustomerName = quoteDetailsModel.EndUserDetails.EndCustomerName;
                    newQuote.EndCustomerContactName = quoteDetailsModel.EndUserDetails.EndCustomerContact;
                    newQuote.EndCustomerContactEmail = quoteDetailsModel.EndUserDetails.EndCustomerEmail;
                    newQuote.EndCustomerContactPhone = quoteDetailsModel.EndUserDetails.EndCustomerPhone;
                    newQuote.EndCustomerABN = quoteDetailsModel.EndUserDetails.EndCustomerABN;
                    newQuote.EndCustomerYearsTrading = quoteDetailsModel.EndUserDetails.EndCustomerYearsTrading;
                    newQuote.EndCustomerPrimaryAddressLine1 = quoteDetailsModel.EndUserDetails.EndCustomerAddressLine1;
                    newQuote.EndCustomerPrimaryAddressLine2 = quoteDetailsModel.EndUserDetails.EndCustomerAddressLine2;
                    newQuote.EndCustomerPrimaryCity = quoteDetailsModel.EndUserDetails.EndCustomerCity;
                    newQuote.EndCustomerPrimaryState = quoteDetailsModel.EndUserDetails.EndCustomerState;
                    newQuote.EndCustomerPrimaryPostcode = quoteDetailsModel.EndUserDetails.EndCustomerPostCode;
                    newQuote.EndCustomerPrimaryCountry = quoteDetailsModel.EndUserDetails.EndCustomerCountry;
                    newQuote.EndCustomerSignatoryName = quoteDetailsModel.EndUserDetails.AuthorisedSignatoryName;
                    newQuote.EndCustomerSignatoryPosition = quoteDetailsModel.EndUserDetails.AuthorisedSignatoryPosition;


                    _quotesRepository.Insert(newQuote);

                    var quoteId = newQuote.Id;

                    response.QuoteId = quoteId;

                    int counter = 1;
                    //Quote Lines
                    foreach (var quoteLine in quoteDetailsModel.QuoteLines)
                    {
                        var newQuoteLine = new Web.Models.DBModel.QuoteLines();
                        newQuoteLine.QuoteId = quoteId;
                        newQuoteLine.Version = 1;
                        newQuoteLine.QuoteLineNumber = counter;
                        newQuoteLine.SKU = string.IsNullOrEmpty(quoteLine.IMSKU) ? "" : quoteLine.IMSKU;
                        newQuoteLine.VPN = quoteLine.VPN;

                        //If ByCategory then add Type and Category
                        if (!string.IsNullOrEmpty(quoteLine.Item))
                        {
                            int outTypeId = 0;
                            if (int.TryParse(quoteLine.Item, out outTypeId))
                                newQuoteLine.TypeID = outTypeId;
                        }

                        if (!string.IsNullOrEmpty(quoteLine.Category))
                        {
                            int outCategoryId = 0;
                            if (int.TryParse(quoteLine.Category, out outCategoryId))
                                newQuoteLine.CategoryID = outCategoryId;
                        }

                        newQuoteLine.Qty = quoteLine.Qty;
                        newQuoteLine.IngramSellPrice = quoteLine.CostPrice;
                        newQuoteLine.ResellerMarginPercent = (float?)quoteLine.Margin;
                        newQuoteLine.ResellerSellPrice = quoteLine.SalePrice;
                        newQuoteLine.LineTotal = quoteLine.LineTotal;
                        newQuoteLine.LineGST = quoteLine.TotalGST;
                        newQuoteLine.Description = quoteLine.Description;
                        newQuoteLine.FinanceProductTypeID = quoteLine.FinanceProductTypeID;
                        _quoteLinesRepository.Insert(newQuoteLine);

                        counter++;
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

        public CustomerResponseModel GetCustomer(string customerName)
        {
            var customerResponse = new CustomerResponseModel();
            var mCustomerList = new List<ORPCustomer>();
            try
            {
                var items = (from mc in _mCustomerRepository.Table
                             join mca in _mCustomerAddressesRepository.Table on mc.CustomerNumber equals mca.CustomerNumber
                             where ((mc.CustomerName.Contains(customerName) || mc.CustomerNumber.Contains(customerName))
                             && mca.AddressType == "Primary")
                             select new ORPCustomer
                             {
                                 DisplayLabel = mc.CustomerNumber + "-" + mc.CustomerName,
                                 CustomerID = mc.CustomerID,
                                 CustomerName = mc.CustomerName,
                                 CustomerNumber = mc.CustomerNumber,
                                 ABN = mc.ABN,
                                 AddressLine1 = mca.AddressLine1,
                                 AddressLine2 = mca.AddressLine2,
                                 City = mca.City,
                                 State = mca.State,
                                 PostCode = mca.PostCode,
                                 Country = mca.Country
                             }).OrderBy(x => x.CustomerName).ToList();

                customerResponse.CustomerResponse = items;

            }
            catch (Exception ex)
            {
                customerResponse.HasError = true;
                customerResponse.ErrorMessage = ex.Message;
            }
            return customerResponse;
        }

        /// <summary>
        /// Contact name, Email and Number for particular CustomerNumber
        /// </summary>
        /// <param name="customerNumber"></param>
        /// <returns></returns>
        public ContactResponseModel GetCustomerContacts(string customerNumber, string connectionString)
        {
            var contactResponse = new ContactResponseModel();
            var mContactList = new List<ORPContact>();
            try
            {
                IDbContext dbContext = new ORPDBContext(connectionString);
                SqlParameter custNumber = new SqlParameter("CustomerNumber", customerNumber);
                SqlParameter contactType = new SqlParameter("Type", "Person");
                SqlParameter isInactive = new SqlParameter("IncludeInactive", false);
                List<spGetCustomerContactsResult> contacts = dbContext.SqlQuery<spGetCustomerContactsResult>("sp_GetCustomerContacts @CustomerNumber,@Type,@IncludeInactive", custNumber, contactType, isInactive).ToList();

                if (contacts != null && contacts.Count > 0)
                {
                    foreach (var item in contacts)
                    {
                        //mContactList.Add(new ORPContact() { ContactName = item.Name, ContactEmail = item.Email, ContactNumber = item.Mobile });
                        contactResponse.ContactResponse.Add(new ORPContact() { ContactName = item.Name, ContactEmail = item.Email, ContactNumber = item.Mobile });
                    }
                }
            }
            catch (Exception ex)
            {
                contactResponse.HasError = true;
                contactResponse.ErrorMessage = ex.Message;
            }

            return contactResponse;
        }


        public QuoteDetailsResponseModel GetQuoteDetails(string quoteId)
        {
            var response = new QuoteDetailsResponseModel();
            try
            {
                var inputQuoteId = Convert.ToInt32(quoteId);
                var funderTypes = _financeTypeRepository.Table.ToList();
                QuoteDetailsModel quoteDetailsModel = new QuoteDetailsModel();
                var existingQuote = _quotesRepository.GetById(inputQuoteId);
                if (existingQuote != null)
                {
                    quoteDetailsModel.Id = Convert.ToInt32(quoteId);
                    quoteDetailsModel.QuoteHeader = new QuoteHeader();
                    quoteDetailsModel.QuoteHeader.QuoteNumber = quoteId;
                    quoteDetailsModel.QuoteHeader.QuoteCreatedBy = existingQuote.QuoteCreatedBy;

                    quoteDetailsModel.QuoteHeader.CreatedDate = (DateTime)existingQuote.QuoteCreatedDate;
                    quoteDetailsModel.QuoteHeader.ExpiryDate = (DateTime)existingQuote.QuoteExpiryDate;
                    quoteDetailsModel.QuoteHeader.QuoteName = existingQuote.QuoteName;
                    quoteDetailsModel.QuoteHeader.FinanceType = existingQuote.QuoteFinanceType;
                    if (!string.IsNullOrEmpty(existingQuote.QuoteFinanceType))
                    {
                        int financeTypeId = 0;
                        Int32.TryParse(existingQuote.QuoteFinanceType, out financeTypeId);
                        quoteDetailsModel.QuoteHeader.FinanceTypeName = funderTypes.Where(x => x.Id == financeTypeId).FirstOrDefault()?.Description;
                    }
                    quoteDetailsModel.QuoteHeader.FunderPlan = Convert.ToString(existingQuote.FunderPlan);
                    quoteDetailsModel.QuoteHeader.QuoteType = existingQuote.QuoteFinanceLineType;
                    quoteDetailsModel.QuoteHeader.Status = existingQuote.QuoteStatus;

                    quoteDetailsModel.QuoteHeader.QuoteDuration = existingQuote.FinanceDuration;
                    quoteDetailsModel.QuoteHeader.Frequency = existingQuote.FinanceFrequency;
                    quoteDetailsModel.QuoteHeader.FunderId = existingQuote.FinanceFunder;
                    quoteDetailsModel.QuoteHeader.FinanceValue = existingQuote.FinanceValue;
                    quoteDetailsModel.QuoteHeader.QuoteTotal = existingQuote.QuoteTotal;
                    //get new columns for save funder code and gst included from db
                    quoteDetailsModel.QuoteHeader.FunderCode = existingQuote.FunderCode;
                    quoteDetailsModel.QuoteHeader.GstInclude = existingQuote.GstInclude;

                    //Selected Customer
                    quoteDetailsModel.SelectedCustomer = new ORPCustomer();
                    quoteDetailsModel.SelectedCustomer.DisplayLabel = existingQuote.ResellerAccount + "-" + existingQuote.ResellerName;
                    quoteDetailsModel.SelectedCustomer.CustomerNumber = existingQuote.ResellerAccount;
                    quoteDetailsModel.SelectedCustomer.CustomerName = existingQuote.ResellerName;
                    quoteDetailsModel.SelectedCustomer.ABN = existingQuote.ResellerABN;
                    quoteDetailsModel.SelectedCustomer.AddressLine1 = existingQuote.ResellerAddressLine1;
                    quoteDetailsModel.SelectedCustomer.AddressLine2 = existingQuote.ResellerAddressLine2;
                    quoteDetailsModel.SelectedCustomer.City = existingQuote.ResellerCity;
                    quoteDetailsModel.SelectedCustomer.State = existingQuote.ResellerState;
                    quoteDetailsModel.SelectedCustomer.PostCode = existingQuote.ResellerPostcode;
                    quoteDetailsModel.SelectedCustomer.Country = existingQuote.ResellerCountry;

                    //Reseller/Customer Details
                    quoteDetailsModel.CustomerDetails.CustomerNumber = existingQuote.ResellerAccount;
                    quoteDetailsModel.CustomerDetails.CustomerName = existingQuote.ResellerName;
                    quoteDetailsModel.CustomerDetails.CustomerABN = existingQuote.ResellerABN;
                    quoteDetailsModel.CustomerDetails.CustomerAddressLine1 = existingQuote.ResellerAddressLine1;
                    quoteDetailsModel.CustomerDetails.CustomerAddressLine2 = existingQuote.ResellerAddressLine2;
                    quoteDetailsModel.CustomerDetails.CustomerAddressCity = existingQuote.ResellerCity;
                    quoteDetailsModel.CustomerDetails.CustomerAddressState = existingQuote.ResellerState;
                    quoteDetailsModel.CustomerDetails.CustomerPostCode = existingQuote.ResellerPostcode;
                    quoteDetailsModel.CustomerDetails.CustomerContact = existingQuote.ResellerContactName;
                    quoteDetailsModel.CustomerDetails.CustomerEmail = existingQuote.ResellerContactEmail;
                    quoteDetailsModel.CustomerDetails.CustomerPhone = existingQuote.ResellerContactPhone;
                    quoteDetailsModel.CustomerDetails.CustomerCountry = existingQuote.ResellerCountry;

                    //EndCustomer Details
                    quoteDetailsModel.EndUserDetails.EndCustomerName = existingQuote.EndCustomerName;
                    quoteDetailsModel.EndUserDetails.EndCustomerContact = existingQuote.EndCustomerContactName;
                    quoteDetailsModel.EndUserDetails.EndCustomerEmail = existingQuote.EndCustomerContactEmail;
                    quoteDetailsModel.EndUserDetails.EndCustomerPhone = existingQuote.EndCustomerContactPhone;
                    quoteDetailsModel.EndUserDetails.EndCustomerABN = existingQuote.EndCustomerABN;
                    quoteDetailsModel.EndUserDetails.EndCustomerYearsTrading = existingQuote.EndCustomerYearsTrading;
                    quoteDetailsModel.EndUserDetails.EndCustomerAddressLine1 = existingQuote.EndCustomerPrimaryAddressLine1;
                    quoteDetailsModel.EndUserDetails.EndCustomerAddressLine2 = existingQuote.EndCustomerPrimaryAddressLine2;
                    quoteDetailsModel.EndUserDetails.EndCustomerCity = existingQuote.EndCustomerPrimaryCity;
                    quoteDetailsModel.EndUserDetails.EndCustomerState = existingQuote.EndCustomerPrimaryState;
                    quoteDetailsModel.EndUserDetails.EndCustomerPostCode = existingQuote.EndCustomerPrimaryPostcode;
                    quoteDetailsModel.EndUserDetails.EndCustomerCountry = existingQuote.EndCustomerPrimaryCountry;
                    quoteDetailsModel.EndUserDetails.AuthorisedSignatoryName = existingQuote.EndCustomerSignatoryName;
                    quoteDetailsModel.EndUserDetails.AuthorisedSignatoryPosition = existingQuote.EndCustomerSignatoryPosition;

                    //QuoteLines
                    var existingQuoteLines = (from quoteLines in _quoteLinesRepository.Table where quoteLines.QuoteId == inputQuoteId select quoteLines).ToList();
                    var types = _typesRepository.Table.ToList();
                    var categories = _categoriesRepository.Table.ToList();

                    foreach (var quoteLines in existingQuoteLines)
                    {
                        var quoteLine = new QuoteLine();


                        quoteLine.LineNumber = quoteLines.QuoteLineNumber;
                        quoteLine.IMSKU = quoteLines.SKU;
                        quoteLine.VPN = quoteLines.VPN;

                        quoteLine.Item = Convert.ToString(quoteLines.TypeID);
                        if (quoteLines.TypeID.HasValue)
                        {
                            quoteLine.ItemName = types.Where(x => x.Id == quoteLines.TypeID).FirstOrDefault().Description;
                        }
                        quoteLine.Category = Convert.ToString(quoteLines.CategoryID);
                        if (quoteLines.CategoryID.HasValue)
                        {
                            quoteLine.CategoryName = categories.Where(x => x.Id == quoteLines.CategoryID).FirstOrDefault().Description;
                        }
                        quoteLine.Category = Convert.ToString(quoteLines.CategoryID);
                        quoteLine.CostPrice = quoteLines.IngramSellPrice;
                        quoteLine.Qty = (int)quoteLines.Qty;
                        quoteLine.Margin = (decimal?)quoteLines.ResellerMarginPercent;
                        quoteLine.SalePrice = quoteLines.ResellerSellPrice;
                        quoteLine.TotalGST = quoteLines.LineGST;
                        quoteLine.Description = quoteLines.Description;
                        quoteLine.LineTotal = quoteLines.LineTotal;
                        quoteLine.FinanceProductTypeID = quoteLines.FinanceProductTypeID;

                        //add item to list
                        quoteDetailsModel.QuoteLines.Add(quoteLine);
                    }

                    response.QuoteDetails = quoteDetailsModel;

                }

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }


        /// <summary>
        /// Search Quote Number
        /// </summary>
        /// <param name="quoteSearchModel"></param>
        /// <returns></returns>
        /// <summary>      
        public QuoteSearchResponseModel SearchQuote(QuoteSearchModel quoteSearchModel, string resellerId)
        {
            var response = new QuoteSearchResponseModel();
            try
            {
                var query = from quotes in _quotesRepository.Table select quotes;
                if (quoteSearchModel.CreatedDate != null)
                {
                    var createdDate = quoteSearchModel.CreatedDate.Value.ToLocalTime();
                    query = query.Where(q => q.QuoteCreatedDate >= createdDate.Date);
                }

                if (quoteSearchModel.ExpiryDate != null)
                {
                    var expiryDate = quoteSearchModel.ExpiryDate.Value.ToLocalTime();
                    query = query.Where(q => q.QuoteExpiryDate >= expiryDate);
                }

                if (!string.IsNullOrEmpty(quoteSearchModel.QuoteFinanceType))
                {
                    query = query.Where(q => q.QuoteFinanceType == quoteSearchModel.QuoteFinanceType);
                }

                if (quoteSearchModel.QuoteStatus != null)
                {
                    query = query.Where(q => q.QuoteStatus == quoteSearchModel.QuoteStatus);
                }

                if (!string.IsNullOrEmpty(quoteSearchModel.EndUser))
                {
                    query = query.Where(q => q.EndCustomerName.Contains(quoteSearchModel.EndUser));
                }

                if (quoteSearchModel.QuoteNumber != null)
                {
                    query = query.Where(q => q.Id == quoteSearchModel.QuoteNumber);
                }

                if (!string.IsNullOrEmpty(resellerId))
                {
                    query = query.Where(q => q.ResellerAccount == resellerId);
                }

                //Quote Status
                var status = (from st in _statusRepository.Table select st).ToList();

                var results = query.Select(q => new QuoteSearchResponse
                {
                    QuoteNumber = q.Id,
                    QuoteName = q.QuoteName,
                    DisplayLabel = q.Id + "-" + q.QuoteName,
                    QuoteStatus = q.QuoteStatus,
                    EndCustomer = q.EndCustomerName,
                    QuoteFinanceType = (!string.IsNullOrEmpty(q.QuoteFinanceType)) ? q.QuoteFinanceType : string.Empty,
                    QuoteTotal = q.QuoteTotal,
                    CreatedDate = q.QuoteCreatedDate,
                    ExpiryDate = q.QuoteExpiryDate,
                    ResellerAccount = q.ResellerAccount,
                    ResellerName = q.ResellerName
                }).ToList();

                foreach (var result in results)
                {
                    result.QuoteStatusDescr = status.Where(st => st.Id == result.QuoteStatus).FirstOrDefault().Description;
                }

                //Set the response
                response.SearchResult = results;

            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Search Quote Number with contains
        /// </summary>
        /// <param name="quoteSearchModel"></param>
        /// <returns></returns>
        public QuoteSearchResponseModel LookupQuoteNumber(QuoteSearchModel quoteSearchModel, string resellerId)
        {
            var response = new QuoteSearchResponseModel();
            try
            {
                var query = from quotes in _quotesRepository.Table select quotes;
                if (quoteSearchModel.CreatedDate != null)
                {
                    var createdDate = quoteSearchModel.CreatedDate.Value.ToLocalTime();
                    query = query.Where(q => q.QuoteCreatedDate >= createdDate.Date);
                }

                if (quoteSearchModel.ExpiryDate != null)
                {
                    var expiryDate = quoteSearchModel.ExpiryDate.Value.ToLocalTime();
                    query = query.Where(q => q.QuoteExpiryDate >= expiryDate);
                }

                if (!string.IsNullOrEmpty(quoteSearchModel.QuoteFinanceType))
                {
                    string[] getEachQuoteFinanceType = null;
                    if (quoteSearchModel.QuoteFinanceType.IndexOf(',') != -1)
                    {
                        getEachQuoteFinanceType = quoteSearchModel.QuoteFinanceType.Split(',');
                        if (getEachQuoteFinanceType.Length > 0)
                        {
                            if (getEachQuoteFinanceType.Length == 3)
                            {
                                query = query.Where(q => q.QuoteFinanceType == getEachQuoteFinanceType[0] || q.QuoteFinanceType == getEachQuoteFinanceType[1] || q.QuoteFinanceType == getEachQuoteFinanceType[2]);
                            }
                            if (getEachQuoteFinanceType.Length == 2)
                            {
                                query = query.Where(q => q.QuoteFinanceType == getEachQuoteFinanceType[0] || q.QuoteFinanceType == getEachQuoteFinanceType[1]);
                            }
                        }
                    }
                    else
                        query = query.Where(q => q.QuoteFinanceType == quoteSearchModel.QuoteFinanceType);
                }

                if (quoteSearchModel.QuoteStatus != null)
                {
                    query = query.Where(q => q.QuoteStatus == quoteSearchModel.QuoteStatus);
                }

                if (!string.IsNullOrEmpty(quoteSearchModel.EndUser))
                {
                    query = query.Where(q => q.EndCustomerName.Contains(quoteSearchModel.EndUser));
                }

                //Search Quote Number with contains
                if (quoteSearchModel.QuoteNumber != null)
                {
                    string filter = Convert.ToString(quoteSearchModel.QuoteNumber);
                    query = query.Where(q => q.Id.ToString().Contains(filter));
                }

                if (quoteSearchModel.ResellerAccount != null && quoteSearchModel.ResellerAccount > 0)
                    resellerId = Convert.ToString(quoteSearchModel.ResellerAccount);
                if (!string.IsNullOrEmpty(resellerId))
                {
                    query = query.Where(q => q.ResellerAccount == resellerId);
                }

                if (!string.IsNullOrEmpty(resellerId))
                {
                    query = query.Where(q => q.ResellerAccount == resellerId);
                }

                //Quote Status
                var status = (from st in _statusRepository.Table select st).ToList();

                var results = query.Select(q => new QuoteSearchResponse
                {
                    QuoteNumber = q.Id,
                    QuoteName = q.QuoteName,
                    DisplayLabel = q.Id + "-" + q.QuoteName,
                    QuoteStatus = q.QuoteStatus,
                    EndCustomer = q.EndCustomerName,
                    QuoteFinanceType = (!string.IsNullOrEmpty(q.QuoteFinanceType)) ? q.QuoteFinanceType : string.Empty,
                    QuoteTotal = q.QuoteTotal,
                    CreatedDate = q.QuoteCreatedDate,
                    ExpiryDate = q.QuoteExpiryDate,
                    ResellerAccount = q.ResellerAccount,
                    ResellerName = q.ResellerName
                }).ToList();

                foreach (var result in results)
                {
                    result.QuoteStatusDescr = status.Where(st => st.Id == result.QuoteStatus).FirstOrDefault().Description;
                }

                //Set the response
                response.SearchResult = results;
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public void InsertQuoteLog(int quoteId, string type, string description)
        {
            _quoteLogRepository.Insert(new QuoteLog() { CreatedBy = "LoggedInUser", Description = description, QuoteId = quoteId, Timestamp = DateTime.Now, Type = type });
        }

        public void UpdateQuoteStatus(int quoteId, int status)
        {
            var quote = _quotesRepository.GetById(quoteId);
            if (quote != null)
            {
                var quoteStatus = (from st in _statusRepository.Table where st.Id == status select st).FirstOrDefault();

                if (quoteStatus != null)
                {
                    quote.QuoteStatus = quoteStatus.Id;
                    quote.QuoteLastModified = DateTime.Now.ToLocalTime();
                    _quotesRepository.Update(quote);
                }

            }
        }

        public List<RecentQuotesModel> GetRecentQuotes(string resellerId)
        {
            var response = new List<RecentQuotesModel>();
            try
            {
                return (from quotes in _quotesRepository.Table
                        join status in _statusRepository.Table on quotes.QuoteStatus equals status.Id
                        where quotes.ResellerAccount == resellerId
                        select new RecentQuotesModel
                        {
                            QuoteNumber = quotes.Id,
                            QuoteName = quotes.QuoteName != null || quotes.QuoteName != "" ? quotes.QuoteName : "N/A",
                            EndCustomerName = quotes.EndCustomerName != null || quotes.EndCustomerName != "" ? quotes.EndCustomerName : "N/A",
                            QuoteTotal = quotes.QuoteTotal,
                            Status = status.Description != null || status.Description != "" ? status.Description : "N/A",
                            QuoteExpiryDate = quotes.QuoteExpiryDate,
                            QuoteCreatedDate = quotes.QuoteCreatedDate,
                            QuoteLastModified = quotes.QuoteLastModified,
                        }).OrderByDescending(x => x.QuoteLastModified).Take(10).ToList();

            }
            catch (Exception ex) //further can be modify by sending various types of values/exception
            {
                return null;
            }
        }

        public async Task<RestResponse<AddressList>> GetAddresses(string grantType, string clientID, string clientSecret, string jsonString)
        {
            try
            {
                string authToken = await GetAuthToken(grantType, clientID, clientSecret);
                if (authToken != string.Empty)
                {
                    //convert object request string to model.
                    var objRequest = JsonConvert.DeserializeObject<AddressRequest>(jsonString);

                    //map api url.
                    var client = new RestClient("https://api-beta.ingrammicro.com:443/");
                    //bind controller api method.
                    var request = new RestRequest("v1/address/validate", Method.Post);
                    //added header
                    request.AddHeader("Accept", "application/json");
                    request.AddHeader("IM-CutomerNumber", "");
                    request.AddHeader("IM-CountryCode", "");
                    request.AddHeader("IM-CorrelationID", "4f514cd7-62c2-4c69-a5e9-28cd05b29878");
                    request.AddHeader("IM-SenderID", "");
                    request.AddHeader("Authorization", "Bearer " + authToken);
                    //set oject request in body.
                    request.AddJsonBody(objRequest);
                    //execute request and get result response.
                    var response = await client.ExecuteAsync<AddressList>(request);

                    if (response.IsSuccessful)
                    {
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }


        //[HttpPost]
        //public async Task<ActionResult<AddressResponse>> Post(AddressRequest _request)
        //{
        //    try
        //    {
        //        #region 

        //        string authToken = String.Empty;

        //        string token = string.Empty;
        //        var client1 = new RestClient("https://api-beta.ingrammicro.com/");
        //        var request1 = new RestRequest("oauth/oauth20/token?grant_type=client_credentials&client_id=AUORPAPPS1&client_secret=QIpwb4H8P7yrUM51Zsamq30tGqHFCds8", Method.Get);
        //        RestResponse<AddressModel> response1 = await client1.ExecuteAsync<AddressModel>(request1);
        //        if (response1 != null)
        //        {
        //            var config = new Config();
        //            var addressModel = JsonConvert.DeserializeObject<AddressModel>(response1.Content);

        //            config.name = addressModel.token_type;
        //            config.value = addressModel.access_token;
        //            Double minutes = TimeSpan.FromMilliseconds(Convert.ToDouble(response1.Data.expires_in)).TotalMinutes;
        //            config.expireDateTime = DateTime.Now.AddMinutes(minutes);
        //            authToken = addressModel.access_token;
        //        }

        //        #endregion

        //        var _response = new AddressResponse();

        //        RestClient client = new RestClient("https://api-beta.ingrammicro.com:443/");
        //        RestRequest request = new RestRequest("v1/address/validate", Method.Post);
        //        request.AddHeader("Accept", "application/json");
        //        request.AddHeader("IM-CutomerNumber", "");
        //        request.AddHeader("IM-CountryCode", "");
        //        request.AddHeader("IM-CorrelationID", "4f514cd7-62c2-4c69-a5e9-28cd05b29878");
        //        request.AddHeader("IM-SenderID", "");
        //        request.AddHeader("Authorization", "Bearer " + authToken);
        //        request.AddJsonBody(_request);

        //        RestResponse<AddressResponse> response = await client.ExecuteAsync<AddressResponse>(request);


        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //            return StatusCode(StatusCodes.Status200OK,
        //                response.Content);
        //        else
        //            return StatusCode(StatusCodes.Status412PreconditionFailed,
        //           "Somthing went wrong.");
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            "Error creating new employee record");
        //    }
        //}

        public async Task<string> GetAuthToken(string grantType, string clientID, string clientSecret)
        {
            string authToken = String.Empty;
            var data = (from configData in _configRepository.Table where configData.expireDateTime > DateTime.Now select configData).FirstOrDefault();
            if (data != null)
            {
                authToken = data.value;
            }
            else
            {
                string token = string.Empty;
                var client = new RestClient("https://api-beta.ingrammicro.com/");
                var request = new RestRequest("oauth/oauth20/token?grant_type=" + grantType + "&client_id=" + clientID + "&client_secret=" + clientSecret, Method.Get);
                RestResponse<AddressModel> response = await client.ExecuteAsync<AddressModel>(request);
                if (response != null)
                {
                    var addressModel = JsonConvert.DeserializeObject<AddressModel>(response.Content);
                    //below id code is temporary and can be change after Login implementation 
                    var config = _configRepository.Table.FirstOrDefault();
                    if (config != null)
                    {
                        config.name = addressModel.token_type;
                        config.value = addressModel.access_token;
                        Double minutes = TimeSpan.FromMilliseconds(Convert.ToDouble(response.Data.expires_in)).TotalMinutes;
                        config.expireDateTime = DateTime.Now.AddMinutes(minutes);
                        _configRepository.Update(config);
                    }
                    else
                    {
                        config = new Web.Models.DBModel.Config();
                        config.name = addressModel.token_type;
                        config.value = addressModel.access_token;
                        Double minutes = TimeSpan.FromMilliseconds(Convert.ToDouble(response.Data.expires_in)).TotalMinutes;
                        config.expireDateTime = DateTime.Now.AddMinutes(minutes);
                        _configRepository.Insert(config);
                    }

                    authToken = addressModel.access_token;
                }
            }
            return authToken;
        }

        #region Quote Attachment Manager


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="uploadBy"></param>
        /// <param name="physicalPath"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<FileUploadResponse> UploadFiles(int id, int? applicationNumber, int source, Guid? uploadBy, string physicalPath, string description, List<IFormFile> files)
        {
            try
            {
                List<FileUploadResponseData> uploadedFiles = new List<FileUploadResponseData>();

                //FileDetail fileDetail = new FileDetail();
                foreach (var file in files)
                {
                    var fileType = Path.GetExtension(file.FileName);
                    if (fileType.ToLower() == ".pdf" || fileType.ToLower() == ".docx" || fileType.ToLower() == ".doc" || fileType.ToLower() == ".xls" || fileType.ToLower() == ".xlsx" || fileType.ToLower() == ".jpg" || fileType.ToLower() == ".png" || fileType.ToLower() == ".jpeg" || fileType.ToLower() == ".csv")
                    {
                        var filePath = physicalPath;
                        var OrignalName = Path.GetFileName(file.FileName);

                        if (file != null && file.Length > 0)
                        {
                            var attachment = new IMFS.Web.Models.DBModel.Attachments();

                            if (source == (int)Source.Application)
                            {
                                attachment.ApplicationId = id;
                                attachment.ApplicationNumber = applicationNumber;
                            }

                            else
                            {
                                attachment.QuoteId = id;
                            }
                            attachment.Source = source;
                            attachment.FileName = OrignalName;
                            attachment.Description = description;
                            attachment.CreatedDate = System.DateTime.Now;

                            _mQuoteAttachmentsRepository.Insert(attachment);

                            var fileName = attachment.Id + fileType;

                            string url = Path.Combine(filePath, id.ToString(), fileName);
                            using (var stream = new FileStream(url, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            attachment.PhysicalPath = url;
                            _mQuoteAttachmentsRepository.Update(attachment);

                        }
                        else
                        {
                            uploadedFiles.Add(new FileUploadResponseData() { Status = "Error", Message = "File is empty." });
                        }
                    }
                    else
                    {
                        uploadedFiles.Add(new FileUploadResponseData() { Status = "Error", Message = "File extension not allowed" });
                    }
                }

                return new FileUploadResponse() { Data = uploadedFiles, Message = "" };
            }
            catch (Exception ex)
            {
                return new FileUploadResponse() { Message = ex.Message.ToString() };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task<string> RemoveQuoteFiles(int fileId)
        {
            try
            {
                var quoteAttachment = _mQuoteAttachmentsRepository.Table
                    .Where(f => f.Id == fileId)
                    .FirstOrDefault();

                if (quoteAttachment != null)
                {
                    _mQuoteAttachmentsRepository.Delete(quoteAttachment);

                    File.Delete(quoteAttachment.PhysicalPath);

                    return "Success";
                }
                else
                    return "File not found";
            }
            catch (Exception ex)
            {
                return ex.Message;

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>

        public async Task<List<AttachmentsResponse>> GetQuoteFiles(int id, int source)

        {
            List<AttachmentsResponse> files = new List<AttachmentsResponse>();
            try
            {
                var quoteAttachments = _mQuoteAttachmentsRepository.Table.Where(x => x.QuoteId == id).ToList();
                if (quoteAttachments != null && quoteAttachments.Count > 0)
                {
                    foreach (var item in quoteAttachments)
                    {
                        AttachmentsResponse file = new AttachmentsResponse();

                        file.FileId = item.Id;
                        file.QuoteId = item.QuoteId;
                        file.Description = item.Description;
                        file.CreatedDate = item.CreatedDate;
                        file.FileName = item.FileName;
                        file.UploadBy = item.UploadBy;

                        files.Add(file);
                    }
                }

                //var collection = _mQuoteAttachmentsRepository.Table.Where(x => x.ApplicationId == id).ToList();
                var appAttachments = _mQuoteAttachmentsRepository.Table.Where(x => x.ApplicationNumber == id).ToList();
                if (appAttachments != null && appAttachments.Count > 0)
                {
                    foreach (var item in appAttachments)
                    {
                        AttachmentsResponse file = new AttachmentsResponse();

                        file.FileId = item.Id;
                        file.ApplicationId = item.ApplicationId;
                        file.Description = item.Description;
                        file.CreatedDate = item.CreatedDate;
                        file.FileName = item.FileName;
                        file.UploadBy = item.UploadBy;

                        files.Add(file);
                    }
                }

                return files;
            }
            catch (Exception ex)
            {
                return files;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FileDownloadResponse> DownloadFile(int id)
        {
            var result = new FileDownloadResponse() { IsResult = false };
            try
            {
                var fileInfo = _mQuoteAttachmentsRepository.Table
                    .Where(f => f.Id == id)
                    .SingleOrDefault();

                if (fileInfo != null)
                {
                    result.IsResult = true;
                    result.Message = string.Empty;
                    result.FileName = fileInfo.FileName;
                    result.PhysicalPath = fileInfo.PhysicalPath;
                }
                else
                {
                    result.Message = "File not found.";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        #endregion

        #region Get product details by sku or vpn
        private DataTable InitStringIDs(string param)
        {
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("ID");
            DataRow drvpn = dt.NewRow();
            drvpn["ID"] = param;
            dt.Rows.Add(drvpn);

            DataRow drsku = dt.NewRow();
            drsku["ID"] = param;
            dt.Rows.Add(drsku);

            return dt;
        }



        public async Task<ProductDetailResponse> GetProductsBySKUAndVPN(string param, string connectionString)
        {
            try
            {
                DataTable dt = InitStringIDs(param);

                List<ProductDetailsResult> _products = new List<ProductDetailsResult>();

                IDbContext dbContext = new ORPDBContext(connectionString);
                SqlParameter skus = new SqlParameter("SKU", dt) { SqlDbType = SqlDbType.Structured, TypeName = "[dbo].[StringIDs]", Direction = ParameterDirection.Input };
                SqlParameter vpns = new SqlParameter("VPN", dt) { SqlDbType = SqlDbType.Structured, TypeName = "[dbo].[StringIDs]", Direction = ParameterDirection.Input };
                List<spGetProductDetailsResult> products = dbContext.SqlQuery<spGetProductDetailsResult>("sp_GetProductDetails @SKU,@VPN", skus, vpns).ToList();


                if (products != null && products.Count > 0)
                {
                    foreach (var product in products)
                    {
                        ProductDetailsResult _product = new ProductDetailsResult();

                        _product.ProductDescription = product.ProductDescription;
                        _product.InternalSKUID = product.InternalSKUID;
                        _product.VendorSKUID = product.VendorSKUID;
                        _product.Vsrid = product.VSRID;
                        _product.CurrencyCode = product.CurrencyCode;
                        _product.UnitNetAmount = product.NettSalesCost;
                        _products.Add(_product);
                    }
                }
                return new ProductDetailResponse() { Products = _products, Status = StatusCodes.Status200OK, Message = "Success" };
            }
            catch (Exception ex)
            {
                return new ProductDetailResponse() { Products = null, Status = StatusCodes.Status500InternalServerError, Message = ex.Message };
            }
        }


        #region Quote Email Atachments

        public List<EmailAttachmentsResponse> GetQuoteAttachments(List<int> documentIDs)
        {
            List<EmailAttachmentsResponse> files = new List<EmailAttachmentsResponse>();
            try
            {
                if (documentIDs != null && documentIDs.Count > 0)
                {
                    foreach (var documentID in documentIDs)
                    {
                        var document = _mQuoteAttachmentsRepository.Table.Where(x => x.Id == documentID).FirstOrDefault();

                        if (document != null)
                        {
                            EmailAttachmentsResponse file = new EmailAttachmentsResponse();

                            file.FileId = document.Id;
                            file.QuoteId = document.QuoteId;
                            file.Description = document.Description;
                            file.CreatedDate = document.CreatedDate;
                            file.FileName = document.FileName;
                            file.PhysicalPath = document.PhysicalPath;

                            files.Add(file);
                        }
                    }
                }

                return files;
            }
            catch (Exception ex)
            {
                return files;
            }
        }

        #endregion

        #endregion
    }
}
