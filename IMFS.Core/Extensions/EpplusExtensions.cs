using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;
using System;
using System.Linq;

namespace ORP.Core.Extensions
{
    public static class EpplusExtensions
    {
        public class EpplusStyle
        {
            public const string NormalLeftAlign = "NormalLeftAlign";
            public const string NormalCenterAlign = "NormalCenterAlign";
            public const string NormalRightAlign = "NormalRightAlign";
            public const string TotalsStyle = "TotalsStyle";
        }

        public static class ByteColor
        {
            public static byte[] White = new byte[3] { 255, 255, 255 };
            public static byte[] Grey20Percent = new byte[3] { 208, 206, 206 };
            public static byte[] LightGrey = new byte[3] { 192, 192, 192 };
            public static byte[] GainsboroGrey = new byte[3] { 230, 230, 230 };
            public static byte[] BlueText = new byte[3] { 0, 112, 192 };
            public static byte[] RedText = new byte[3] { 255, 0, 0 };
            public static byte[] RedBackground = new byte[3] { 241, 168, 153 };
            public static byte[] GreenBackground = new byte[3] { 226, 239, 218 };
            public static byte[] LightYellow = new byte[3] { 255, 255, 153 };
            public static byte[] LightSteelBlue = new byte[3] { 176, 196, 222 };
            public static byte[] LightBlue = new byte[3] { 0, 176, 240 };
            public static byte[] IMFSBlue = new byte[3] { 0, 129, 192 };

        }

