using Volo.Abp.AuditLogging;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.Attachments
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AttachmentsDomainSharedModule),
        typeof(AbpAuditLoggingDomainModule)
    )]
    public class AttachmentsDomainModule : AbpModule
    {

    }
}
