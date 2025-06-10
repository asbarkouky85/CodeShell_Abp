using Newtonsoft.Json.Linq;
using Codeshell.Abp.Attachments.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Attachments.Storage
{
    public class BlobContainerConfigItem
    {
        public string Name { get; set; }
        public BlobStorageTypes StorageType { get; set; }
        public Dictionary<string, string> Config { get; set; }
    }
}
