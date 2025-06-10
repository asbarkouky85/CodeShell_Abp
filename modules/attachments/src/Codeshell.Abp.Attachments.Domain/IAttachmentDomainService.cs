using Codeshell.Abp.Files;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;

namespace Codeshell.Abp.Attachments
{
    public interface IAttachmentDomainService : IDomainService, ITransientDependency
    {
        Task<Attachment> SaveTemp(TempFile tmpFile);
        Task<Attachment> SaveBytes(int attachmentTypeId, FileBytes tmpBytes, Guid? id = null);
        Task<AttachmentExportResult> ExportAttachments(IEnumerable<Guid> attachmentIds, string outputPath);
        Task<FileBytes> GetFile(Attachment attachment);
        Task<List<Attachment>> ImportAttachments(byte[] archive);
    }
}