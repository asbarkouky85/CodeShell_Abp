using Codeshell.Abp.Files;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Codeshell.Abp.Http
{
    public interface IHttpService
    {
        Dictionary<string, string> Headers { get; }

        FileBytes DownloadFile(string url, object query = null);
        JObject Get(string url, object query = null);
        JObject Post(string url, object data, object query = null);
        Task<FileBytes> DownloadFileAsync(string url, object query = null);
        Task<HttpResponseMessage> UploadFilesAsync(string url, IEnumerable<FileData> files, object query = null);
        Task<JObject> GetAsync(string url, object query = null);
        Task<JObject> PostAsync(string url, object data, object query = null);
        Task<HttpResponseMessage> GetResponseAsync(string url, object query = null);
        Task<T> GetAsyncAs<T>(string url, object query = null) where T : class;
        Task<string> GetAsyncAsString(string url, object query = null);
        Task<T> PostAsyncAs<T>(string url, object data, object query = null) where T : class;

    }
}