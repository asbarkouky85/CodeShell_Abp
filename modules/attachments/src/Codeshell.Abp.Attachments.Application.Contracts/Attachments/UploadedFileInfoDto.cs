using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Codeshell.Abp.Attachments;

namespace Codeshell.Abp.Attachments.Attachments
{
    public class UploadedFileInfoDto : EntityDto<Guid>
    {
        public string FileName { get; set; }
        public int? Size { get; set; }
    }
}
