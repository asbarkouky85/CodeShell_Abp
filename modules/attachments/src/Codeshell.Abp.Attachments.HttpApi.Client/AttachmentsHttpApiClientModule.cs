using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.Attachments
{
    [DependsOn(
        typeof(AttachmentsApplicationContractsModule),
        typeof(AbpHttpClientModule))]
    public class AttachmentsHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "Attachments";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(AttachmentsApplicationContractsModule).Assembly,
                RemoteServiceName
            );
        }
    }
}
