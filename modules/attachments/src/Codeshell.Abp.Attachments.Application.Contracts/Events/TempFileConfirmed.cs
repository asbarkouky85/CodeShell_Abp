using System;
using System.Collections.Generic;
using System.Text;
using Codeshell.Abp.Attachments.Attachments;

namespace Codeshell.Abp.Attachments.Events
{
    public class TempFileConfirmed
    {
        public SaveAttachmentRequestDto SaveRequest { get; set; }
    }
}
