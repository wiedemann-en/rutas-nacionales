using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Models;
using Vialidad.Contracts.Services;
using Vialidad.Logger.Interfaces;
using Vialidad.Logger.Logic;
using Vialidad.Services;

namespace Vialidad.Routing
{
    public class RoutingCalculator
    {
        #region Private Attributes
        private readonly ILogger _logger;
        private readonly IServiceTramo _serviceTramo;
        #endregion

        #region Constructors
        public RoutingCalculator()
        {
            _logger = LoggerFactory.GetInstance();
            _serviceTramo = new ServiceTramo();
        }
        #endregion

        #region Public Methods
        public void CalculatePendingRoutes(string profile, bool overview = false, bool alternatives = false, bool steps = false)
        {
            //Obtenemos todos los tramos disponibles
            IEnumerable<TramoDto> tramos = _serviceTramo.GetAll(false);

            //Nos quedamos solo con aquellos registros que posean coordenadas y no posean configuración de ruteo
            tramos = tramos.Where(x =>
                (!string.IsNullOrEmpty(x.Coordenadas)) &&
                (string.IsNullOrEmpty(x.JsonRouting)));

            int updatedRows = 0;

            foreach (var itemTramo in tramos)
            {
                if (itemTramo.Coordenadas.StartsWith("//"))
                    continue;

                try
                {
                    //Determinamos coordenadas. Son al reves que lo grabado.
                    List<string> coordenadas = new List<string>();
                    foreach (var itemCoordenada in itemTramo.Coordenadas.Split('/'))
                    {
                        string coordToAdd = $"{itemCoordenada.Split(',')[1]},{itemCoordenada.Split(',')[0]}";
                        coordenadas.Add(coordToAdd);
                    }

                    //Contruimos la url de consulta
                    string url = string.Format("https://s.ruta0.net/ruteo3.aspx?/{0}/{1}?overview={2}&alternatives={3}&steps={4}",
                        profile, 
                        string.Join(";", coordenadas), 
                        overview, 
                        alternatives, 
                        steps);

                    //Obtenemos los datos de ruteo. Por el momento se toman de ruta0
                    WebClient wc = new WebClient();
                    byte[] response = wc.DownloadData(url);

                    string jsonContent = Encoding.UTF8.GetString(response);
                    if (string.IsNullOrEmpty(jsonContent))
                        continue;

                    //Actualizamos la información
                    itemTramo.JsonRouting = jsonContent;
                    _serviceTramo.UpdateRouting(itemTramo);
                    updatedRows++;
                }
                catch (Exception ex)
                {
                    _logger.Error("RoutingCalculator.CalculateAllPendingRoutes", ex.Message, ex);
                }
            }

            _logger.Info("RoutingCalculator.CalculateAllPendingRoutes", $"{updatedRows} registros actualizados.");
        }
        #endregion
    }
}
