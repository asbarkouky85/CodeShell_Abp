using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.Attachments
{
    [DependsOn(
        typeof(AttachmentsDomainModule),
        typeof(AttachmentsApplicationContractsModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpBackgroundWorkersModule),
        typeof(CodeshellApplicationModule)
        )]

    public class AttachmentsApplicationModule : AbpModule
    {
        public override void PostConfigureServices(ServiceConfigurationContext context)
        {

        }
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //context.Services.AddCustomEndpointAmazonS3ClientFactory();
            context.Services.AddAutoMapperObjectMapper<AttachmentsApplicationModule>();

            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<AttachmentsApplicationModule>(validate: false);
            });

            Configure<AbpBackgroundWorkerOptions>(conf =>
            {
                conf.IsEnabled = true;
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            //context.AddBackgroundWorker<TempFileCleanUpJob>();
        }
    }
}
