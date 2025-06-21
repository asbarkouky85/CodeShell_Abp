using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Codeshell.Abp.Results;
using Codeshell.Abp.Extensions;
using Codeshell.Abp.Exceptions;

namespace Codeshell.Abp.Results
{
    public class Result : IResult
    {
        public Dictionary<string, object> Data { get; set; }
        public virtual int Code { get; set; }
        public virtual string Message { get; set; }
        public virtual string ExceptionMessage { get; set; }
        public virtual string[] StackTrace { get; set; }
        public virtual bool IsSuccess { get { return Code == 0; } }
        public virtual Result InnerResult { get; set; }

        protected Exception _exception;

        public Result()
        {
            Data = new Dictionary<string, object>();
        }

        public Result(int code)
        {
            Code = code;
            Data = new Dictionary<string, object>();
        }

        public TOut MapToResult<TOut>(TOut result = null) where TOut : Result
        {
            var inst = result ?? Activator.CreateInstance<TOut>();

            inst.Message = Message;
            inst.StackTrace = StackTrace;
            inst.Data = Data;
            var ex = inst.GetException();
            if (ex != null)
                inst.SetException(ex);
            else
                inst.ExceptionMessage = ExceptionMessage;
            inst.Code = Code;

            return inst;
        }

        public Exception GetException()
        {
            return _exception;
        }

        public virtual void SetException(Exception e, bool recurse = false)
        {
            _exception = e;
            if (Code == 200)
                Code = 500;

            if (_exception is RemoteServerException)
            {
                var res = (_exception as RemoteServerException).RemoteResult;
                if ((_exception as RemoteServerException).RemoteResult != null)
                {
                    Code = res.Code;
                    Message = res.Message;
                    ExceptionMessage = res.ExceptionMessage;
                    StackTrace = res.StackTrace;
                    return;
                }
            }

            if (_exception.InnerException != null && _exception.InnerException is RemoteServerException)
            {
                try
                {
                    InnerResult = ((RemoteServerException)_exception.InnerException).RemoteResult;
                    ExceptionMessage = "Check inner result";
                }
                catch
                {
                    ExceptionMessage = e.GetMessageRecursive();
                }
            }
            else
            {
                ExceptionMessage = e.GetMessageRecursive();
            }


            if (e.StackTrace != null)
                StackTrace = e.GetStackTrace();
        }

        public T ReadFromDataAs<T>(string index) where T : class
        {
            if (Data.TryGetValue(index, out object item))
            {
                JObject ob = (JObject)item;
                return ob.ToObject<T>();
            }
            return null;
        }
    }


}
