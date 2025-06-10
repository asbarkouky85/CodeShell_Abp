using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.Attachments.BlobStoring
{
    public interface IChunkUploader
    {
        string NormalizeName(string name);
        Task Finish(string name, string uploadId, Dictionary<int, string> partIds, CancellationToken cancellationToken = default);
        Task<string> StartFile(string name, CancellationToken cancellationToken = default);
        Task<string> UploadPart(string name, string uploadId, int partNumber, byte[] bytes, CancellationToken cancellationToken = default);
    }
}