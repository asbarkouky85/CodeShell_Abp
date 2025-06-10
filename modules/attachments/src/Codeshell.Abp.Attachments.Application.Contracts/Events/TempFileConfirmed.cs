using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Attachments.Events
{
    public class TempFileConfirmed
    {
        public SaveAttachmentRequestDto SaveRequest { get; set; }
    }
}
