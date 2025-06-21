using Codeshell.Abp.Files;
using Codeshell.Abp.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Codeshell.Abp.Attachments;

namespace Codeshell.Abp.Attachments.Attachments
{
    public class FileValidationRequest : IFileInfo
    {
        public long? Size { get; set; }
        public string Extension { get; set; }
        public int AttachmentType { get; set; }
        public FileDimesion Dimesion { get; set; }
        public string FileName { get; set; }
    }
}
