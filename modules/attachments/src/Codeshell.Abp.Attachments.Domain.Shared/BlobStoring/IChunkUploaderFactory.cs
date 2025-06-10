using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.Attachments.BlobStoring
{
    public interface IChunkUploaderFactory : ITransientDependency
    {
        IChunkUploader Create(string containerName);
    }
}
