using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vialidad.Web.Api.Models.Infrastructure
{
   public class ResultError : ValidationResult
    {
        public string ErrorCode { get; set; }

        public ResultError()
            : base("error")
        {
        }

        public ResultError(string errorMessage)
            : base(errorMessage)
        {
        }

        public ResultError(string errorMessage, IEnumerable<string> memberNames)
            : base(errorMessage, memberNames)
        {
        }

        public ResultError(string errorCode, string errorMessage, IEnumerable<string> memberNames)
            : base(errorMessage, memberNames)
        {
            this.ErrorCode = errorCode;
        }

        public ResultError(ValidationResult validationResult)
            : base(validationResult)
        {
        }
    }
}