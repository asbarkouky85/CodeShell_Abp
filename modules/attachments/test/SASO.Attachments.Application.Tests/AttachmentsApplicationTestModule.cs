using Volo.Abp.Modularity;

namespace SASO.Attachments
{
    [DependsOn(
        typeof(AttachmentsApplicationModule),
        typeof(AttachmentsDomainTestModule)
        )]
    public class AttachmentsApplicationTestModule : AbpModule
    {

    }
}
