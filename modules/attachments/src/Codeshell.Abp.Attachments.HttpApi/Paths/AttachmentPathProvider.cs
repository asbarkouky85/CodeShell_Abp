using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Settings;

namespace Codeshell.Abp.Attachments.Paths
{
    public class AttachmentPathProvider : IPathProvider
    {
        private readonly ISettingProvider setting;
        private string _mainFolder;
        public AttachmentPathProvider(ISettingProvider setting)
        {
            this.setting = setting;
        }

        public async Task<string> GetTempFolderPath()
        {
            var root = await GetRootFolderPath();
            return Path.Combine(root, "_temp");
        }

        public async Task<string> GetRootFolderPath()
        {
            if (string.IsNullOrEmpty(_mainFolder))
                _mainFolder = await setting.GetOrNullAsync(AttachmentConst.DriveRootPath);
            return _mainFolder;
        }
    }
}
