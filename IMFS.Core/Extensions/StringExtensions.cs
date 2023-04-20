using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IMFS.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNull(this string str)
        {
            return str == null;
        }

        public static int ToInt(this string str, int defaultValue = 0)
        {
            int integer = 0;
            Int32.TryParse(str, out integer);
            if (integer == 0 && str != "0")
                integer = defaultValue;
            return integer;
        }

        public static int? ToIntNullable(this string str)
        {
            int integer = 0;
            Int32.TryParse(str, out integer);
            if (integer == 0 && str != "0")
                return null;
            return integer;
        }

        public static decimal ToDecimal(this string str, int defaultValue = 0)
        {
            decimal dec = 0;
            if (!Decimal.TryParse(str, out dec))
                dec = defaultValue;
            return dec;
        }

        public static decimal? ToDecimalNullable(this string str, NumberStyles numberStyle = NumberStyles.Any)
        {
            decimal dec = 0;
            try
            {
                dec = decimal.Parse(str, numberStyle);
                return dec;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static double ToDouble(this string str, int defaultValue = 0)
        {
            double dou = 0;
            if (!Double.TryParse(str, out dou))
                dou = defaultValue;
            return dou;
        }

        public static double? ToDoubleNullable(this string str, NumberStyles numberStyle = NumberStyles.Any)
        {
            double dou = 0;
            try
            {
                dou = double.Parse(str, numberStyle);
                return dou;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string ToBlankReplacedString(this string text, string blankText = "blank")
        {
            return string.IsNullOrEmpty(text) ? blankText : text;
        }

        public static DateTime? ToDateTimeNullable(this string str, string format)
        {
            return str.ToDateTimeNullable(new string[] { format });
        }

        // usage string.ToDateTimeNullable(new string[] { "d/MM/yyyy", "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy" });
        public static DateTime? ToDateTimeNullable(this string str, string[] format)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None);
                return dt;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        public static bool? ToBoolNullable(this string str, string trueValue = "")
        {
            if (string.IsNullOrEmpty(str)) return null;

            if (str.ToUpper() == trueValue.ToUpper()) return true;
            return false;
        }

        public static int GetNthIndex(this string s, char t, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == t)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static string Trim(this string s, string defaultReturn = "")
        {
            if (string.IsNullOrEmpty(s))
                return defaultReturn;
            else
                return s.Trim();
        }

        public static string ToTitleCase(this string s)
        {

            if (string.IsNullOrEmpty(s)) return string.Empty;
            TextInfo textInfo = new CultureInfo("en-AU", false).TextInfo;

            // to title case is not working if incoming string is all upper case.
            // To prevent this, convert to lowercase first and then convert to title case
            return textInfo.ToTitleCase(s.ToLower());
        }

        public static string EmptyNull(this string s)
        {
            return s ?? "";
        }

        public static string ToUpperIgnoreNull(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return s.ToUpper();
        }

        public static string ToLowerIgnoreNull(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return s.ToLower();
        }

        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        public static string Between(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        /// <summary>
        /// Get string value after [first] a.
        /// </summary>
        public static string Before(this string value, string a)
        {
            int posA = value.IndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] a.
        /// </summary>
        public static string After(this string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }

    }
}
