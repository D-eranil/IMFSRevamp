using IMFS.Web.Models.Quote;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ORP.Core.Extensions;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static ORP.Core.Extensions.EpplusExtensions;

namespace IMFS.BusinessLogic.Quote
{
    public class QuoteDownloadManager : IQuoteDownloadManager
    {

        private readonly IQuoteManager _quoteManager;
        private readonly IConfiguration _configuration;

        public QuoteDownloadManager(IQuoteManager quoteManager,
            IConfiguration configuration)

        {
            _quoteManager = quoteManager;
            _configuration = configuration;

        }

        public QuoteDownloadResponse DownloadQuote(QuoteDownloadInput inputModel)
        {
            if (inputModel.DownloadMode == "Proposal")
            {
                return downloadProposal(inputModel.QuoteId);
            }
            else
            {
                return downloadExcel(inputModel.QuoteId);
            }

        }

        private QuoteDownloadResponse downloadExcel(int quoteId)
        {
            var response = new QuoteDownloadResponse();
            try
            {
                string templatesFolderPath = _configuration.GetValue<string>("TemplatesFolderPath");
                templatesFolderPath = templatesFolderPath + "Quotes\\" + _configuration.GetValue<string>("CountryCode") + "\\";

                var quoteDetailsResponse = _quoteManager.GetQuoteDetails(quoteId.ToString());
                if (quoteDetailsResponse.HasError)
                {
                    response.HasError = true;
                    response.ErrorMessage = quoteDetailsResponse.ErrorMessage;
                    return response;
                }
                if (quoteDetailsResponse.QuoteDetails == null)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Cannot read quote details of " + quoteId.ToString();
                    return response;
                }

                var quoteDetails = quoteDetailsResponse.QuoteDetails;
                bool isCategoryQuote = false;
                var categories = quoteDetailsResponse.QuoteDetails.QuoteLines.Where(x => !string.IsNullOrEmpty(x.Category)).Select(x => x.Category).ToList();
                if (categories != null && categories.Count > 0)
                {
                    isCategoryQuote = true;
                }

                FileInfo fileInfo;

                if (isCategoryQuote)
                {
                    fileInfo = new FileInfo(templatesFolderPath + "CategoryTemplate.xlsx");

                }
                else
                {
                    fileInfo = new FileInfo(templatesFolderPath + "SKUTemplate.xlsx");
                }

                using (ExcelPackage excel = new ExcelPackage(fileInfo))
                {
                    var workbook = excel.Workbook;


                    ExcelWorksheet sheet = excel.Workbook.Worksheets.FirstOrDefault();


                    if (sheet == null)
                    {
                        response.HasError = true;
                        response.ErrorMessage = "Sheet is empty " + templatesFolderPath;
                        return response;
                    }
                    sheet.Name = quoteId.ToString();

                    var cellStyleLeftAlign = EpplusExtensions.GetStyle(sheet, EpplusExtensions.EpplusStyle.NormalLeftAlign);
                    var cellStyleCenterAlign = EpplusExtensions.GetStyle(sheet, EpplusExtensions.EpplusStyle.NormalCenterAlign);
                    var cellStyleRightAlign = EpplusExtensions.GetStyle(sheet, EpplusExtensions.EpplusStyle.NormalRightAlign);

                    int rowShift = 3;
                    SetHeaderProperties(quoteDetails, sheet, rowShift, 6);

                    int rowIndex = 20;
                    int itemStartIndex = 20;
                    foreach (var quoteLine in quoteDetails.QuoteLines)
                    {
                        EpplusExtensions.InsertNewRow(sheet, itemStartIndex, rowIndex, false);
                        if (isCategoryQuote)
                        {
                            WriteCategoryDetails(quoteLine, sheet, rowIndex);
                        }
                        else
                        {
                            WriteSKUDetails(quoteLine, sheet, rowIndex);
                        }
                        rowIndex++;
                    }

                    #region insert quote total
                    rowIndex++;
                    var cellStyleTotal = EpplusExtensions.GetStyle(sheet, EpplusExtensions.EpplusStyle.TotalsStyle);

                    int totalColumnPosition = 7;
                    if (isCategoryQuote)
                    {
                        totalColumnPosition = 6;
                    }
                    sheet.EpplusSetCellValue(rowIndex, totalColumnPosition, quoteDetails.QuoteHeader.QuoteTotal.Value.ToString("C"), cellStyleTotal.Name);  // Quote Total
                    #endregion

                    #region Frequency and Finance Type
                    rowIndex++;
                    string label = quoteDetails.QuoteHeader.Frequency + " " + quoteDetails.QuoteHeader.FinanceTypeName + ":";
                    sheet.EpplusSetCellValue(rowIndex, totalColumnPosition - 1, label, cellStyleTotal.Name);  // Finance Type label
                    if (quoteDetails.QuoteHeader.FinanceValue.HasValue)
                    {
                        sheet.EpplusSetCellValue(rowIndex, totalColumnPosition, quoteDetails.QuoteHeader.FinanceValue.Value.ToString("C"), cellStyleTotal.Name);  // Finance Value
                    }
                    #endregion

                    #region insert duration
                    rowIndex++;
                    sheet.EpplusSetCellValue(rowIndex, totalColumnPosition, quoteDetails.QuoteHeader.QuoteDuration, cellStyleTotal.Name);  // Quote Duration
                    #endregion

                    #region update OPEX Proposal

                    #endregion

                    workbook.Worksheets.FirstOrDefault().Select();
                    string quoteName = Regex.Replace(quoteDetails.QuoteHeader.QuoteName, "[^a-zA-Z0-9 -]", "");
                    response.FileName = "IMFS-" + quoteDetails.Id + "-" + quoteName + ".xlsx";
                    if (response.FileName.Length > 200)
                    {
                        response.FileName = response.FileName.Substring(0, 200);
                    }
                    response.DownloadFile = excel.GetAsByteArray();
                }



            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.ToString();
            }
            return response;
        }

