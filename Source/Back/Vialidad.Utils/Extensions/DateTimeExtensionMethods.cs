using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Utils.Extensions
{
    public static class DateTimeExtensionMethods
    {
        public static DateTime StrToDateTime(this string value)
        {
            if (!DateTime.TryParseExact(value, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var result))
            {
                if (!DateTime.TryParseExact(value, "dd/MM/yy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
                    result = DateTime.MinValue;
            }
            return result;
        }
    }
}
