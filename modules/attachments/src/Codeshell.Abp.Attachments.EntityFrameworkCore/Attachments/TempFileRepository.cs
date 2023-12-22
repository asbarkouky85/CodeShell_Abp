using Codeshell.Abp.Attachments.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Codeshell.Abp.Attachments
{
    public class TempFileRepository : EfCoreRepository<AttachmentsDbContext, TempFile, Guid>, ITempFileRepository
    {
        public TempFileRepository(IDbContextProvider<AttachmentsDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<TempFile>> GetFilesBefore(DateTime createdBefore)
        {
            return (await GetDbSetAsync()).Where(e => e.CreationTime < createdBefore).ToList();
        }
    }
}