        protected void SetHeaderProperties(QuoteDetailsModel quote, ExcelWorksheet sheet, int rowShift, int secondColumnIndex)
        {
            #region set header properties

            // End User Name
            sheet.EpplusSetCellValue(rowShift + 3, 3, quote.EndUserDetails.EndCustomerName);

            // EU Contact Name
            sheet.EpplusSetCellValue(rowShift + 4, 3, quote.EndUserDetails.EndCustomerContact);
            // EU Contact Email
            sheet.EpplusSetCellValue(rowShift + 5, 3, quote.EndUserDetails.EndCustomerEmail);

            // Quote By
            sheet.EpplusSetCellValue(rowShift + 7, 3, quote.CustomerDetails.CustomerContact);
            // Email
            sheet.EpplusSetCellValue(rowShift + 8, 3, quote.CustomerDetails.CustomerEmail);
            // Phone
            sheet.EpplusSetCellValue(rowShift + 9, 3, quote.CustomerDetails.CustomerPhone);
            // Valid From
            sheet.EpplusSetCellValue(rowShift + 10, 3, quote.QuoteHeader.CreatedDate.ToString("dd MMM yyyy"), null, "dd MMM yyyy");
            // Valid To
            sheet.EpplusSetCellValue(rowShift + 11, 3, quote.QuoteHeader.ExpiryDate.ToString("dd MMM yyyy"), null, "dd MMM yyyy");

            // QuoteId
            sheet.EpplusSetCellValue(rowShift + 7, secondColumnIndex, quote.Id); ;
            // QuoteName
            sheet.EpplusSetCellValue(rowShift + 8, secondColumnIndex, quote.QuoteHeader.QuoteName);

            //Add Style
            var allCells = sheet.Cells[rowShift + 3, 3, rowShift + 11, secondColumnIndex];

            var cellFont = allCells.Style.Font;
            cellFont.SetFromFont(new Font("Calibri", 10));
            cellFont.Bold = false;

            #endregion set header properties
        }

