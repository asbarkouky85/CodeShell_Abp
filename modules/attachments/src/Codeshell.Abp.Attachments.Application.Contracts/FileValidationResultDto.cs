using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Attachments
{
    public class FileValidationResultDto
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public string Code { get; set; }

        
    }
}
