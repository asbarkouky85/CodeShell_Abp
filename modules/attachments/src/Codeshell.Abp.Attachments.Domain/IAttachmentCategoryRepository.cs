using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Codeshell.Abp.Attachments
{
    public interface IAttachmentCategoryRepository : IRepository<AttachmentCategory, int>, ITransientDependency
    {
        public Task<bool> AnonymousDownloadAllowed(Guid attachmentId);
        public Task<bool> DownloadAllowed(string[] roles, Guid attachmentId);

        public Task<bool> AnonymousDownloadAllowed(int attachmentCategoryId);
        public Task<bool> DownloadAllowed(string[] roles, int attachmentCategoryId);
    }
}
