using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Codeshell.Abp.Attachments
{
    public class UploadedFileInfoDto : EntityDto<Guid>
    {
        public string FileName { get; set; }
        public int? Size { get; set; }
    }
}
