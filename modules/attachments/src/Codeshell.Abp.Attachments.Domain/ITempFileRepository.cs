using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Codeshell.Abp.Attachments
{
    public interface ITempFileRepository : IRepository<TempFile,Guid>
    {
        Task<List<TempFile>> GetCleanUpFiles(DateTime createdBefore, int v);
        Task<TempFile> GetWithCategory(Guid id);
        Task<TempFile> GetWithChunks(Guid id);
        Task<bool> IsChunksComplete(Guid id);
    }
}
