using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Enums;
using Vialidad.Contracts.Models;
using Vialidad.Contracts.Services;
using Vialidad.Download;
using Vialidad.Logger.Interfaces;
using Vialidad.Logger.Logic;
using Vialidad.Models;
using Vialidad.Services;
using Vialidad.Services.Normalizer;
using Vialidad.Utils;
using Vialidad.Utils.Extensions;

namespace Vialidad.Compat
{
    public class ImportFromExcel
    {
        #region Private Attributes
        private readonly DownloadBase _downloader;
        private readonly ILogger _logger;
        #endregion

        #region Constructors
        public ImportFromExcel()
        {
            _downloader = new DownloadDb(enDataProviderType.NoAplica);
            _logger = LoggerFactory.GetInstance();
        }
        #endregion

        #region Public Methods
        public void ImportToDb()
        {
            var items = new List<TramoImport>();

            try
            {
                string[] fileEntries = Directory.GetFiles(@"C:\Tempo\Files");
                foreach (var item in fileEntries)
                {
                    HSSFWorkbook hssfwb;
                    using (FileStream file = new FileStream(@"C:\Tempo\Files\" + item, FileMode.Open, FileAccess.Read))
                    {
                        hssfwb = new HSSFWorkbook(file);
                    }

                    ISheet sheet = hssfwb.GetSheet("rutas");
                    for (int row = 0; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) == null) continue;
                        if (sheet.GetRow(row).GetCell(0) == null) continue;
                        if (sheet.GetRow(row).GetCell(0).StringCellValue == "Provincia") continue;
                        if (sheet.GetRow(row).GetCell(0).StringCellValue == "") continue;

                        TramoImport itemToAdd = new TramoImport();
                        itemToAdd.Provincia = sheet.GetRow(row).GetCell(0).StringCellValue;
                        itemToAdd.Ruta = sheet.GetRow(row).GetCell(1).StringCellValue;
                        itemToAdd.TramoNormalizado = sheet.GetRow(row).GetCell(2).StringCellValue;
                        itemToAdd.TramoDesnormalizado = sheet.GetRow(row).GetCell(2).StringCellValue;
                        itemToAdd.Calzada = sheet.GetRow(row).GetCell(3).StringCellValue;
                        itemToAdd.Detalle = sheet.GetRow(row).GetCell(4).StringCellValue;
                        if (sheet.GetRow(row).GetCell(5) != null)
                            itemToAdd.Observaciones = sheet.GetRow(row).GetCell(5).StringCellValue;
                        if (sheet.GetRow(row).GetCell(6) != null)
                            itemToAdd.Actualizacion = sheet.GetRow(row).GetCell(6).StringCellValue.StrToDateTime();
                        if (sheet.GetRow(row).GetCell(7) != null)
                            itemToAdd.Coordenadas = sheet.GetRow(row).GetCell(7).StringCellValue;

                        //Normalizamos la información del tramos
                        itemToAdd.TramoNormalizado = NormalizerTramo.Normalize(itemToAdd.TramoNormalizado);

                        var tramoExiste = items.FirstOrDefault(x =>
                            x.Provincia.ToUpper() == itemToAdd.Provincia.ToUpper() &&
                            x.Ruta.ToUpper() == itemToAdd.Ruta.ToLower() &&
                            x.TramoNormalizado.ToUpper() == itemToAdd.TramoNormalizado.ToUpper());

                        if (tramoExiste == null)
                        {
                            items.Add(itemToAdd);
                        }
                        else
                        {
                            tramoExiste.Calzada = itemToAdd.Calzada;
                            tramoExiste.Detalle = itemToAdd.Detalle;
                            tramoExiste.Observaciones = itemToAdd.Observaciones;
                            tramoExiste.Actualizacion = itemToAdd.Actualizacion;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ImportFromExcel.ImportToDb", ex.Message, ex);
            }

            //Grabamos en el repositorio que deseamos
            _downloader.SaveData(items);
        }
        #endregion
    }
}
