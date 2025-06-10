using Codeshell.Abp.Attachments.BlobStoring;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Aws;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Attachments.BlobStoring
{
    public class ChunkUploaderFactory : IChunkUploaderFactory
    {
        protected IAbpLazyServiceProvider LazyServiceProvider { get; set; }
        IBlobContainerConfigurationProvider ConfigurationProvider => LazyServiceProvider.LazyGetRequiredService<IBlobContainerConfigurationProvider>();

        public ChunkUploaderFactory(IServiceProvider provider)
        {
            LazyServiceProvider = new AbpLazyServiceProvider(provider);
        }

        public virtual IChunkUploader Create(string containerName)
        {
            var conf = ConfigurationProvider.Get(containerName);
            if (conf.ProviderType.IsAssignableFrom(typeof(AwsBlobProvider)))
            {
                var uploader = LazyServiceProvider.LazyGetRequiredService<AwsChunkUploader>();
                uploader.SetUp(containerName, conf);
                return uploader;
            }
            else
            {
                return LazyServiceProvider.LazyGetRequiredService<FileSystemChunkUploader>();
            }
        }
    }
}
