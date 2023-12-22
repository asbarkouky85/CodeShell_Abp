using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.Http
{
    public interface ICodeShellHealthCheckService : IApplicationService, ITransientDependency
    {
        Task<ServiceInfoDto> CheckRemote();
        Task<string> ServicesInfo();
        ServiceInfoDto Info<TModule>();
    }
}