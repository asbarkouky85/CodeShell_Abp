using Codeshell.Abp;
using Codeshell.Abp.Attachments.Enums;
using Codeshell.Abp.Attachments.Storage;
using Codeshell.Abp.Extensions;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Aws;

namespace Codeshell.Abp.Attachments
{
    public static class AttachmentsApplicationExtensions
    {
        public static void UseConfigurationItem(this BlobContainerConfiguration container, BlobContainerConfigItem c)
        {
            switch (c.StorageType)
            {
                case BlobStorageTypes.FileSystem:
                    var cng = c.Config;

                    container.ProviderType = typeof(CustomFileSystemBlobProvider);
                    break;
                case BlobStorageTypes.Aws:
                    container.UseAws(e =>
                    {
                        e.AppendProperties(c.Config);
                    });
                    break;
            }
        }

        
    }
}
