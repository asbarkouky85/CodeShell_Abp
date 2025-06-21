using Codeshell.Abp.Extensions;
using Codeshell.Abp.Results;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.Threading;

namespace Codeshell.Abp.Exceptions
{
    public class RemoteServerException : Exception
    {
        private string _message;
        public HttpStatusCode Status { get; private set; }
        public RemoteServerResult RemoteResult { get; private set; }
        public override string Message { get { return _message; } }
        public string GetFullMessage()
        {
            return this.GetMessageRecursive() + $" E: {RemoteResult?.ExceptionMessage}";
        }
        public RemoteServerException(HttpResponseMessage mes, Uri uri = null, string method = null)
        {
            AsyncHelper.RunSync(async () =>
            {
                await _fillFromResponseAsync(this, mes, uri, method);
            });
        }

        private RemoteServerException()
        {

        }

        private static async Task _fillFromResponseAsync(RemoteServerException result, HttpResponseMessage mes, Uri uri = null, string method = null)
        {
            result.Status = mes.StatusCode;
            var res = await mes.Content.ReadAsStringAsync();
            uri = uri ?? mes.RequestMessage.RequestUri;
            method = method ?? mes.RequestMessage?.Method.ToString();
            if (res.TryRead(out Result resData))
            {
                result._message = resData.Message;
                result.RemoteResult = new RemoteServerResult
                {
                    RequestUrl = uri != null ? uri.AbsoluteUri : mes.RequestMessage?.RequestUri.ToString(),
                    Code = (int)mes.StatusCode,
                    Message = "Server responded with error : " + resData.Message,
                    ExceptionMessage = resData.ExceptionMessage,
                    Method = method
                };
            }
            else
            {
                result._message = res;
                result.RemoteResult = new RemoteServerResult
                {
                    RequestUrl = uri != null ? uri.AbsoluteUri : mes.RequestMessage?.RequestUri.ToString(),
                    Code = (int)mes.StatusCode,
                    Message = mes.StatusCode.ToString(),
                    ExceptionMessage = await mes.Content.ReadAsStringAsync(),
                    Method = method ?? mes.RequestMessage?.Method.ToString()
                };
            }
        }

        public async static Task<RemoteServerException> FromResponse(HttpResponseMessage mes, Uri uri = null, string method = null)
        {
            var ex = new RemoteServerException();
            await _fillFromResponseAsync(ex, mes, uri, method);
            return ex;
        }

        public RemoteServerException(HttpStatusCode code, string message)
        {
            Status = code;
            _message = message;
            RemoteResult = new RemoteServerResult
            {
                Code = (int)code,
                Message = code.ToString(),
                ExceptionMessage = message,
            };
        }

        public RemoteServerException(RemoteServerResult message)
        {
            Status = (HttpStatusCode)message.Code;
            _message = message.Message;

            RemoteResult = message;
        }
    }
}
