using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vialidad.Contracts.Models;
using Vialidad.Contracts.Services;
using Vialidad.Services.Normalizer;
using Vialidad.Web.Api.Extensions;
using Vialidad.Web.Api.Mapping;
using Vialidad.Web.Api.Models.Infrastructure;
using Vialidad.Web.Api.Models.Logic;

namespace Vialidad.Web.Api.Controllers
{
    [RoutePrefix("provincias")]
    public class ProvinciaController : ApiController
    {
        #region Properties
        private readonly IServiceProvincia _serviceProvincia;
        #endregion

        #region Constructors
        public ProvinciaController(IServiceProvincia serviceProvincia)
        {
            _serviceProvincia = serviceProvincia;
        }
        #endregion

        #region WebApi Methods
        [HttpGet]
        [Route("allinfo")]
        public IHttpActionResult GetAllInfo()
        {
            IEnumerable<ProvinciaDto> provincias = _serviceProvincia.GetAll(false);
            IEnumerable<InfoMapModel> provinciasVM = MapDtoToViewModel.MapInfo(provincias);
            provinciasVM = provinciasVM.Where(x => NormalizerKey.Normalize(x.Nombre) != "bahia-blanca");
            Result result = new Result(provinciasVM);
            return result.CreateResponse(this);
        }

        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            IEnumerable<ProvinciaDto> provincias = _serviceProvincia.GetAll(true);
            IEnumerable<DropDownItemModel> provinciasVM = MapDtoToViewModel.Map(provincias);
            provinciasVM = provinciasVM.Where(x => NormalizerKey.Normalize(x.Nombre) != "bahia-blanca");
            Result result = new Result(provinciasVM);
            return result.CreateResponse(this);
        }

        [HttpGet]
        [Route("all/{idRuta:int}")]
        public IHttpActionResult GetAllByRuta(int idRuta)
        {
            IEnumerable<ProvinciaDto> provincias = _serviceProvincia.GetAllByRuta(idRuta);
            IEnumerable<DropDownItemModel> provinciasVM = MapDtoToViewModel.Map(provincias);
            provinciasVM = provinciasVM.Where(x => NormalizerKey.Normalize(x.Nombre) != "bahia-blanca");
            Result result = new Result(provinciasVM);
            return result.CreateResponse(this);
        }
        #endregion
    }
}
