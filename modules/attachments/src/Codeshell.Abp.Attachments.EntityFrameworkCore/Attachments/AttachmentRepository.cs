using Microsoft.EntityFrameworkCore;
using Codeshell.Abp.Attachments.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Expressions;
using Codeshell.Abp.Repositories;

namespace Codeshell.Abp.Attachments.Attachments
{
    public class AttachmentRepository : CodeshellEfCoreRepository<AttachmentsDbContext, Attachment, Guid>, IAttachmentRepository
    {
        public AttachmentRepository(IDbContextProvider<AttachmentsDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        async Task<IQueryable<UploadedFileInfo>> _queryInfo()
        {
            var q = await GetQueryableAsync();
            return q.Select(e => new UploadedFileInfo
            {
                Id = e.Id,
                FileName = e.FileName,
                Size = e.Size
            });
        }

        public async Task<List<UploadedFileInfo>> GetInfo(IEnumerable<Guid> id)
        {
            var q = await _queryInfo();
            return await q.Where(e => id.Contains(e.Id)).ToListAsync();
        }

        public async Task<UploadedFileInfo> GetInfo(Guid id)
        {
            var q = await _queryInfo();
            return await q.Where(e => e.Id == id).FirstOrDefaultAsync();
        }
    }
}
