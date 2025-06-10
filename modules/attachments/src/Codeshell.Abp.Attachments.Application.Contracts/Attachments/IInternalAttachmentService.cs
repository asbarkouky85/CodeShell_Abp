using Codeshell.Abp.Files;
using Codeshell.Abp.Files.Uploads;
using System;
using System.Threading.Tasks;

namespace Codeshell.Abp.Attachments
{
    public interface IInternalAttachmentService
    {
        Task<TempFileDto> UploadFile(UploadedStream stream, int attachmentTypeId);
        Task<TempFileDto> UploadAndSaveFile(UploadedStream stream, int attachmentTypeId);
        Task<FileBytes> GetBytes(Guid id);
        Task<FileBytes> GetTempBytes(string path);
    }
}