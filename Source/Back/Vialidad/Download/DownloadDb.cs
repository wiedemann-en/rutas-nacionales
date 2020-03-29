using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Enums;
using Vialidad.Contracts.Models;
using Vialidad.Contracts.Services;
using Vialidad.Models;
using Vialidad.Services;
using Vialidad.Services.Normalizer;

namespace Vialidad.Download
{
    public class DownloadDb : DownloadBase
    {
        #region Private Attributes
        private readonly IServiceProvincia _serviceProvincia;
        private readonly IServiceRuta _serviceRuta;
        private readonly IServiceCalzada _serviceCalzada;
        private readonly IServiceTramo _serviceTramo;
        #endregion

        #region Constructors
        public DownloadDb(enDataProviderType dataProviderType)
            : base(dataProviderType)
        {
            _serviceProvincia = new ServiceProvincia();
            _serviceRuta = new ServiceRuta();
            _serviceCalzada = new ServiceCalzada();
            _serviceTramo = new ServiceTramo();
        }
        #endregion

        #region Overrides
        public override void SaveData(List<TramoImport> info)
        {
            var provincias = _serviceProvincia.GetAll(false);
            var rutas = _serviceRuta.GetAll(false);
            var calzadas = _serviceCalzada.GetAll(false);

            foreach (var itemTramo in info)
            {
                if (string.IsNullOrEmpty(itemTramo.TramoDesnormalizado))
                    continue;

                try
                {
                    //Verificamos información de la provincia
                    var provinciaKey = NormalizerKey.Normalize(itemTramo.Provincia);
                    if (provinciaKey == "bahia-blanca")
                        provinciaKey = "buenos-aires";
                    var provinciaDto = provincias.SingleOrDefault(x => x.Key == provinciaKey);
                    if (provinciaDto == null)
                    {
                        provinciaDto = new ProvinciaDto();
                        provinciaDto.Nombre = itemTramo.Provincia;
                        provinciaDto.Key = provinciaKey;
                        provinciaDto.Id = _serviceProvincia.Create(provinciaDto);
                    }

                    //Verificamos información de la ruta
                    var rutaKey = NormalizerKey.Normalize(itemTramo.Ruta);
                    var rutaDto = rutas.SingleOrDefault(x => x.Key == rutaKey);
                    if (rutaDto == null)
                    {
                        rutaDto = new RutaDto();
                        rutaDto.Nombre = itemTramo.Ruta;
                        rutaDto.Key = rutaKey;
                        rutaDto.Id = _serviceRuta.Create(rutaDto);
                    }

                    //Verificamos información de la calzada
                    var calzadaKey = NormalizerKey.Normalize(itemTramo.Calzada);
                    if (calzadaKey.StartsWith("pavimento-flexible"))
                        calzadaKey = "pavimento-flexible";
                    var calzadaDto = calzadas.SingleOrDefault(x => x.Key == calzadaKey);
                    if (calzadaDto == null)
                    {
                        if (!string.IsNullOrEmpty(itemTramo.Calzada))
                        {
                            calzadaDto = new CalzadaDto();
                            calzadaDto.Nombre = itemTramo.Calzada;
                            calzadaDto.Key = calzadaKey;
                            calzadaDto.Id = _serviceCalzada.Create(calzadaDto);
                        }
                        else
                        {
                            //Valor default
                            calzadaDto = new CalzadaDto();
                            calzadaDto.Id = 1;
                        }
                    }

                    //Mapeamos dto
                    TramoDto tramoDto = new TramoDto();
                    tramoDto.IdProvincia = provinciaDto.Id;
                    tramoDto.IdRuta = rutaDto.Id;
                    tramoDto.IdCalzada = calzadaDto.Id;
                    tramoDto.TramoNormalizado = itemTramo.TramoNormalizado;
                    tramoDto.TramoDesnormalizado = itemTramo.TramoDesnormalizado;
                    tramoDto.Detalle = itemTramo.Detalle;
                    tramoDto.Observaciones = itemTramo.Observaciones;
                    tramoDto.FechaActualizacion = itemTramo.Actualizacion;
                    tramoDto.Coordenadas = itemTramo.Coordenadas;

                    //Grabamos información del tramo
                    _serviceTramo.CreateOrUpdate(tramoDto);
                }
                catch (Exception ex)
                {
                    _logger.Error("DownloadDb.SaveData", ex.Message, ex);
                }
            }

            _logger.Info("DownloadDb.SaveData", $"{info.Count} registros grabados.");
        }
        #endregion
    }
}
