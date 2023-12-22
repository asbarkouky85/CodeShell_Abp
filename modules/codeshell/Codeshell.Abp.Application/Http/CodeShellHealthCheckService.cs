using Codeshell.Abp.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.Http
{
    public class CodeShellHealthCheckService : ApplicationService, ITransientDependency, ICodeShellHealthCheckService
    {
        private readonly IConfiguration config;

        public CodeShellHealthCheckService(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<ServiceInfoDto> CheckRemote()
        {

            var sec = config.GetSection("RemoteServices:Default:BaseUrl");
            var res = new ServiceInfoDto();
            if (sec.Value != null)
            {
                var service = new HttpService();
                service.BaseUrl = sec.Value + "/Home";
                try
                {

                    var info = await service.GetAsyncAs<ServiceInfoDto>("Info");
                    res.RemoteServiceState = "Connected";
                    res.Version = info.Version;
                    res.ServiceName = info.ServiceName;
                }
                catch (HttpServiceException servEx)
                {
                    res.RemoteServiceState = servEx.Message + " - " + servEx.ServiceResponse + " - " + servEx.GetMessageRecursive();
                }
                catch (Exception ex)
                {
                    res.RemoteServiceState = ex.GetMessageRecursive();
                }
            }

            res.RemoteServiceUrl = sec.Value;
            return res;
        }

        protected Task<ServiceInfoDto> GetCheckTask(ServiceInfoStateDto info)
        {
            var service = new HttpService();
            service.BaseUrl = info.Url + "/Home";
            var startTime = DateTime.Now;
            var task = service.GetAsyncAs<ServiceInfoDto>("Info");

            task.GetAwaiter().OnCompleted(() =>
            {
                var span = DateTime.Now - startTime;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    info.ServiceName = task.Result.ServiceName + " [" + info.ServiceName + "]";
                    info.Version = task.Result.Version;
                }
                else
                {
                    info.Version = "FAILED";
                }
                info.Elapsed = span.ToString();
            });
            return task;

        }

        public virtual ServiceInfoDto Info<TModule>()
        {
            return new ServiceInfoDto
            {
                ServiceName = typeof(TModule).Assembly.GetName().Name,
                Version = typeof(TModule).Assembly.GetVersionString()
            };
        }

        public async Task<string> ServicesInfo()
        {
            var config = LazyServiceProvider
                .LazyGetService<IConfiguration>()
                .GetSection("ReRoutes")
                .Get<ReRouteItem[]>();

            List<ServiceInfoStateDto> serviceInfoList = new List<ServiceInfoStateDto>();
            List<Task> tasks = new List<Task>();

            foreach (var route in config)
            {
                var info = new ServiceInfoStateDto(route);
                if (info.Url != null)
                {
                    var task = GetCheckTask(info);

                    serviceInfoList.Add(info);
                    tasks.Add(task);
                }
            }

            try
            {
                await Task.WhenAll(tasks.ToArray());
            }
            catch { }

            var html = WriteHtml(serviceInfoList);
            return html;
        }

        protected string WriteHtml(List<ServiceInfoStateDto> serviceInfoList)
        {
            var thisServiceInfo = Assembly.GetEntryAssembly().GetName().Name;
            serviceInfoList = serviceInfoList.OrderBy(e => e.Url).ToList();
            var res = "<html>" +
                $"<head><title>{thisServiceInfo}</title></head>" +
                "<body>" +
                "<style>td,th{padding:5px}</style>" +
                "<table border='1' style='border-collapse:collapse'>" +
                $"<tr>" +
                $"<th>Url</th>" +
                $"<th>Service</th>" +
                $"<th>Version</th>" +
                $"<th>Time</th>" +
                $"</tr>";

            foreach (var inf in serviceInfoList)
            {
                var style = inf.Version == "FAILED" ? "style='color:red'" : "";
                res += $"<tr {style}>" +
                    $"<td><a href='{inf.Url}' target='_blank'>{inf.Url}</a></td>" +
                    $"<td>{inf.ServiceName}</td>" +
                    $"<th>{inf.Version}</th>" +
                    $"<th>{inf.Elapsed}</th>" +
                    $"</tr>";
            }
            res += "</table>" +
                "</body>" +
                "</html>";
            return res;
        }
    }
}
