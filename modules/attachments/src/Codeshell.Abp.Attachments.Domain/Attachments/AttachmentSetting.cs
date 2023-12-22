using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

#nullable disable

namespace Codeshell.Abp.Attachments
{
    public partial class AttachmentSetting : AuditedEntity<Guid>
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