        protected void WriteSKUDetails(QuoteLine quoteLine, ExcelWorksheet sheet, int rowIndex)
        {
            var cellStyleLeftAlign = EpplusExtensions.GetStyle(sheet, EpplusExtensions.EpplusStyle.NormalLeftAlign);
            var cellStyleCenterAlign = EpplusExtensions.GetStyle(sheet, EpplusExtensions.EpplusStyle.NormalCenterAlign);
            var cellStyleRightAlign = EpplusExtensions.GetStyle(sheet, EpplusExtensions.EpplusStyle.NormalRightAlign);


            sheet.EpplusSetCellValue(rowIndex, 2, quoteLine.IMSKU, cellStyleLeftAlign.Name);
            sheet.EpplusSetCellValue(rowIndex, 3, quoteLine.VPN, cellStyleLeftAlign.Name);
            sheet.EpplusSetCellValue(rowIndex, 4, quoteLine.Description, cellStyleLeftAlign.Name);

            sheet.EpplusSetCellValue(rowIndex, 5, quoteLine.Qty, cellStyleCenterAlign.Name);

            sheet.EpplusSetCellValue(rowIndex, 6, quoteLine.SalePrice, cellStyleRightAlign.Name, "$###,###,##0.00");  // Sales Pricing
            sheet.EpplusSetCellValue(rowIndex, 7, quoteLine.LineTotal, cellStyleRightAlign.Name, "$###,###,##0.00");  // Line Total
        }

        protected void WriteCategoryDetails(QuoteLine quoteLine, ExcelWorksheet sheet, int rowIndex)
        {
            var cellStyleLeftAlign = EpplusExtensions.GetStyle(sheet, EpplusExtensions.EpplusStyle.NormalLeftAlign);
            var cellStyleCenterAlign = EpplusExtensions.GetStyle(sheet, EpplusExtensions.EpplusStyle.NormalCenterAlign);
            var cellStyleRightAlign = EpplusExtensions.GetStyle(sheet, EpplusExtensions.EpplusStyle.NormalRightAlign);


            sheet.EpplusSetCellValue(rowIndex, 2, quoteLine.ItemName, cellStyleLeftAlign.Name);
            sheet.EpplusSetCellValue(rowIndex, 3, quoteLine.CategoryName, cellStyleLeftAlign.Name);

            sheet.EpplusSetCellValue(rowIndex, 5, quoteLine.Qty, cellStyleCenterAlign.Name);
            // sheet.EpplusSetCellValue(rowIndex, 6, quoteLine.SOH, cellStyleCenterAlign.Name);
            //if (quoteLine.RRP.HasValue)
            //{
            //    sheet.EpplusSetCellValue(rowIndex, 7, quoteLine.RRP.ToDouble(), cellStyleRightAlign.Name, "$###,###,##0.00");  // RRP Pricing
            //}
            //else
            //{
            //    sheet.EpplusSetCellValue(rowIndex, 7, string.Empty, cellStyleRightAlign.Name); // RRP Pricing
            //}

            sheet.EpplusSetCellValue(rowIndex, 7, quoteLine.SalePrice, cellStyleRightAlign.Name, "$###,###,##0.00");  // Sales Pricing
            sheet.EpplusSetCellValue(rowIndex, 8, quoteLine.LineTotal, cellStyleRightAlign.Name, "$###,###,##0.00");  // Line Total
        }

