using Codeshell.Abp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Codeshell.Abp.Attachments
{
    public interface IAttachmentRepository : ICodeshellRepository<Attachment, Guid>
    {
        Task<List<UploadedFileInfo>> GetInfo(IEnumerable<Guid> id);
        Task<UploadedFileInfo> GetInfo(Guid id);
    }
}
