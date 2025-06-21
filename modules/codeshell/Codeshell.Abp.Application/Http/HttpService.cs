using Codeshell.Abp.Exceptions;
using Codeshell.Abp.Extensions;
using Codeshell.Abp.Files;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Codeshell.Abp.Http
{
    public class HttpService : ApplicationService, IHttpService
    {
        private HttpClient Client;
        public virtual string BaseUrl { get; set; } = "";
        protected virtual object AppendToQuery { get; set; }

        private Dictionary<string, string> _headers;
        public Dictionary<string, string> Headers
        {
            get
            {
                if (_headers == null)
                    _headers = new Dictionary<string, string>();
                return _headers;
            }
        }

        static HttpService()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
        }

        public HttpService()
        {

        }

        protected virtual Uri AppendQuery(Uri uri, object obj)
        {
            PropertyInfo[] infs = obj.GetType().GetProperties();
            string qm = string.IsNullOrEmpty(uri.Query) ? "?" : "&";
            List<string> data = new List<string>();
            foreach (PropertyInfo inf in infs)
            {
                var val = inf.GetValue(obj);
                if (val != null)
                    data.Add(inf.Name + "=" + val.ToString());
            }
            string ur = uri.AbsoluteUri + qm + string.Join("&", data);
            return new Uri(ur);
        }

        protected virtual void SetSecurityProtocol() { }

        protected virtual Uri GetUri(string url, object query = null)
        {
            Uri uri = new Uri(Utils.CombineUrl(BaseUrl, url));

            if (query != null)
                uri = AppendQuery(uri, query);
            if (AppendToQuery != null)
                uri = AppendQuery(uri, AppendToQuery);

            return uri;
        }

        protected virtual Task BuildClientAsync(HttpClient cl)
        {
            if (Headers.Count > 0)
            {
                foreach (var h in Headers)
                    Client.DefaultRequestHeaders.TryAddWithoutValidation(h.Key, h.Value);
                SetSecurityProtocol();
            }
            return Task.FromResult(new { });
        }

        public async Task<T> PostAsyncAs<T>(string url, object data, object query = null) where T : class
        {
            HttpResponseMessage res = await ExecutePostAsync(url, data, query);
            return await DeserializeAsync<T>(res);
        }
        public async Task<T> PostUrlEncoded<T>(string url, object data, object query = null) where T : class
        {
            var res = await PostUrlEncodedAsync(url, data, query);

            if (!res.IsSuccessStatusCode)
                throw new RemoteServerException(res);
            else
            {
                var content = await res.Content.ReadAsStringAsync();
                return content.FromJson<T>();
            }
        }

        public async Task<HttpResponseMessage> PostUrlEncodedAsync(string url, object data, object query = null)
        {
            var Client = new HttpClient();
            await BuildClientAsync(Client);
            Uri uri = GetUri(url, query);

            var c = data.ToDictionaryOfProperties();
            var content = new FormUrlEncodedContent(c);

            HttpResponseMessage res = await Client.PostAsync(uri, content);

            if (res.IsSuccessStatusCode)
            {
                return res;
            }
            else
            {
                throw new RemoteServerException(res);
            }

        }

        protected virtual async Task<Exception> ToExceptionAsync(HttpResponseMessage res)
        {
            var st = await res.Content.ReadAsStringAsync();
            return new RemoteServerException(res);
        }

        protected virtual async Task<HttpResponseMessage> ExecuteGetAsync(string url, object query = null)
        {
            Client = new HttpClient();
            await BuildClientAsync(Client);
            Uri uri = GetUri(url, query);

            try
            {
                HttpResponseMessage mes = await Client.GetAsync(uri);
                return new HttpResponseMessage()
                {
                    RequestMessage = mes.RequestMessage,
                    StatusCode = mes.StatusCode,
                    Content = mes.Content
                };
            }
            catch (Exception ex)
            {
                return ExceptionToResponse(ex);
            }

        }

        protected virtual async Task<HttpResponseMessage> ExecutePutAsync(string url, object data, object query = null)
        {
            Client = new HttpClient();
            await BuildClientAsync(Client);
            Uri uri = GetUri(url, query);

            try
            {
                var st = data.ToJson();
                HttpResponseMessage mes = await Client.PutAsync(uri, new StringContent(st, Encoding.UTF8, "application/json"));
                return new HttpResponseMessage()
                {
                    RequestMessage = mes.RequestMessage,
                    StatusCode = mes.StatusCode,
                    Content = mes.Content
                };
            }
            catch (Exception ex)
            {
                return ExceptionToResponse(ex);
            }
        }

        protected virtual async Task<HttpResponseMessage> ExecuteDeleteAsync(string url, object data, object query = null)
        {
            Client = new HttpClient();
            await BuildClientAsync(Client);
            Uri uri = GetUri(url, query);

            try
            {
                HttpResponseMessage mes = await Client.DeleteAsync(uri);
                return new HttpResponseMessage()
                {
                    RequestMessage = mes.RequestMessage,
                    StatusCode = mes.StatusCode,
                    Content = mes.Content
                };
            }
            catch (Exception ex)
            {
                return ExceptionToResponse(ex);
            }
        }

        protected virtual async Task<HttpResponseMessage> ExecutePostAsync(string url, object data, object query = null)
        {
            Client = new HttpClient();
            await BuildClientAsync(Client);
            Uri uri = GetUri(url, query);

            try
            {
                var st = data.ToJson();
                HttpResponseMessage mes = await Client.PostAsync(uri, new StringContent(st, Encoding.UTF8, "application/json"));
                return new HttpResponseMessage()
                {
                    RequestMessage = mes.RequestMessage,
                    StatusCode = mes.StatusCode,
                    Content = mes.Content
                };
            }
            catch (Exception ex)
            {
                return ExceptionToResponse(ex);
            }
        }

        public virtual JObject Get(string url, object query = null)
        {
            var tsk = GetAsync(url, query);
            tsk.Wait(2000);
            return tsk.Result;
        }

        public async Task<T> GetAsyncAs<T>(string url, object query = null) where T : class
        {
            var res = await ExecuteGetAsync(url, query);
            return await DeserializeAsync<T>(res);
        }

        private async Task<T> DeserializeAsync<T>(HttpResponseMessage res)
        {
            if (res.IsSuccessStatusCode)
            {
                string data = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data);
            }
            else
            {
                throw await ToExceptionAsync(res);
            }
        }

        private async Task<string> ToStringAsync(HttpResponseMessage res)
        {
            if (res.IsSuccessStatusCode)
            {
                string data = await res.Content.ReadAsStringAsync();
                return data;
            }
            else
            {
                throw await ToExceptionAsync(res);
            }
        }

        private async Task<JObject> ToJsonAsync(HttpResponseMessage res)
        {
            if (res.IsSuccessStatusCode)
            {
                string data = await res.Content.ReadAsStringAsync();
                return (JObject)JsonConvert.DeserializeObject(data);
            }
            else
            {
                throw await ToExceptionAsync(res);
            }
        }

        public async Task<JObject> GetAsync(string url, object query = null)
        {
            var res = await ExecuteGetAsync(url, query);
            return await ToJsonAsync(res);
        }

        public async Task<string> GetAsyncAsString(string url, object query = null)
        {
            var res = await ExecuteGetAsync(url, query);
            return await ToStringAsync(res);
        }

        public async Task<FileBytes> DownloadFileAsync(string url, object query = null)
        {
            HttpResponseMessage mes = await ExecuteGetAsync(url, query);
            if (mes.IsSuccessStatusCode)
            {

                var byts = await mes.Content.ReadAsByteArrayAsync();
                var name = url.GetAfterLast("/")?.GetBeforeFirst("?");

                FileBytes b = new FileBytes(name, byts);

                if (mes.Content.Headers.ContentDisposition != null)
                    b.SetFileName(mes.Content.Headers.ContentDisposition.FileName);

                if (mes.Content.Headers.ContentType != null)
                    b.SetMimeType(mes.Content.Headers.ContentType.MediaType);

                return b;
            }
            else
            {
                throw await ToExceptionAsync(mes);
            }
        }

        public FileBytes DownloadFile(string url, object query = null)
        {
            url = url.Replace("\\", "/");
            Task<FileBytes> obj = Task.Run(() => DownloadFileAsync(url, query));
            Task.WaitAll(obj);
            return obj.Result;
        }

        public async Task<HttpResponseMessage> UploadFilesAsync(string url, IEnumerable<FileData> files, object query = null)
        {
            Client = new HttpClient();
            Uri uri = GetUri(url, query);

            try
            {
                List<MemoryStream> strs = new List<MemoryStream>();
                using (var content = new MultipartFormDataContent())
                {
                    foreach (FileData fil in files)
                    {

                        byte[] byts = File.ReadAllBytes(fil.FullPath);
                        MemoryStream str = new MemoryStream(byts);
                        strs.Add(str);
                        StreamContent cont = new StreamContent(str);
                        cont.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "\"" + fil.FieldName + "\"",
                            FileName = "\"" + fil.FileName + "\""
                        };
                        content.Add(cont, fil.FieldName, fil.FileName);
                    }
                    HttpResponseMessage mes = await Client.PostAsync(uri, content);

                    mes.EnsureSuccessStatusCode();
                    return new HttpResponseMessage { StatusCode = mes.StatusCode, Content = mes.Content };
                }
            }
            catch (Exception ex)
            {

                return ExceptionToResponse(ex);
            }

        }

        protected HttpResponseMessage ExceptionToResponse(Exception ex)
        {
            HttpResponseMessage mes = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
            {
                Content = new StringContent(ex.Message)
            };

            return mes;
        }

        public async Task<JObject> PostAsync(string url, object data, object query = null)
        {
            var res = await ExecutePostAsync(url, data, query);
            return await ToJsonAsync(res);
        }

        public JObject Post(string url, object data, object query = null)
        {
            var t = PostAsync(url, data, query);
            t.Wait(2000);
            return t.Result;
        }
    }
}
