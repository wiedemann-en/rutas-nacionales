using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vialidad.Contracts.Models;
using Vialidad.Contracts.Services;
using Vialidad.Services;
using Vialidad.Utils.Extensions;
using Vialidad.Web.Api.Extensions;
using Vialidad.Web.Api.Mapping;
using Vialidad.Web.Api.Models.Infrastructure;
using Vialidad.Web.Api.Models.Logic;

namespace Vialidad.Web.Api.Controllers
{
    [RoutePrefix("tramos")]
    public class TramoController : ApiController
    {
        #region Properties
        private readonly IServiceProvincia _serviceProvincia;
        private readonly IServiceRuta _serviceRuta;
        private readonly IServiceCalzada _serviceCalzada;
        private readonly IServiceTramo _serviceTramo;
        private readonly IServiceReferencia _serviceReferencia;
        #endregion

        #region Constructors
        public TramoController(IServiceProvincia serviceProvincia,
            IServiceRuta serviceRuta,
            IServiceCalzada serviceCalzada,
            IServiceTramo serviceTramo,
            IServiceReferencia serviceReferencia)
        {
            _serviceProvincia = serviceProvincia;
            _serviceRuta = serviceRuta;
            _serviceCalzada = serviceCalzada;
            _serviceTramo = serviceTramo;
            _serviceReferencia = serviceReferencia;
        }
        #endregion

        #region WebApi Methods
        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            IEnumerable<TramoDto> tramos = _serviceTramo.GetAll();
            IEnumerable<TramoModel> tramosVM = MapDtoToViewModel.Map(tramos);
            NormalizeInfoVM(tramos, tramosVM);
            Result result = new Result(tramosVM);
            return result.CreateResponse(this);
        }

        [HttpPost]
        [Route("all/byfilter")]
        public IHttpActionResult GetAllByFilter([FromBody]TramoFiltroDto filtro)
        {
            IEnumerable<TramoDto> tramos = _serviceTramo.GetAll(filtro);
            IEnumerable<TramoModel> tramosVM = MapDtoToViewModel.Map(tramos);
            NormalizeInfoVM(tramos, tramosVM);
            Result result = new Result(tramosVM);
            return result.CreateResponse(this);
        }

        [HttpGet]
        [Route("routing/{profile}/{coordinates}")]
        public IHttpActionResult GetRouting(string profile, string coordinates, bool overview = false, bool alternatives = false, bool steps = false, string hints = "")
        {
            coordinates = coordinates.Replace(";", "/").Trim();
            TramoDto tramo = _serviceTramo.GetByCoordinates(coordinates);
            string jsonRouting = (tramo != null) ? tramo.JsonRouting : string.Empty;
            JObject jsonObj = null;
            if (!string.IsNullOrEmpty(jsonRouting))
                jsonObj = JObject.Parse(jsonRouting);
            Result result = new Result(jsonObj);
            return result.CreateResponse(this);
        }

        [HttpGet]
        [Route("bycoordinates/{startLatitude}/{startLongitude}/{endLatitude}/{endLongitude}")]
        public IHttpActionResult GetByCoordinates(string startLatitude, string startLongitude, string endLatitude, string endLongitude)
        {
            Result result = new Result("Test");
            return result.CreateResponse(this);
        }
        #endregion

        #region Helpers
        private void NormalizeInfoVM(IEnumerable<TramoDto> tramos, IEnumerable<TramoModel> tramosVM)
        {
            IList<TramoModel> result = new List<TramoModel>();

            IEnumerable<ProvinciaDto> provincias = _serviceProvincia.GetAll(false);
            IEnumerable<RutaDto> rutas = _serviceRuta.GetAll(false);
            IEnumerable<CalzadaDto> calzadas = _serviceCalzada.GetAll(false);
            IEnumerable<ReferenciaDto> referencias = _serviceReferencia.GetAll();

            foreach (var itemTramoVM in tramosVM)
            {
                TramoDto tramoDto = tramos.SingleOrDefault(x => x.IdTramo == itemTramoVM.IdTramo);
                if (tramoDto == null)
                    continue;

                itemTramoVM.Provincia = provincias.SingleOrDefault(x => x.Id == itemTramoVM.IdProvincia)?.Nombre;
                itemTramoVM.Ruta = rutas.SingleOrDefault(x => x.Id == itemTramoVM.IdRuta)?.Nombre;
                itemTramoVM.Calzada = calzadas.SingleOrDefault(x => x.Id == itemTramoVM.IdCalzada)?.Nombre;
                itemTramoVM.Referencias = tramoDto.GetListReferencias(referencias);
            }
        }
        #endregion
    }
}
