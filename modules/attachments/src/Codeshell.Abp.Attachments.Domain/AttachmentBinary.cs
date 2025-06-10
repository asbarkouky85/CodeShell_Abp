using System;
using Volo.Abp.Domain.Entities;

#nullable disable

namespace Codeshell.Abp.Attachments
{
    public partial class AttachmentBinary : Entity<Guid>
    {
        public byte[] Bytes { get; private set; }

        public virtual Attachment Attachment { get; set; }

        protected AttachmentBinary() { }

        public AttachmentBinary(Guid id, byte[] bytes) : base(id)
        {
            Bytes = bytes;
        }
    }
}
