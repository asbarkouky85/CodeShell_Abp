using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace Codeshell.Abp.Attachments
{
    public class TempFileChunk : Entity<Guid>
    {

        public Guid TempFileId { get; protected set; }
        public TempFile TempFile { get; protected set; }
        public int ChunkIndex { get; protected set; }
        public string ReferenceId { get; protected set; }

        protected TempFileChunk()
        {

        }

        public TempFileChunk(Guid tempFileId, int currentChunkIndex, string referenceId) : this()
        {
            TempFileId = tempFileId;
            ChunkIndex = currentChunkIndex;
            ReferenceId = referenceId;
        }

    }
}
