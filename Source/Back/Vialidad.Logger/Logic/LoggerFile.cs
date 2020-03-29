using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Logger.Interfaces;

namespace Vialidad.Logger.Logic
{
    public class LoggerFile : ILogger
    {
        #region ILogger
        public void Error(string origen, string message = "", Exception ex = null, [CallerMemberName] string sourceMember = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "", bool global = false, bool generateMessage = true)
        {
            throw new NotImplementedException();

            //var path = SxDataCore.Parametros.SxConfigurations.SxAppSetting["PathLogCodigos"];
            //var key = Guid.NewGuid();
            ////var key = DateTime.Now.Date.ToString("ddMMyyyy.hhmmss");

            //string fileName = Path.Combine(path, string.Format("{0}_{1}_{2}.xml", key, conexionAseguradora.IdAseguradora, p.CodigoCia));
            //p.SerializeAndSave(fileName);

            //var sb = new StringBuilder();
            //sb.AppendLine(ex.Message);
            //sb.AppendLine(ex.StackTrace);
            //sb.AppendLine(ex.Source);
            //if (ex.InnerException != null)
            //{
            //    sb.AppendLine(ex.InnerException.Message);
            //    sb.AppendLine(ex.InnerException.StackTrace);
            //    sb.AppendLine(ex.InnerException.Source);
            //}

            //File.WriteAllText(path + "err-normalizador-" + key + ".txt", sb.ToString());
        }

        public void Info(string origen, string message, [CallerMemberName] string sourceMember = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            throw new NotImplementedException();
        }

        public void Warning(string origen, string message, [CallerMemberName] string sourceMember = "", [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
