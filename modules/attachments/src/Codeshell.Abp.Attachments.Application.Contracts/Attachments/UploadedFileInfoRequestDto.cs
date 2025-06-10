using System;
using System.Collections;
using System.Collections.Generic;

namespace Codeshell.Abp.Attachments
{
    public class UploadedFileInfoRequestDto
    {
        public IEnumerable<Guid> Ids { get; set; }
    }
}