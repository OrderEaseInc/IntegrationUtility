using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text.RegularExpressions;

using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common.Model;

// ReSharper disable once CheckNamespace
namespace LinkGreenODBCUtility
{
    public class Tools
    {
        private static IList<CountryMapping> _allCountries;
        private static IList<ProvinceMapping> _allProvinces;

        public static string CleanStringForSql(string dirtyString)
        {
            // HashSet<char> removeChars = new HashSet<char>("|*<>,=~^();`");
            var removeChars = new HashSet<char>("*<>,=~^();`");
            var result = new StringBuilder(dirtyString.Length);
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var c in dirtyString)
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
                return decimal
                    .Parse(value,
                        NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
                        new CultureInfo("en-US")).ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                Logger.Instance.Warning($"Failed to parse invalid currency \"{value}\"");
                return null;
            }
        }

        public static string CleanUniqueId(string s)
        {
            return Regex.Replace(s, @"[^\/a-zA-Z0-9_-]+", string.Empty);
        }

        public static string CleanCountry(string s, string connectionString, OdbcConnection connection)
        {
            if (_allCountries == null)
                _allCountries = new CountryMappingRepository(connectionString, connection).GetAll().ToArray();
            if (s.Equals("CANADA", StringComparison.InvariantCultureIgnoreCase)) s = "Canada";
            // Country is already a "destination" country
            if (_allCountries.Any(c => c.Destination.Equals(s, StringComparison.InvariantCultureIgnoreCase))) return s;
            // Get map country, otherwise it's "Other"
            var cleanCountry = _allCountries.FirstOrDefault(c => c.Source.Equals(s, StringComparison.InvariantCultureIgnoreCase))?.Destination ?? "Other";
            return cleanCountry;
        }

        public static string CleanProvince(string s, string connectionString, OdbcConnection connection)
        {
            if (_allProvinces == null)
                _allProvinces = new ProvinceMappingRepository(connectionString, connection).GetAll().ToArray();
            // Country is already a "destination" country
            if (_allProvinces.Any(c => c.Destination.Equals(s, StringComparison.InvariantCultureIgnoreCase))) return s;
            // Get map country, otherwise it's "Other"
            return _allProvinces.FirstOrDefault(c => c.Source.Equals(s, StringComparison.InvariantCultureIgnoreCase))?.Destination ?? "Other";
        }
    }
}
