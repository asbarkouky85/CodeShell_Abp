using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeshell.Abp.Services
{
    public interface ISqlCommandCapturere<TDbContext> where TDbContext : DbContext, ICaptureSqlDbContext
    {
        Task<List<string>> CaptureCommandsAsync(Func<TDbContext, Task> action);
    }
}