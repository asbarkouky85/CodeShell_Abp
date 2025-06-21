using Codeshell.Abp.Files;
using System;
using Codeshell.Abp.Attachments;

namespace Codeshell.Abp.Attachments.Attachments
{
    public class ChunkUploadRequestDto : IFileInfo
    {
        public Guid? Id { get; set; }
        public int AttachmentTypeId { get; set; }
        public int CurrentChunkIndex { get; set; }
        public int TotalChunkCount { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string Chunk { get; set; }
        public long? Size { get; set; }

        public FileDimesion Dimesion { get; set; }
    }
}
