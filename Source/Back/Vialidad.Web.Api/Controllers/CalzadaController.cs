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
    [RoutePrefix("calzadas")]
    public class CalzadaController : ApiController
    {
        #region Properties
        private readonly IServiceCalzada _serviceCalzada;
        #endregion

        #region Constructors
        public CalzadaController(IServiceCalzada serviceCalzada)
        {
            _serviceCalzada = serviceCalzada;
        }
        #endregion

        #region WebApi Methods
        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAll()
        {
            IEnumerable<CalzadaDto> calzadas = _serviceCalzada.GetAll(true);
            IEnumerable<DropDownItemModel> calzadasVM = MapDtoToViewModel.Map(calzadas);
            Result result = new Result(calzadasVM);
            return result.CreateResponse(this);
        }
        #endregion
    }
}
