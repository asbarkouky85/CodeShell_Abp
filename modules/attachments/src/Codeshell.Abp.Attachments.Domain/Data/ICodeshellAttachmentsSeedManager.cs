using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;

namespace Codeshell.Abp.Attachments.Data
{
    public interface ICodeshellAttachmentsSeedManager : IDomainService, ITransientDependency
    {
        AttachmentCategory AddCategoryByEnum<T>(T categoryId, string allowedExtensions, int maxSize, string folderPath = null, int? maxCount = null, string container = null, Dimension dimension = null, bool anonymousDownload = false) where T : struct;
        void PreventDownloadPermissionByEnum<T>(T category, params object[] roles) where T : struct;
        Task SaveAsync();
        void SetUploadPermissionByEnum<T>(T category, params object[] roles);
    }
}