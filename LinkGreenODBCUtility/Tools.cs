using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinkGreenODBCUtility
{
    public class Tools
    {
        public static string CleanString(string dirtyString)
        {
            HashSet<char> removeChars = new HashSet<char>("?&^$#!()+,:;<>’*");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
                if (!removeChars.Contains(c)) // prevent dirty chars
                    result.Append(c);

            // Remove unwanted ascii chars
            string resultString = Regex.Replace(result.ToString(), @"[^\u0020-\u007F]+", string.Empty);
            return resultString;
        }

        public static string CleanStringOfNonDigits(string s)
        {
            string cleaned = Regex.Replace(s, @"[^\d]", "");
            return cleaned;
        }
    }
}
