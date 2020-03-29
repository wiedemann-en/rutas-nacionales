using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vialidad.Web.Api.Models.Infrastructure
{
    public class Result
    {
        public readonly List<ResultError> _errors;

        public object ResultData { get; set; }
        public int TotalRecords { get; set; }

        public Result(object data)
            : this()
        {
            this.ResultData = data;
        }

        public Result()
        {
            this._errors = new List<ResultError>();
        }

        public virtual bool HasErrors
        {
            get
            {
                return this._errors.Any();
            }
        }

        public void AddError(string message)
        {
            this._errors.Add(new ResultError(message));
        }

        public void AddError(string message, string errorCode)
        {
            this._errors.Add(new ResultError(errorCode, message, new List<string>()));
        }

        public void AddError(string message, string errorCode, string memberName)
        {
            this._errors.Add(new ResultError(errorCode, message, new List<string> { memberName }));
        }

        public void AddError(ResultError validationResult)
        {
            this._errors.Add(validationResult);
        }

        public void AddErrorRange(IEnumerable<ResultError> validationResults)
        {
            this._errors.AddRange(validationResults);
        }

        public void AddErrorRange(IEnumerable<string> errorMessages)
        {
            foreach (var error in errorMessages)
                this._errors.Add(new ResultError(error));
        }

        public Result AddErrorFluent(string message)
        {
            this._errors.Add(new ResultError(message));
            return this;
        }
    }

    public sealed class Result<T> : Result
    {
        private T _data;

        public Result()
        {
        }

        public Result(T data)
        {
            this.Data = data;
        }

        public T Data
        {
            get
            {
                return this._data;
            }

            set
            {
                this._data = value;
                this.ResultData = _data;
            }
        }

        public Result<TTarget> ToResult<TTarget>(Func<T, TTarget> converter = null)
        {
            if (HasErrors)
            {
                Result<TTarget> errorsResult = new Result<TTarget>();
                errorsResult.AddErrorRange(_errors);
                return errorsResult;
            }

            if (converter == null)
                return new Result<TTarget>();

            var translated = converter(Data);

            return new Result<TTarget>(translated);
        }

        public Result<T> AddErrors(Result result)
        {
            this._errors.AddRange(result._errors);
            return this;
        }

        public new Result<T> AddErrorFluent(string message)
        {
            this._errors.Add(new ResultError(message));
            return this;
        }
    }
}