using Codeshell.Abp.Attachments.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Aws;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Modularity;

namespace Codeshell.Abp.Attachments.BlobStorage
{

    [DependsOn(
        typeof(AbpBlobStoringModule),
        typeof(AbpBlobStoringFileSystemModule),
        typeof(AbpBlobStoringAwsModule)
        )]
    public class AttachmentsBlobStorageModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBlobStoringOptions>(options =>
            {
                var con = context.Services.GetConfiguration();
                var conf = con.GetSection("Attachments").Get<AttachmentsModuleOptions>() ?? new AttachmentsModuleOptions();
                conf.Default = conf.Default ?? new BlobContainerConfigItem
                {
                    Name = "default",
                    StorageType = Enums.BlobStorageTypes.FileSystem
                };
                options.Containers.ConfigureDefault(container => container.UseConfigurationItem(conf.Default));

                if (conf.BlobContainers != null && conf.BlobContainers.Any())
                {
                    foreach (var c in conf.BlobContainers)
                    {
                        options.Containers.Configure(c.Name, container => container.UseConfigurationItem(c));
                    }
                }

            });
        }
    }
}
