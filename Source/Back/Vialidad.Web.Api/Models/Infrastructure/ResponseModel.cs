using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vialidad.Web.Api.Models.Infrastructure
{
    public class ResponseModel
    {
        public string Status { get; set; }
        public object Data { get; set; }
        public IEnumerable<ResultError> Errors { get; set; }
    }
}