using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Vialidad.Web.Api.Models.Infrastructure
{
    public class DomainActionResult : IHttpActionResult
    {
        internal const string ContentProperty = "ResultData";

        internal readonly HttpRequestMessage Request;

        public HttpStatusCode HttpStatusCode { get; private set; }

        public Result Result { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DomainActionResult" /> class with a request message.
        /// </summary>
        /// <param name="request">The request message which led to this result.</param>
        protected DomainActionResult(HttpRequestMessage request)
        {
            Request = request;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DomainActionResult" /> class based on a particular result.
        /// </summary>
        /// <param name="request">The request message which led to this result.</param>
        /// <param name="result">The result of a business workflow that needs to be communicated to the referrer.</param>
        public DomainActionResult(HttpRequestMessage request, Result result)
            : this(request)
        {
            Result = result;

            SetHttpStatusCode();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DomainActionResult" /> class based on a collectionn of validation
        ///     errors.
        /// </summary>
        /// <param name="request">The request message which led to this result.</param>
        /// <param name="issues">Validation errors result of a business workflow or some business validations.</param>
        public DomainActionResult(HttpRequestMessage request, IEnumerable<ResultError> issues)
            : this(request)
        {
            Result = new Result();

            Result.AddErrorRange(issues);

            SetHttpStatusCode();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:DomainActionResult" /> class based on a model state.
        /// </summary>
        /// <param name="request">The request message which led to this result.</param>
        /// <param name="modelState">Result of a model validation.</param>
        public DomainActionResult(HttpRequestMessage request, ModelStateDictionary modelState)
            : this(request)
        {
            var issues =
                modelState.Select(err => new ResultError(err.Value.Errors.First().ErrorMessage));

            Result = new Result();

            Result.AddErrorRange(issues);

            SetHttpStatusCode();
        }

        private void SetHttpStatusCode()
        {
            HttpStatusCode = Result.HasErrors
                ? HttpStatusCode.BadRequest // Http422UnprocessableEntity
                : HttpStatusCode.OK;
        }

        internal Func<HttpRequestMessage, HttpStatusCode, object, HttpResponseMessage> CreateResponse { get; set; }

        /// <summary>
        ///     Creates an <see cref="T:System.Net.Http.HttpResponseMessage" /> asynchronously.
        /// </summary>
        /// <returns>
        ///     A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.
        /// </returns>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new ResponseModel
            {
                Data = GetContentsFromResult(),
                Errors = Result._errors.ToList(),
                Status = Result.HasErrors ? StatusType.Error.ToString().ToUpper() : StatusType.Success.ToString().ToUpper()
            };

            var responseData = response;

            var httpResponse = Request.CreateResponse(HttpStatusCode, responseData);

            return Task.FromResult(httpResponse);
        }

        private object GetContentsFromResult()
        {
            var t = Result.GetType();

            return
                t.GetProperty(ContentProperty) != null
                ? t.GetProperty(ContentProperty).GetValue(Result)
                : null;
        }
    }
}