using Codeshell.Abp.Attachments.Attachments;
using Codeshell.Abp.Attachments.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Codeshell.Abp.Attachments
{
    public class AttachmentRepository : EfCoreRepository<AttachmentsDbContext, Attachment, Guid>, IAttachmentRepository
    {
        public AttachmentRepository(IDbContextProvider<AttachmentsDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<AttachmentBinary> GetBinaryAttachment(Guid id)
        {
            return (await GetDbSetAsync()).Where(e => e.Id == id).Select(e => e.BinaryAttachment).FirstOrDefault();
        }

        public async Task<string> GetFileName(Guid id)
        {
            return (await GetDbSetAsync()).Where(w => w.Id == id).Select(e => e.FileName).FirstOrDefault();
        }
    }
}
