﻿using Codeshell.Abp.Files.Uploads;

namespace Codeshell.Abp.Attachments
{
    public class SaveAttachmentRequest : TempFileDto
    {
        public string ContainerName { get; set; }
        public string Folder { get; set; }
    }
}
