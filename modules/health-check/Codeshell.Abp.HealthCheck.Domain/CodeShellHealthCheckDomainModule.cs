using Volo.Abp.Modularity;

namespace Codeshell.Abp.HealthCheck
{
    [DependsOn(
        typeof(CodeshellDomainModule)
        )]
    public class CodeShellHealthCheckDomainModule : AbpModule
    {
    }
}