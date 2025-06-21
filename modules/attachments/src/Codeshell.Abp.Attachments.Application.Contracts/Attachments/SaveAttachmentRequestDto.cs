using Codeshell.Abp.Files.Uploads;
using Codeshell.Abp.Attachments;

namespace Codeshell.Abp.Attachments.Attachments
{
    public class SaveAttachmentRequestDto : TempFileDto
    {
        public string ContainerName { get; set; }
        public string Folder { get; set; }
    }
}
