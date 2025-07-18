﻿using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;

namespace Codeshell.Abp.Attachments
{
    public class AttachmentCategoryPermission : AuditedEntity<long>
    {
        public int AttachmentCategoryId { get; protected set; }
        public AttachmentCategory AttachmentCategory { get; protected set; }
        public string Role { get; protected set; }
        public bool Upload { get; protected set; }
        public bool Download { get; protected set; }

        protected AttachmentCategoryPermission()
        {

        }

        public AttachmentCategoryPermission(string role, bool upload, bool downLoad)
        {
            Role = role;
            Upload = upload;
            Download = downLoad;
        }

        public void SetPermission(bool? upload, bool? download)
        {
            if (upload.HasValue)
                Upload = upload.Value;
            if (download.HasValue)
                Download = download.Value;
        }
    }
}
