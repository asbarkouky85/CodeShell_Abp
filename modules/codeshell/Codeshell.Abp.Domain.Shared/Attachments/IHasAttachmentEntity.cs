using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Attachments
{

    public interface IHasAttachmentEntity
    {
        Guid AttachmentId { get; set; }
        string FileName { get; set; }
    }
}
