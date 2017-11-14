using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinkGreenODBCUtility
{
    public class Tools
    {
        public static string CleanStringForSql(string dirtyString)
        {
            HashSet<char> removeChars = new HashSet<char>("|*<>,=~^();`");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
                if (!removeChars.Contains(c)) // prevent dirty chars
                    result.Append(c);

            // Remove unwanted ascii chars
            return Regex.Replace(result.ToString(), @"[^\u0020-\u007F]+", string.Empty);
        }

        public static string CleanStringOfNonDigits(string s)
        {
            return Regex.Replace(s, @"[^\d]", "");
        }

        public static string CleanAlphanumeric(string s)
        {
            return Regex.Replace(s, @"^[a-zA-Z][a-zA-Z0-9]*$", string.Empty);
        }

        public static string CleanEmail(string s)
        {
            MatchCollection matchList = Regex.Matches(s, @"(?i)\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b(?-i)");
            var list = matchList.Cast<Match>().Select(match => match.Value).ToList();
            if (list.Any())
            {
                return list[0];
            }

            Logger.Instance.Warning($"No valid email address found in \"{s}\"");
            return null;
        }

        public static string FormatDecimal(string value)
        {
            try
            {
                return Decimal.Parse(value, NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, new CultureInfo("en-US")).ToString();
            }
            catch (Exception e)
            {
                Logger.Instance.Warning($"Failed to parse invalid currency \"{value}\"");
                return null;
            }
        }

        public static string CleanUniqueId(string s)
        {
            return Regex.Replace(s, @"[^a-zA-Z0-9_-]+", string.Empty);
        }
    }
}
