using Codeshell.Abp;
using Codeshell.Abp.Attachments.BlobStoring;
using Codeshell.Abp.Attachments.Paths;
using Codeshell.Abp.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.Attachments.BlobStoring
{
    public class FileSystemChunkUploader : IChunkUploader, ITransientDependency
    {
        readonly IPathProvider _paths;

        public FileSystemChunkUploader(IPathProvider pathProvider)
        {
            _paths = pathProvider;
        }

        public async Task Finish(string name, string uploadId, Dictionary<int, string> partIds, CancellationToken cancellationToken = default)
        {
            var chunks = partIds.OrderBy(e => e.Key).Select(e => e.Value).ToList();
            var root = await _paths.GetRootFolderPath();
            var filePath = Path.Combine(root, name);
            Utils.CreateFolderForFile(filePath);
            File.Create(filePath).Close();
            using (var stream = File.Open(filePath, FileMode.Append, FileAccess.Write))
            {
                foreach (var chunk in chunks)
                {
                    var bytes = File.ReadAllBytes(Path.Combine(root, chunk));
                    foreach (var b in bytes)
                    {
                        stream.WriteByte(b);
                    }
                }
            }
        }

        public string NormalizeName(string name)
        {
            return name;
        }

        public async Task<string> StartFile(string name, CancellationToken cancellationToken = default)
        {
            var uploadId = name.GetBeforeLast(".");
            var root = await _paths.GetRootFolderPath();
            var savePath = Path.Combine(root, uploadId).Replace("\\", "/");
            Directory.CreateDirectory(savePath);
            return uploadId;
        }

        public async Task<string> UploadPart(string name, string uploadId, int partNumber, byte[] bytes, CancellationToken cancellationToken = default)
        {
            var root = await _paths.GetRootFolderPath();
            var chunkId = Utils.CombineUrl(uploadId, partNumber.ToString());
            var savePath = Path.Combine(root, chunkId);
            Utils.CreateFolderForFile(savePath);
            File.WriteAllBytes(savePath, bytes);
            return chunkId;
        }
    }
}
