using Codeshell.Abp.Files.Uploads;

namespace Codeshell.Abp.Attachments
{

    public interface IHasFileDto
    {
        TempFileDto File { get; set; }
    }

}
