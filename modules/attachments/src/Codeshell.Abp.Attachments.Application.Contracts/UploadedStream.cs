using Codeshell.Abp.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Codeshell.Abp.Attachments
{
    public class UploadedStream : IFileInfo
    {
        public string Id { get; private set; }
        public string FileName { get; protected set; }
        public string MimeType { get; private set; }
        public string Extension { get; protected set; }
        public long? Size => Stream?.Length;
        public FileDimesion Dimesion { get; protected set; }
        public Stream Stream { get; protected set; }

        protected UploadedStream()
        {

        }

        public UploadedStream(string name, Stream stream, string id = null) : this()
        {
            FileName = name;
            Stream = stream;
            Extension = Path.GetExtension(name);
            MimeType = MimeData.GetFileMimeType(name);
            Id = id;
        }
    }
}
