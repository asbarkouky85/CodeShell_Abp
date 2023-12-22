using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace SASO.Attachments
{
    [DependsOn(
        typeof(AttachmentsHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class AttachmentsConsoleApiClientModule : AbpModule
    {
        
    }
}
