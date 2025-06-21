using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.HealthCheck.Application
{
    [DependsOn(
        typeof(CodeshellApplicationModule)
        )]
    public class FmsHealthCheckerModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            context.Services.AddOptions<CodeshellHealthCheckerOptions>();
            context.Services.Configure<CodeshellHealthCheckerOptions>(e =>
            {
                configuration.GetSection("HealthChecker").Bind(e);
            });

            context.Services.AddHostedService<CodeshellHealthCheckHostedService>();
        }
    }
}
