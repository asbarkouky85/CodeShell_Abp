using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Files.Uploads
{
    public class TempFileDto
    {
        public Guid? Id { get; set; }
        public string FileName { get; set; }
        public string FileTempPath { get; set; }
        public int AttachmentTypeId { get; set; }
        public long? Size { get; set; }
        public TempFileDto()
        {

        }

        public TempFileDto(int type, Guid? id, string fileName = null)
        {
            Id = id;
            AttachmentTypeId = type;
            FileName = fileName;
        }
    }
}
