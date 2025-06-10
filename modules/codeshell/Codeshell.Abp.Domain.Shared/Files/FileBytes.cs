using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Codeshell.Abp.Files
{
    public class FileBytes : IFileInfo
    {
        public string Id { get; private set; }
        public string FileName { get; private set; }
        public string MimeType { get; private set; }
        public byte[] Bytes { get; private set; }
        public string Extension { get; private set; }
        public long? Size => Bytes?.Length;
        public FileDimesion Dimesion { get; set; }

        public void Save(string folder)
        {
            File.WriteAllBytes(Path.Combine(folder, FileName), Bytes);
        }

        public FileBytes(string name, byte[] bytes, string id = null)
        {
            FileName = name;
            Bytes = bytes;
            Extension = Path.GetExtension(name);
            MimeType = MimeData.GetFileMimeType(name);
            Id = id;
        }

        public FileBytes(string filePath)
        {
            FileName = Path.GetFileName(filePath);
            Extension = Path.GetExtension(filePath);
            MimeType = MimeData.GetFileMimeType(FileName);
            if (File.Exists(filePath))
            {
                Bytes = File.ReadAllBytes(filePath);
            }
        }

        public string ToBase64String()
        {
            return Convert.ToBase64String(Bytes);
        }

        public void SetFileName(string name)
        {
            FileName = name;
            Extension = Path.GetExtension(name);
            MimeType = MimeData.GetFileMimeType(name);
        }

        public void SetMimeType(string mime)
        {
            MimeType = mime;
        }
    }
}
