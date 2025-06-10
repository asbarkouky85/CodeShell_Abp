using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.BlobStoring;

namespace Codeshell.Abp.Attachments.Extensions
{
    public static class AttachmentsSharedExtensions
    {
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
