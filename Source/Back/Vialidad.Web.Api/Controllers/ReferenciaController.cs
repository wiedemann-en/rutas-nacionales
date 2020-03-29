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
    [RoutePrefix("referencias")]
    public class ReferenciaController : ApiController
    {
        #region Properties
        private readonly IServiceReferencia _serviceReferencia;
        #endregion

        #region Constructors
        public ReferenciaController(IServiceReferencia serviceReferencia)
        {
            _serviceReferencia = serviceReferencia;
        }
        #endregion

        #region WebApi Methods
        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            IEnumerable<ReferenciaDto> referencias = _serviceReferencia.GetAll();
            IEnumerable<ReferenciaModel> calzadasVM = MapDtoToViewModel.Map(referencias);
            Result result = new Result(calzadasVM);
            return result.CreateResponse(this);
        }
        #endregion
    }
}
