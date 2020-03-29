using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vialidad.Web.Api.Models.Infrastructure;

namespace Vialidad.Web.Api.Extensions
{
    public static class ResultExtensionMethods
    {
        public static DomainActionResult CreateResponse(this Result result, ApiController controller)
        {
            return new DomainActionResult(controller.Request, result);
        }

        public static List<ValidationResult> Validate<T>(this T model)
        {
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);
            return validationResults;
        }
    }
}