using SASO.Attachments.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace SASO.Attachments
{
    /* Domain tests are configured to use the EF Core provider.
     * You can switch to MongoDB, however your domain tests should be
     * database independent anyway.
     */
    [DependsOn(
        //typeof(AttachmentsEntityFrameworkCoreTestModule)
        )]
    public class AttachmentsDomainTestModule : AbpModule
    {
        
    }
}
