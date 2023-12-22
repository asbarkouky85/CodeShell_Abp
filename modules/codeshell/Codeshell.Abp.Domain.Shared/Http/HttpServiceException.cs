using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Codeshell.Abp.Http
{
    [Serializable]
    public class HttpServiceException : Exception
    {
        public string Url { get; private set; }
        public string Code { get; private set; }
        public HttpStatusCode HttpCode { get; private set; }
        private readonly string message;
        public override string Message => message;
        public string ServiceResponse { get; private set; }
        public HttpServiceException(HttpStatusCode code, string response, Uri url = null)
        {
            HttpCode = code;
            ServiceResponse = response;
            message = "RequestFailed";
            Url = url?.AbsoluteUri;
            try
            {
                var re = (JObject)JsonConvert.DeserializeObject(response);
                if (re != null)
                {
                    if (re.ContainsKey("message"))
                        message = re["message"].ToString();
                    if (re.ContainsKey("code"))
                        Code = re["code"].ToString();
                }
            }
            catch (Exception)
            {
                message = response;
            }

        }

        protected HttpServiceException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }
}
