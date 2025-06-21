using System.Net;

namespace Codeshell.Abp.Results
{
    public class RemoteServerResult : Result
    {
        public string RequestUrl { get; set; }
        public string Method { get; set; }

        public RemoteServerResult()
        {
            Code = 200;
        }

        public RemoteServerResult(HttpStatusCode code, string message = null)
        {
            Code = (int)code;
            Message = message;
        }

        public void SetStatusCode(HttpStatusCode code)
        {
            Code = (int)code;
        }

    }
}
