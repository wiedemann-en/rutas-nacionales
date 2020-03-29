using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Logger.Interfaces;
using Vialidad.Model;
using Vialidad.Model.DbModel;
using Vialidad.Utils.Extensions;

namespace Vialidad.Logger.Logic
{
    public class LoggerDb : ILogger
    {
        private static readonly string _SourceFormat = "Source: {0}/LineNumber: {1}/File: {2}";
        private static readonly string _UtcOffSet = TimeZoneInfo.Local.BaseUtcOffset.ToString();

        #region ILogger
        public void Info(string origen, string message, [CallerMemberName]string sourceMember = "", [CallerLineNumber]int lineNumber = 0, [CallerFilePath]string filePath = "")
        {
            var log = new LogEntity();
            log.TipoLog = "I";
            log.Origen = string.Format(_SourceFormat, sourceMember, lineNumber, filePath.GetResumePath());
            log.Fecha = DateTime.UtcNow;
            log.UtcOffSet = _UtcOffSet;
            log.Descripcion = message;

            using (var dbContext = new VialidadContext())
            {
                dbContext.LogDataSet.Add(log);
                dbContext.SaveChanges();
            }
        }

        public void Warning(string origen, string message, [CallerMemberName]string sourceMember = "", [CallerLineNumber]int lineNumber = 0, [CallerFilePath]string filePath = "")
        {
            var log = new LogEntity();
            log.TipoLog = "W";
            log.Origen = string.Format(_SourceFormat, sourceMember, lineNumber, filePath.GetResumePath());
            log.Fecha = DateTime.UtcNow;
            log.UtcOffSet = _UtcOffSet;
            log.Descripcion = message;

            using (var dbContext = new VialidadContext())
            {
                dbContext.LogDataSet.Add(log);
                dbContext.SaveChanges();
            }
        }

        public void Error(string origen, string message = "", Exception ex = null, [CallerMemberName]string sourceMember = "", [CallerLineNumber]int lineNumber = 0, [CallerFilePath]string filePath = "", bool global = false, bool generateMessage = true)
        {
            var log = new LogEntity();
            log.TipoLog = "E";
            log.Fecha = DateTime.UtcNow;
            log.UtcOffSet = _UtcOffSet;

            if (ex != null)
            {
                log.Origen = global ? origen : string.Format(_SourceFormat, sourceMember, lineNumber, filePath.GetResumePath());
                log.Descripcion = (!generateMessage && string.IsNullOrWhiteSpace(ex.Message)) ? message : ex.GetDescription();
                log.Detalle = ex.GetDetail();
                log.StackTrace = ex.GetStackTrace();
                log.Source = ex.Source;
                log.TargetSite = ex.TargetSite != null ? ex.TargetSite.Name : string.Empty;
            }
            else
            {
                log.Origen = string.Format(_SourceFormat, sourceMember, lineNumber, filePath);
                log.Descripcion = message;
            }

            using (var dbContext = new VialidadContext())
            {
                dbContext.LogDataSet.Add(log);
                dbContext.SaveChanges();
            }
        }
        #endregion
    }
}
