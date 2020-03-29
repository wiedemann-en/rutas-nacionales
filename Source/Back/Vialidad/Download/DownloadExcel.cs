using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Enums;
using Vialidad.Contracts.Models;
using Vialidad.Models;
using Vialidad.Utils;
using Vialidad.Utils.Export;

namespace Vialidad.Download
{
    public class DownloadExcel : DownloadBase
    {
        #region Constructors
        public DownloadExcel(enDataProviderType dataProviderType)
            : base(dataProviderType) { }
        #endregion

        #region Overrides
        public override void SaveData(List<TramoImport> info)
        {
            try
            {
                ExcelExport<TramoImport> excelResult = new ExcelExport<TramoImport>(info, null, "rutas");
                excelResult.ExecuteResult();

                _logger.Info("DownloadExcel.SaveData", $"{info.Count} registros exportados.");
            }
            catch (Exception ex)
            {
                _logger.Error("DownloadExcel.SaveData", ex.Message, ex);
            }
        }
        #endregion
    }
}
