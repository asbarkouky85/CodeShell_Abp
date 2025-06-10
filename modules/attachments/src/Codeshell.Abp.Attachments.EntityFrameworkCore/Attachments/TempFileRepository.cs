using Microsoft.EntityFrameworkCore;
using Codeshell.Abp.Attachments.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Codeshell.Abp.Attachments.Attachments
{
    public class TempFileRepository : EfCoreRepository<AttachmentsDbContext, TempFile, Guid>, ITempFileRepository
    {
        public TempFileRepository(IDbContextProvider<AttachmentsDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<TempFile>> GetCleanUpFiles(DateTime createdBefore, int maxResultCount)
        {
            var q = await GetQueryableAsync();
            q = q.Include(e => e.AttachmentCategory);
            return await q.Where(e => e.CreationTime < createdBefore || e.FullPath == null).Take(maxResultCount).ToListAsync();
        }

        public async Task<TempFile> GetWithCategory(Guid id)
        {
            var q = await GetQueryableAsync();
            q = q.Include(e => e.AttachmentCategory);
            return await q.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<TempFile> GetWithChunks(Guid id)
        {
            var q = await GetQueryableAsync();
            q = q.AsNoTracking().Include(e => e.Chunks);
            return await q.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> IsChunksComplete(Guid id)
        {
            var q = await GetQueryableAsync();
            return await q.Where(e => e.Id == id).Select(e => e.TotalChunkCount == e.Chunks.Count).FirstOrDefaultAsync();
        }
    }
}
