using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        static RoutingCalculator()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
        }

        #region Public Methods
        public void CalculatePendingRoutes(string profile, bool overview = false, bool alternatives = false, bool steps = false)
        {
            //var urlTest = @"https://router.project-osrm.org/route/v1/driving/-58.51845247691521,-34.66250892873649;-58.59302218953386,-34.71521833939614;-58.70131246959419,-34.906246724083914;-58.73529205702289,-35.033556752201775?overview=false&alternatives=false&steps=false";
            //var resultTest = DownloadWithPython(urlTest);

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
                    //string url = string.Format("https://s.ruta0.net/ruteo3.aspx?/{0}/{1}?overview={2}&alternatives={3}&steps={4}",
                    //    profile, 
                    //    string.Join(";", coordenadas), 
                    //    overview, 
                    //    alternatives, 
                    //    steps);
                    string url = string.Format("https://router.project-osrm.org/route/v1/{0}/{1}?overview={2}&alternatives={3}&steps={4}",
                        profile,
                        string.Join(";", coordenadas),
                        overview.ToString().ToLower(),
                        alternatives.ToString().ToLower(),
                        steps.ToString().ToLower());

                    string jsonContent = DownloadWithPython(url);
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

            if (updatedRows > 0)
                _serviceTramo.SaveChanges();

            _logger.Info("RoutingCalculator.CalculateAllPendingRoutes", $"{updatedRows} registros actualizados.");
        }
        private string DownloadWithPython(string url)
        {
            string script = $"import urllib.request; r=urllib.request.urlopen('{url}'); print(r.read().decode('utf-8'))";

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "python.exe",
                    Arguments = $"-c \"{script}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    string error = process.StandardError.ReadToEnd();
                    throw new Exception($"Python error (exit {process.ExitCode}): {error}");
                }

                return output;
            }
        }
        #endregion
    }
}
