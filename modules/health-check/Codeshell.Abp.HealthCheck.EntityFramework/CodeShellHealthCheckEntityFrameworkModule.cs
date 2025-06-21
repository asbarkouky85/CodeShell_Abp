using Volo.Abp.Modularity;

namespace Codeshell.Abp.HealthCheck
{
    [DependsOn(
        typeof(CodeshellEntityFrameworkCoreModule),
        typeof(CodeShellHealthCheckDomainModule)
        )]
    public class CodeShellHealthCheckEntityFrameworkModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {


        }
    }
}