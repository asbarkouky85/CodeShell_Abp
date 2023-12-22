using Codeshell.Abp.Attachments.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Codeshell.Abp.Attachments.Attachments
{
    public class AttachmentCategoryRepository : EfCoreRepository<AttachmentsDbContext, AttachmentCategory, int>, ITransientDependency, IAttachmentCategoryRepository
    {
        public AttachmentCategoryRepository(IDbContextProvider<AttachmentsDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<bool> AnonymousDownloadAllowed(Guid attachmentId)
        {
            var res = (await GetDbContextAsync()).Attachments
                .Where(e => e.Id == attachmentId)
                .Select(e => e.AttachmentCategory.AnonymousDownload || e.AttachmentCategory.AttachmentCategoryPermissions.Any(e => e.Role == "Anonymous" && e.Download))
            .FirstOrDefault();
            return res;
        }

        public async Task<bool> DownloadAllowed(string[] roles, Guid attachmentId)
        {
            if (roles.Length == 0)
                return false;
            var res = (await GetDbContextAsync()).Attachments
                .Where(e => e.Id == attachmentId)
                .Select(e => e.AttachmentCategory.AnonymousDownload || !e.AttachmentCategory.AttachmentCategoryPermissions.Any(e => e.Role == roles[0] && !e.Download))
            .FirstOrDefault();
            return res;
        }
    }
}
