using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Utils.Infrastructure
{
    public abstract class CommonSettings
    {
        #region Helpers
        protected static string GetSettingDefault(string codSetting, string defaultValue)
        {
            var retorno = ConfigurationManager.AppSettings[codSetting];
            if (string.IsNullOrWhiteSpace(retorno))
                return defaultValue;
            return retorno;
        }
        #endregion
    }
}
