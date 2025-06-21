using System;
using System.Collections;
using System.Collections.Generic;
using Codeshell.Abp.Attachments;

namespace Codeshell.Abp.Attachments.Attachments
{
    public class UploadedFileInfoRequestDto
    {
        public IEnumerable<Guid> Ids { get; set; }
    }
}
