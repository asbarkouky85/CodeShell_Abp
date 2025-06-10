using Volo.Abp.Modularity;
using Codeshell.Abp;

namespace Codeshell.Abp.Notifications
{
    [DependsOn(
        typeof(CodeshellDomainSharedModule)
        )]
    public class CodeshellNotificationsDomainSharedModule : AbpModule
    {
    }
}