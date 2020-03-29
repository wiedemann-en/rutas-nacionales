using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Utils.Infrastructure;

namespace Vialidad.Cache.Infrastructure
{
    public class CacheCommonSettings : CommonSettings
    {
        public static bool CacheActive
        {
            get { return bool.Parse(GetSettingDefault("CACHE:Active", "False")); }
        }
        public static double CacheMinutes
        {
            get { return double.Parse(GetSettingDefault("CACHE:Minutes", "0")); }
        }
    }
}
