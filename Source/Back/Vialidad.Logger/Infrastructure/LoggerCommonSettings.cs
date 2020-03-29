using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Utils.Infrastructure;

namespace Vialidad.Logger.Infrastructure
{
    public class LoggerCommonSettings : CommonSettings
    {
        public static bool LogActive
        {
            get { return bool.Parse(GetSettingDefault("LOG:Active", "False")); }
        }
        public static string LogMode
        {
            get { return GetSettingDefault("LOG:Mode", "File"); }
        }
        public static string LogPath
        {
            get { return GetSettingDefault("LOG:Path", ""); }
        }
        public static string LogFileName
        {
            get { return GetSettingDefault("LOG:FileName", ""); }
        }
    }
}
