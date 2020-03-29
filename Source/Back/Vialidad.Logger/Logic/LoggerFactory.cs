using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Logger.Infrastructure;
using Vialidad.Logger.Interfaces;

namespace Vialidad.Logger.Logic
{
    public static class LoggerFactory
    {
        #region Public Methods
        public static ILogger GetInstance()
        {
            ILogger result = null;
            switch (LoggerCommonSettings.LogMode.ToUpper())
            {
                case "DB":
                    result = new LoggerDb();
                    break;
                case "FILE":
                    result = new LoggerFile();
                    break;
                default:
                    break;
            }
            return result;
        }
        #endregion
    }
}
