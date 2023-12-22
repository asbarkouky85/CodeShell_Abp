using Codeshell.Abp.Files;
using Codeshell.Abp.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Attachments
{
    public class UploadRequestDto
    {
        public int AttachmentTypeId { get; set; }
        public IList<FileBytes> Files { get; set; }
        

    }
}
