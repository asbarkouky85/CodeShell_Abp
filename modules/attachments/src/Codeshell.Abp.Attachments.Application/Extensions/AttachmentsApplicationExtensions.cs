using Codeshell.Abp;
using Codeshell.Abp.Attachments.Enums;
using Codeshell.Abp.Attachments.Storage;
using Volo.Abp.BlobStoring;
//using Volo.Abp.BlobStoring.Aws;

namespace Codeshell.Abp.Attachments
{
    public class TestFileConfig
    {
        public string BasePath { get; set; }
    }
    public static class AttachmentsApplicationExtensions
    {
        public static void UseConfigurationItem(this BlobContainerConfiguration container, BlobContainerConfigItem c)
        {
            switch (c.StorageType)
            {
                case BlobStorageTypes.FileSystem:
                    //FileSystemBlobProviderConfiguration
                    var cng = c.Config;

                    container.ProviderType = typeof(CustomFileSystemBlobProvider);
                    break;
                case BlobStorageTypes.Aws:
                    //container.UseAws(e =>
                    //{
                    //    e.AppendProperties(c.Config);
                    //});
                    break;
            }
        }

        public static IBlobContainer GetContainer(this IBlobContainerFactory factory, string name = null)
        {
            return factory.Create(name ?? "default");
        }

        public static IBlobContainer GetTempContainer(this IBlobContainerFactory factory)
        {
            return factory.Create("temp");
        }
    }
}
