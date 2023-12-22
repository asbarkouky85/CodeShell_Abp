using Codeshell.Abp.Attachments.Localization;
using Volo.Abp.AuditLogging;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Codeshell.Abp.Attachments
{
    [DependsOn(
        typeof(AbpValidationModule),
        typeof(CodeshellDomainSharedModule),
        typeof(AbpAuditLoggingDomainSharedModule)
    )]
    public class AttachmentsDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<AttachmentsDomainSharedModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<AttachmentsResource>("ar")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/Localization/Attachments");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("Attachments", typeof(AttachmentsResource));
            });
        }
    }
}
