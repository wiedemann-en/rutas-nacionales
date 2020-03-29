using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Utils.Extensions
{
    public static class FilePathExtensionMethods
    {
        public static string GetResumePath(this string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return string.Empty;

            var paths = filePath.Split('\\');
            return paths.SkipWhile(f => !f.Contains("Vialidad")).Aggregate
            (
                new StringBuilder(),
                (acc, itm) => acc.Append(itm).Append('\\'),
                f => f.Remove(f.Length - 1, 1).ToString()
            );
        }
    }
}
