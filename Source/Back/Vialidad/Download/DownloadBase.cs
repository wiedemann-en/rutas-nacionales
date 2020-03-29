using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vialidad.Contracts.Enums;
using Vialidad.Contracts.Models;
using Vialidad.Download.Providers;
using Vialidad.Logger.Interfaces;
using Vialidad.Logger.Logic;
using Vialidad.Models;
using Vialidad.Services.Normalizer;

namespace Vialidad.Download
{
    public abstract class DownloadBase
    {
        #region Protected Attributes
        protected readonly ILogger _logger;
        private readonly enDataProviderType _dataProviderType;
        #endregion

        #region Constructors
        public DownloadBase(enDataProviderType dataProviderType)
        {
            _dataProviderType = dataProviderType;
            _logger = LoggerFactory.GetInstance();
        }
        #endregion

        #region Public Methods
        public void Download()
        {
            DataProviderBase dataSource = DataProviderFactory.GetInstance(_dataProviderType);

            //Obtenemos informacion del origen de datos
            List<TramoImport> info = dataSource.GetInfo();

            //Ordenamos la información
            info = info
                .OrderBy(x => x.Provincia)
                .ThenBy(x => x.Ruta)
                .ThenBy(x => x.TramoNormalizado)
                .ToList();

            //Normalizamos la información
            info.ForEach(x => x.TramoNormalizado = NormalizerTramo.Normalize(x.TramoNormalizado));

            //Eliminamos duplicados
            info = info.Distinct(new ItemRutaComparer()).ToList();

            //Grabamos la información
            SaveData(info);
        }
        #endregion

        #region Abstract Methods
        public abstract void SaveData(List<TramoImport> info);
        #endregion
    }
}
