using Codeshell.Abp.Files.Uploads;

namespace Codeshell.Abp.Attachments
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
