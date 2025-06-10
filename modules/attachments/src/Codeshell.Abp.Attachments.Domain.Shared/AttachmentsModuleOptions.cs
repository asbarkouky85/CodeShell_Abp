using Codeshell.Abp.Attachments.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Attachments
{
    public class AttachmentsModuleOptions
    {
        public bool Authorize { get; set; } = true;
        public bool Mock { get; set; }
        public BlobContainerConfigItem Default { get; set; } = new BlobContainerConfigItem
        {
            Name = "default",
            StorageType = Enums.BlobStorageTypes.FileSystem
        };

        public List<BlobContainerConfigItem> BlobContainers { get; private set; } = new List<BlobContainerConfigItem>();
    }
}
