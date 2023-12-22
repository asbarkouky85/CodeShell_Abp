using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Codeshell.Abp.Attachments.Attachments
{
    public interface IAttachmentRepository : IRepository<Attachment, Guid>, ITransientDependency
    {
        Task<string> GetFileName(Guid id);
        Task<AttachmentBinary> GetBinaryAttachment(Guid id);
    }
}
