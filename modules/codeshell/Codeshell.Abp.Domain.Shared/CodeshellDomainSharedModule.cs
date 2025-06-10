using Codeshell.Abp.MultiTenancy;
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

            context.Services.AddOptions<CodeshellMultiTenancyOptions>();
            context.Services.Configure<CodeshellMultiTenancyOptions>(context.Services.GetConfiguration().GetSection("Codeshell:MultiTenancy"));

        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            CodeshellRoot.RootProvider = context.ServiceProvider;

            base.OnApplicationInitialization(context);
        }
    }
}
