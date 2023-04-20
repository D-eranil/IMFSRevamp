using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IMFS.Core
{
    public static class Tools
    {
        public static void SetProperties(object source, object target)
        {
            var targetType = target.GetType();
            foreach (var prop in source.GetType().GetProperties())
            {
                try
                {
                    var propGetter = prop.GetGetMethod();
                    var targetProperty = targetType.GetProperty(prop.Name);
                    if (targetProperty == null) continue;
                    var propSetter = targetProperty.GetSetMethod();
                    var valueToSet = propGetter.Invoke(source, null);
                    propSetter.Invoke(target, new[] { valueToSet });
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        public static string GetErrorMessage(Exception ex)
        {
            if (ex == null) return string.Empty;

            string errorMessage = ex.ToString();
            if (ex.InnerException != null)
            {
                errorMessage = ex.InnerException.ToString();
            }
            return errorMessage;
        }

        public static string GetTimeStamp()
        {
            return DateTime.Now.ToString("dd/MM/yyyy hh:mm:sstt");
        }

        public static List<string> GetAllFilesAndFolders(string fullPath)
        {
            var fileList = new List<string>();
            var allFolders = Directory.GetDirectories(fullPath, "*.*", SearchOption.AllDirectories);
            fileList.AddRange(allFolders.ToList());
            var allFiles = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories);
            fileList.AddRange(allFiles.ToList());
            return fileList;
        }

        public static bool IsFolderExist(string fullPath)
        {
            return Directory.Exists(fullPath);
        }

        public static bool IsFileExist(string fullPath)
        {
            return File.Exists(@fullPath);
        }

        public static void CreateFolder(string fullFolderPath, string folderName)
        {
            var directoryPath = Path.Combine(fullFolderPath, folderName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
        public static void CreateFolder(string fullFolderPath)
        {
            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
            }
        }

        public static void RenameFolder(string oldFolderPath, string newFolderPath)
        {
            if (Directory.Exists(oldFolderPath))
            {
                Directory.Move(oldFolderPath, newFolderPath);
            }
        }

        public static void CopyFile(string sourceFile, string newFile)
        {
            if (File.Exists(sourceFile))
            {
                File.Copy(sourceFile, newFile);
            }
        }

        public static void DeleteFolder(string fullFolderPath)
        {
            if (Directory.Exists(fullFolderPath))
            {
                Directory.Delete(fullFolderPath);
            }
        }

        public static void DeleteFile(string fullFilePath)
        {
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }
        }

        public static string AppendCSV(string text)
        {
            string delimeter = ",";
            if (string.IsNullOrEmpty(text))
            {
                return String.Format("\"\"{0}", delimeter);
            }
            if (text.Contains("\""))
            {
                text = text.Replace("\"", "\"\"");
            }

            if (text.Contains(System.Environment.NewLine))
            {
                text = text.Replace("\r\n", " ");
            }

            text = String.Format("\"{0}\"{1}", text, delimeter);
            return text;
        }

        public static bool WriteFile(string fullFilePath, string fileContent)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fullFilePath);
                using (StreamWriter fileWriter = fileInfo.CreateText())
                {
                    fileWriter.WriteLine(fileContent);
                    fileWriter.Flush();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ReadFile(string fullFilePath)
        {
            try
            {
                return File.ReadAllText(fullFilePath);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        public static int GetWeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll
                // be the same week# as whatever Thursday, Friday or Saturday are,
                // and we always get those right
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime FirstDayOfWeek(this DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
                diff += 7;
            return dt.AddDays(-diff).Date;
        }

        public static DateTime LastDayOfWeek(this DateTime dt)
        {
            return dt.FirstDayOfWeek().AddDays(6);
        }

        public static string GetPlainTextFromHtml(string htmlString, int maxLength = 8000, bool removeBlankLine = false)
        {

            if (string.IsNullOrEmpty(htmlString))
            {
                return string.Empty;
            }
            string htmlTagPattern = "<.*?>";
            var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            htmlString = htmlString.Replace("&nbsp;", string.Empty);

            if (htmlString.Length > maxLength)
            {
                htmlString = htmlString.Substring(0, maxLength);
            }
            if (removeBlankLine)
            {
                htmlString = Regex.Replace(htmlString, @"\s+", " ");
            }
            return htmlString;
        }

        // this function will generate letter like excel column A-Z, AA-ZZ
        public static IEnumerable<string> GenerateLetters()
        {
            for (char c = 'A'; c <= 'Z'; c++)
                yield return new string(c, 1);
            for (char c = 'A'; c <= 'Z'; c++)
                for (char d = 'A'; d <= 'Z'; d++)
                    yield return new string(new[] { c, d });
        }
    }
}
