using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Vialidad.Utils.Extensions
{
    public static class StringExtensionMethods
    {
        public static string ToTitleCase(this string value)
        {
            var trimmer = new Regex(@"\s\s+");
            value = trimmer.Replace(value, " ");
            value = value.Trim();
            value = value.ToLower();

            if (string.IsNullOrEmpty(value))
                return value;

            var wordsOmite = new List<string>() { "el", "la", "los", "las", "en" };

            var words = value.Split(' ');
            for (int iPos = 0; iPos < words.Length; iPos++)
            {
                if ((iPos > 0) && (!words[iPos - 1].EndsWith(".")) && (wordsOmite.Contains(words[iPos])))
                    continue;

                char[] wordArray = words[iPos].ToCharArray();
                wordArray[0] = char.ToUpper(wordArray[0]);
                words[iPos] = wordArray[0] + words[iPos].Substring(1);

            }

            var result = string.Join(" ", words);
            return result;
        }
    }
}
