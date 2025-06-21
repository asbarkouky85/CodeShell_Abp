using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Codeshell.Abp.Attachments;

namespace Codeshell.Abp.Attachments.Attachments
{
    public interface ITempFileService : ITransientDependency
    {
        Task CleanUp(DateTime createdBefore);
    }
}
