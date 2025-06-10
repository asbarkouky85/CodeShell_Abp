using Codeshell.Abp;
using Codeshell.Abp.Attachments.Paths;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.Attachments.Storage
{
    public class CustomFileSystemBlobProvider : IBlobProvider, ITransientDependency
    {
        private readonly IPathProvider _paths;

        public CustomFileSystemBlobProvider(IPathProvider path)
        {
            _paths = path;

        }
        public async Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
        {
            var root = await _paths.GetRootFolderPath();
            var path = Path.Combine(root, args.BlobName);
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }

        public async Task<bool> ExistsAsync(BlobProviderExistsArgs args)
        {
            var root = await _paths.GetRootFolderPath();
            var path = Path.Combine(root, args.BlobName);
            return File.Exists(path);
        }

        public async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            var root = await _paths.GetRootFolderPath();
            var path = Path.Combine(root, args.BlobName);
            if (File.Exists(path))
            {
                return File.OpenRead(path);
            }
            return null;
        }

        public async Task SaveAsync(BlobProviderSaveArgs args)
        {
            var root = await _paths.GetRootFolderPath();
            var savePath = Path.Combine(root, args.BlobName).Replace("\\", "/");

            Utils.CreateFolderForFile(savePath);
            using (var s = File.Create(savePath))
            {

                byte[] buffer = new byte[8 * 1024];
                int len;
                while ((len = args.BlobStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    s.Write(buffer, 0, len);
                }
            }
        }
    }
}
