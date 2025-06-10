using System;

namespace Codeshell.Abp.Attachments
{
    public class UploadedFileInfo
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public long? Size { get; set; }
    }
}
