using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Codeshell.Abp.Attachments
{
    public interface ITempFileRepository : IRepository<TempFile, Guid>, ITransientDependency
    {
        Task<List<TempFile>> GetFilesBefore(DateTime createdBefore);
    }
}
