using Volo.Abp.Modularity;

namespace Codeshell.Abp.Notifications
{
    [DependsOn(
        typeof(CodeshellDomainModule)
        )]
    public class CodeshellNotificationsDomainModule : AbpModule
    {

    }
}
