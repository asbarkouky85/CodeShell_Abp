using System;

namespace Codeshell.Abp.Attachments
{
    public interface IHasOptionalAttachmentEntity
    {
        Guid? AttachmentId { get; set; }
    }
}