        protected QuoteDownloadResponse downloadProposal(int quoteId)
        {
            var response = new QuoteDownloadResponse();
            try
            {
                string templatesFolderPath = _configuration.GetValue<string>("TemplatesFolderPath");
                templatesFolderPath = templatesFolderPath + "Quotes\\" + _configuration.GetValue<string>("CountryCode") + "\\";

                var quoteDetailsResponse = _quoteManager.GetQuoteDetails(quoteId.ToString());
                if (quoteDetailsResponse.HasError)
                {
                    response.HasError = true;
                    response.ErrorMessage = quoteDetailsResponse.ErrorMessage;
                    return response;
                }


                var quoteDetails = quoteDetailsResponse.QuoteDetails;
                FileInfo fileInfo;
                if (quoteDetails.QuoteHeader.FinanceTypeName == "Instalment")
                {
                    fileInfo = new FileInfo(templatesFolderPath + "InstalmentProposal.xlsx");
                }
                else
                {
                    fileInfo = new FileInfo(templatesFolderPath + "LeaseOrRentalProposal.xlsx");
                }

                using (ExcelPackage excel = new ExcelPackage(fileInfo))
                {
                    var workbook = excel.Workbook;


                    ExcelWorksheet opexProposalSheet = excel.Workbook.Worksheets.FirstOrDefault();

                    // Label
                    var label = "Your " + quoteDetails.QuoteHeader.Frequency + " Payment will be approximately:";
                    opexProposalSheet.EpplusSetCellValue(41, 5, label, "LabelStyle");
                    var labelStyle = opexProposalSheet.Cells[41, 5].Style;
                    labelStyle.Font.Size = 11;
                    labelStyle.Font.Bold = true;
                    labelStyle.WrapText = true;
                    labelStyle.Fill.PatternType = ExcelFillStyle.Solid;
                    labelStyle.Fill.BackgroundColor.SetColor(0, ByteColor.IMFSBlue[0], ByteColor.IMFSBlue[1], ByteColor.IMFSBlue[2]);
                    labelStyle.Font.Color.SetColor(0, ByteColor.White[0], ByteColor.White[1], ByteColor.White[2]);

                    
                    // Duration
                    opexProposalSheet.EpplusSetCellValue(46, 6, quoteDetails.QuoteHeader.QuoteDuration, "DurationStyle");
                    var durationStyle = opexProposalSheet.Cells[46, 6].Style;
                    durationStyle.Font.Size = 14;
                    durationStyle.Font.Bold = true;
                    durationStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    durationStyle.VerticalAlignment = ExcelVerticalAlignment.Center;
                    durationStyle.Fill.PatternType = ExcelFillStyle.Solid;
                    durationStyle.Fill.BackgroundColor.SetColor(0, ByteColor.IMFSBlue[0], ByteColor.IMFSBlue[1], ByteColor.IMFSBlue[2]);
                    durationStyle.Font.Color.SetColor(0, ByteColor.White[0], ByteColor.White[1], ByteColor.White[2]);
                    // Finance Value
                    if (quoteDetails.QuoteHeader.FinanceValue.HasValue)
                    {
                        opexProposalSheet.EpplusSetCellValue(43, 6, quoteDetails.QuoteHeader.FinanceValue.Value.ToString("C"), "DurationStyle");
                        var financeValueStyle = opexProposalSheet.Cells[43, 6].Style;
                        financeValueStyle.Font.Size = 14;
                        financeValueStyle.Font.Bold = true;
                        financeValueStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        financeValueStyle.VerticalAlignment = ExcelVerticalAlignment.Center;
                        financeValueStyle.Fill.PatternType = ExcelFillStyle.Solid;
                        financeValueStyle.Fill.BackgroundColor.SetColor(0, ByteColor.IMFSBlue[0], ByteColor.IMFSBlue[1], ByteColor.IMFSBlue[2]);
                        financeValueStyle.Font.Color.SetColor(0, ByteColor.White[0], ByteColor.White[1], ByteColor.White[2]);
                    }
                    workbook.Worksheets.FirstOrDefault().Select();
                    string quoteName = Regex.Replace(quoteDetails.QuoteHeader.QuoteName, "[^a-zA-Z0-9 -]", "");
                    response.FileName = "IMFS-" + quoteDetails.Id + "-" + quoteDetails.QuoteHeader.FinanceTypeName + "-Proposal.xlsx";
                    if (response.FileName.Length > 200)
                    {
                        response.FileName = response.FileName.Substring(0, 200);
                    }
                    response.DownloadFile = excel.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.ToString();
            }
            return response;
        }
    }
}
