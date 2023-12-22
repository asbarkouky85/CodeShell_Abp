using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Codeshell.Abp.Attachments.Attachments
{
    public interface IInternalAttachmentAppService
    {
        Task SaveAttachmentsByPhysicalPath(Dictionary<Guid, string> files,int attachmentTypeId);
    }
}