        public static void InsertNewRow(ExcelWorksheet worksheet, int fromRow, int destinationRow, bool copyStyle = true)
        {
            int defaultColumnStart = 2;
            if (fromRow == destinationRow) return;

            try
            {
                if (copyStyle)
                {
                    //It clears style of FromRow, need to saved it
                    var style = worksheet.Cells[fromRow, defaultColumnStart, fromRow, worksheet.Dimension.End.Column].StyleID;
                    worksheet.InsertRow(destinationRow, 1, fromRow);
                    //copy merge style todo
                    worksheet.Cells[fromRow, 1, fromRow, worksheet.Dimension.End.Column].Copy(worksheet.Cells[destinationRow, 1, destinationRow, worksheet.Dimension.End.Column]);
                    worksheet.Cells[fromRow, defaultColumnStart, fromRow, worksheet.Dimension.End.Column].StyleID = style;
                }
                else
                {
                    worksheet.InsertRow(destinationRow, 1);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.ToString();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="worksheet">Excel worksheet</param>
        /// <param name="styleName">Defines style name</param>
        /// <param name="isBold">Sets Bold font </param>
        /// <param name="isItalic">Sets Italic font</param>
        /// <param name="alignmentHorizontal">Horizontal Alignment: General = 0, Left = 1, Center = 2, CenterContinuous = 3, Right = 4, Fill = 5, Distributed = 6, Justify = 7</param>
        /// <param name="alignmentVertical">Vertical Alignment: Top = 0, Center = 1, Bottom = 2, Distributed = 3, Justify = 4</param>
        /// <param name="wrapText">Sets Cell Wrap Text</param>
        /// <param name="fontColor">Sets Font Color</param>
        /// <param name="fontSize">Sets Font Size</param>
        /// <param name="backgroundColor">Sets Cell Background Color</param>
        /// <param name="dataFormat"></param>
        /// <returns></returns>
        public static ExcelNamedStyleXml CreateStyle(ExcelWorksheet worksheet,
                string styleName,
                bool isBold = false,
                bool isItalic = false,
                OfficeOpenXml.Style.ExcelHorizontalAlignment? alignmentHorizontal = null,
                OfficeOpenXml.Style.ExcelVerticalAlignment? alignmentVertical = null,
                bool? wrapText = null,
                byte[] fontColor = null,
                float? fontSize = null,
                byte[] backgroundColor = null,
                short? dataFormat = null
                )
        {
            //Check if Style already exists
            ExcelStyleCollection<ExcelNamedStyleXml> nameStyles = worksheet.Workbook.Styles.NamedStyles;

            var existingStyle = nameStyles.Where(x => x.Name == styleName).FirstOrDefault();
            if (existingStyle != null)
            {
                return existingStyle;
            }

            var namedStyle = worksheet.Workbook.Styles.CreateNamedStyle(styleName);

            //When Creating defaults to Arial
            namedStyle.Style.Font.Name = "Calibri";

            namedStyle.Style.Font.Bold = isBold;
            namedStyle.Style.Font.Italic = isItalic;

            if (alignmentHorizontal != null && alignmentHorizontal.HasValue)
            {
                namedStyle.Style.HorizontalAlignment = alignmentHorizontal.Value;
            }

            if (alignmentVertical != null && alignmentVertical.HasValue)
            {
                namedStyle.Style.VerticalAlignment = alignmentVertical.Value;
            }

            if (wrapText != null && wrapText.HasValue)
            {
                namedStyle.Style.WrapText = wrapText.Value;
            }

            if (fontColor != null && fontColor.Length == 3)
            {
                namedStyle.Style.Font.Color.SetColor(0, fontColor[0], fontColor[1], fontColor[2]);
            }

            if (fontSize != null && fontSize.HasValue)
            {
                namedStyle.Style.Font.Size = fontSize.Value;
            }

            if (backgroundColor != null && backgroundColor.Length == 3)
            {
                namedStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                namedStyle.Style.Fill.BackgroundColor.SetColor(0, backgroundColor[0], backgroundColor[1], backgroundColor[2]);
            }

            return namedStyle;
        }

        public static void EpplusSetCellValue(this ExcelWorksheet sheet, int rowNumber, int columnNumber, object value, string styleName = null, string numberformat = null)
        {
            try
            {
                sheet.Cells[rowNumber, columnNumber].Value = value;
                if (!string.IsNullOrEmpty(styleName))
                    sheet.Cells[rowNumber, columnNumber].StyleName = styleName;

                if (!string.IsNullOrEmpty(numberformat))
                    sheet.Cells[rowNumber, columnNumber].Style.Numberformat.Format = numberformat;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.ToString();
            }
        }

        public static void SetRowBgColor(ExcelWorksheet sheet, int rowNumber, byte[] backgroundColor)
        {
            sheet.Row(rowNumber).Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Row(rowNumber).Style.Fill.BackgroundColor.SetColor(0, backgroundColor[0], backgroundColor[1], backgroundColor[2]);
        }

        public static ExcelNamedStyleXml GetStyle(ExcelWorksheet worksheet, string styleName)
        {
            ExcelStyleCollection<ExcelNamedStyleXml> namedStyles = worksheet.Workbook.Styles.NamedStyles;
            var existingStyle = namedStyles.Where(x => x.Name == styleName).FirstOrDefault();
            if (existingStyle != null)
            {
                return existingStyle;
            }

            var namedStyle = worksheet.Workbook.Styles.CreateNamedStyle(styleName);
            namedStyle.Style.Font.Name = "Calibri";
            namedStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
            switch (styleName)
            {
                case EpplusStyle.NormalLeftAlign:
                    {
                        namedStyle.Style.Font.Bold = false;
                        namedStyle.Style.Font.Italic = false;
                        namedStyle.Style.Font.Size = 10;
                        namedStyle.Style.WrapText = true;
                        namedStyle.Style.Border.Top.Style = ExcelBorderStyle.None;
                        namedStyle.Style.Border.Bottom.Style = ExcelBorderStyle.None;
                        namedStyle.Style.Border.Left.Style = ExcelBorderStyle.None;
                        namedStyle.Style.Border.Right.Style = ExcelBorderStyle.None;
                        namedStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        namedStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        namedStyle.Style.Fill.BackgroundColor.SetColor(0, ByteColor.White[0], ByteColor.White[1], ByteColor.White[2]);
                        namedStyle.Style.WrapText = true;
                    }
                    break;

                case EpplusStyle.NormalCenterAlign:
                    {
                        namedStyle.Style.Font.Bold = false;
                        namedStyle.Style.Font.Italic = false;
                        namedStyle.Style.Font.Size = 10;
                        namedStyle.Style.WrapText = true;
                        namedStyle.Style.Border.Top.Style = ExcelBorderStyle.None;
                        namedStyle.Style.Border.Bottom.Style = ExcelBorderStyle.None;
                        namedStyle.Style.Border.Left.Style = ExcelBorderStyle.None;
                        namedStyle.Style.Border.Right.Style = ExcelBorderStyle.None;
                        namedStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        namedStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        namedStyle.Style.Fill.BackgroundColor.SetColor(0, ByteColor.White[0], ByteColor.White[1], ByteColor.White[2]);
                        namedStyle.Style.WrapText = true;
                    }
                    break;

                case EpplusStyle.NormalRightAlign:
                    {
                        namedStyle.Style.Font.Bold = false;
                        namedStyle.Style.Font.Italic = false;
                        namedStyle.Style.Font.Size = 10;
                        namedStyle.Style.WrapText = true;
                        namedStyle.Style.Border.Top.Style = ExcelBorderStyle.None;
                        namedStyle.Style.Border.Bottom.Style = ExcelBorderStyle.None;
                        namedStyle.Style.Border.Left.Style = ExcelBorderStyle.None;
                        namedStyle.Style.Border.Right.Style = ExcelBorderStyle.None;
                        namedStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        namedStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        namedStyle.Style.Fill.BackgroundColor.SetColor(0, ByteColor.White[0], ByteColor.White[1], ByteColor.White[2]);
                        namedStyle.Style.WrapText = true;
                    }
                    break;

                case EpplusStyle.TotalsStyle: //InsertTotals
                    {
                        namedStyle.Style.Font.Bold = true;
                        namedStyle.Style.Font.Italic = false;
                        namedStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        namedStyle.Style.Fill.BackgroundColor.SetColor(0, ByteColor.White[0], ByteColor.White[1], ByteColor.White[2]);
                        namedStyle.Style.Font.Color.SetColor(0, ByteColor.BlueText[0], ByteColor.BlueText[1], ByteColor.BlueText[2]);
                        namedStyle.Style.Font.Size = 10;
                    }
                    break;
                default:
                    return null;
            }

            return namedStyle;
        }
    }
}