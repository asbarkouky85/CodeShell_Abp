using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Codeshell.Abp.Attachments.Paths
{
    public class NullAttachmentPathProvider : IPathProvider
    {
        public Task<string> GetRootFolderPath()
        {
            return Task.FromResult("c:/_attachments");
        }

        public Task<string> GetTempFolderPath()
        {
            return Task.FromResult("c:/_attachments/temp");
        }
    }
}
