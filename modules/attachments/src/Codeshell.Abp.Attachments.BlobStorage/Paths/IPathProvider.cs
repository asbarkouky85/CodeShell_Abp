using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.Attachments.Paths
{
    public interface IPathProvider : ITransientDependency
    {
        Task<string> GetTempFolderPath();
        Task<string> GetRootFolderPath();

    }
}
