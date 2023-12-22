using Codeshell.Abp.Files;
using System;
using System.IO;
using Volo.Abp.Domain.Entities.Auditing;

namespace Codeshell.Abp.Attachments
{
    public class TempFile : AuditedEntity<Guid>
    {
        protected TempFile() { }
        public TempFile(Guid id, string fileName)
        {
            Id = id;
            FileName = fileName;
            Extension = Path.GetExtension(FileName);
            ContentType = MimeData.GetFileMimeType(FileName);
        }

        public string FileName { get; protected set; }
        public string FullPath { get; protected set; }
        public string Extension { get; protected set; }
        public string ContentType { get; protected set; }

        public string GetBlobName()
        {
            return "_tmp/" + Id + Extension;
        }
    }
}
