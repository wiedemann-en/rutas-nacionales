using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vialidad.Fixture;
using Vialidad.Logger.Interfaces;
using Vialidad.Normalizer;
using Vialidad.Routing;
using Vialidad.Web.Api.Extensions;
using Vialidad.Web.Api.Models.Infrastructure;

namespace Vialidad.Web.Api.Controllers
{
    [RoutePrefix("setup")]
    public class SetupController : ApiController
    {
        #region Constants
        private const string KAccessKey = "fLV08Pvz1wKHRMg9Q";
        #endregion

        #region Properties
        private readonly ILogger _logger;
        #endregion

        #region Constructors
        public SetupController(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region WebApi Methods
        [HttpGet]
        [Route("downloadinfo/{accessKey}")]
        public IHttpActionResult DownloadInfo(string accessKey)
        {
            Result result = new Result(true);
            try
            {
                if (accessKey != KAccessKey)
                {
                    result.ResultData = false;
                    result.AddError("Invalid accessKey.");
                }
                else
                {
                    ScraperFixture fixture = new ScraperFixture();
                    fixture.ExecuteFixture();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("WebApi", "SetupController: DownloadInfo", ex);
                result.ResultData = false;
                result.AddError($"Error al ejecutar el proceso de descarga: {ex.Message}");
            }
            return result.CreateResponse(this);
        }

        [HttpGet]
        [Route("normalizedb/{accessKey}")]
        public IHttpActionResult NormalizeDb(string accessKey)
        {
            Result result = new Result(true);
            try
            {
                if (accessKey != KAccessKey)
                {
                    result.ResultData = false;
                    result.AddError("Invalid accessKey.");
                }
                else
                {
                    ScraperNormalizer normalizer = new ScraperNormalizer();
                    normalizer.NormalizeDb();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("WebApi", "SetupController: NormalizeDb", ex);
                result.ResultData = false;
                result.AddError($"Error al ejecutar el proceso de normalización: {ex.Message}");
            }
            return result.CreateResponse(this);
        }

        [HttpGet]
        [Route("calculatependingroutes/{accessKey}")]
        public IHttpActionResult CalculatePendingRoutes(string accessKey, string profile, bool overview = false, bool alternatives = false, bool steps = false)
        {
            Result result = new Result(true);
            try
            {
                if (accessKey != KAccessKey)
                {
                    result.ResultData = false;
                    result.AddError("Invalid accessKey.");
                }
                else
                {
                    RoutingCalculator routingCalculator = new RoutingCalculator();
                    routingCalculator.CalculatePendingRoutes(profile, overview, alternatives, steps);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("WebApi", "SetupController: CalculatePendingRoutes", ex);
                result.ResultData = false;
                result.AddError($"Error al ejecutar el proceso de cálculo de rutas: {ex.Message}");
            }
            return result.CreateResponse(this);
        }
        #endregion
    }
}
