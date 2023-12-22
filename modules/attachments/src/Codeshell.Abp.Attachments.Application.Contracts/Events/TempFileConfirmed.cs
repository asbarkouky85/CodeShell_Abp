using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Attachments.Events
{
    public class TempFileConfirmed
    {
        public SaveAttachmentRequest SaveRequest { get; set; }
    }
}
