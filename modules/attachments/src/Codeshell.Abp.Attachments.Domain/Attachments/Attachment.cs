using Codeshell.Abp.Files;
using System;
using System.IO;
using Volo.Abp.Domain.Entities.Auditing;

#nullable disable

namespace Codeshell.Abp.Attachments
{
    public partial class Attachment : FullAuditedEntity<Guid>
    {
        public string FileName { get; protected set; }
        public string FullPath { get; protected set; }
        public int AttachmentCategoryId { get; protected set; }
        public Guid? BinaryAttachmentId { get; protected set; }
        public string Extension { get; protected set; }
        public string ContentType { get; protected set; }
        public string ContainerName { get; protected set; }

        public virtual AttachmentCategory AttachmentCategory { get; set; }

        public virtual AttachmentBinary BinaryAttachment { get; set; }

        public Attachment()
        {

        }

        public Attachment(Guid id, string fileName, int attachmentTypeId, string containerName = null) : base(id)
        {
            FileName = fileName;
            AttachmentCategoryId = attachmentTypeId;
            IsDeleted = false;
            Extension = Path.GetExtension(FileName);
            ContentType = MimeData.GetFileMimeType(FileName);
            ContainerName = containerName;
        }

        public void SetBlobName(string blobName)
        {
            FullPath = blobName;
        }
    }
}
