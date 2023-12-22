using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Codeshell.Abp
{
    public class CodeshellDomainSharedModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddScoped<CurrentCulture>();
            context.Services.AddSingleton<IScopedProviderAccessor, DefaultScopedProviderAccessor>();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            CodeshellRoot.RootProvider = context.ServiceProvider;
            base.OnApplicationInitialization(context);
        }
    }
}
