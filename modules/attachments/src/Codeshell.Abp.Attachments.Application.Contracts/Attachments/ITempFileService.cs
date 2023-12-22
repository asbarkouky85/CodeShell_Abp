using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.Attachments
{
    public interface ITempFileService : ITransientDependency
    {
        Task CleanUp(DateTime createdBefore);
    }
}