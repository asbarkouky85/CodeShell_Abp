using Codeshell.Abp.Files.Uploads;
using Codeshell.Abp.Attachments;

namespace Codeshell.Abp.Attachments.Attachments
{
    public class UploadResult
    {
        public virtual bool Success => Code == "0";
        public string Code { get; set; } = "0";
        public string Message { get; set; }
        public string Details { get; set; }
        public TempFileDto[] Data { get; set; }
    }
}
