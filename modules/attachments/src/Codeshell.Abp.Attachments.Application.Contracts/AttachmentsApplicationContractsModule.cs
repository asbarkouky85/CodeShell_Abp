using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace Codeshell.Abp.Attachments
{
    [DependsOn(
        typeof(AttachmentsDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationAbstractionsModule)
        )]
    public class AttachmentsApplicationContractsModule : AbpModule
    {

    }
}
