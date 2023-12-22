using Codeshell.Abp.Attachments.Jobs;
using Codeshell.Abp.Attachments.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.Attachments
{
    [DependsOn(
        typeof(AttachmentsDomainModule),
        typeof(AttachmentsApplicationContractsModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpBlobStoringModule),
        typeof(AbpBlobStoringFileSystemModule),
        //typeof(AbpBlobStoringAwsModule),
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

            Configure<AbpBlobStoringOptions>(options =>
            {
                var con = context.Services.GetConfiguration();
                var conf = con.GetSection("BlobContainers").Get<BlobContainerConfigItem[]>();
                foreach (var c in conf)
                {
                    if (c.Name == "default")
                    {
                        options.Containers.ConfigureDefault(container => container.UseConfigurationItem(c));
                    }
                    else
                    {
                        options.Containers.Configure(c.Name, container => container.UseConfigurationItem(c));
                    }

                }
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
