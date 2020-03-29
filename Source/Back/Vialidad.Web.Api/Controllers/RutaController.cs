using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vialidad.Contracts.Models;
using Vialidad.Contracts.Services;
using Vialidad.Web.Api.Extensions;
using Vialidad.Web.Api.Mapping;
using Vialidad.Web.Api.Models.Infrastructure;
using Vialidad.Web.Api.Models.Logic;

namespace Vialidad.Web.Api.Controllers
{
    [RoutePrefix("rutas")]
    public class RutaController : ApiController
    {
        #region Properties
        private readonly IServiceRuta _serviceRuta;
        #endregion

        #region Constructors
        public RutaController(IServiceRuta serviceRuta)
        {
            _serviceRuta = serviceRuta;
        }
        #endregion

        #region WebApi Methods
        [HttpGet]
        [Route("allinfo")]
        public IHttpActionResult GetAllInfo()
        {
            IEnumerable<RutaDto> rutas = _serviceRuta.GetAll(false);
            IEnumerable<InfoMapModel> rutasVM = MapDtoToViewModel.MapInfo(rutas);
            Result result = new Result(rutasVM);
            return result.CreateResponse(this);
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            IEnumerable<RutaDto> rutas = _serviceRuta.GetAll(true);
            IEnumerable<DropDownItemModel> rutasVM = MapDtoToViewModel.Map(rutas);
            Result result = new Result(rutasVM);
            return result.CreateResponse(this);
        }

        [HttpGet]
        [Route("all/{idProvincia:int}")]
        public IHttpActionResult GetAllByProvincia(int idProvincia)
        {
            IEnumerable<RutaDto> rutas = _serviceRuta.GetAllByProvincia(idProvincia);
            IEnumerable<DropDownItemModel> rutasVM = MapDtoToViewModel.Map(rutas);
            Result result = new Result(rutasVM);
            return result.CreateResponse(this);
        }
        #endregion
    }
}
