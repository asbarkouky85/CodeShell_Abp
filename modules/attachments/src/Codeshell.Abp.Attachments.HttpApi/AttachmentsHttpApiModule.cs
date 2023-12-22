using Localization.Resources.AbpUi;
using Codeshell.Abp;
using Microsoft.Extensions.DependencyInjection;
using Codeshell.Abp.Attachments.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.Attachments
{
    [DependsOn(
        typeof(AttachmentsApplicationContractsModule),
        typeof(CodeShellHttpModule),
        typeof(AbpAspNetCoreMvcModule))]
    public class AttachmentsHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(AttachmentsHttpApiModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<AttachmentsResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });
        }
    }
}
