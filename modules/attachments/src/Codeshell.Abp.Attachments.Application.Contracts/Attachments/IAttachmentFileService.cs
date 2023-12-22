using Codeshell.Abp.Attachments.Categories;
using Codeshell.Abp.Files;
using Codeshell.Abp.Files.Uploads;
using System;
using System.Threading.Tasks;

namespace Codeshell.Abp.Attachments
{
    public interface IAttachmentFileService
    {
        Task<UploadResult> Upload(UploadRequestDto dto);
        Task<TempFileDto> ChunkUpload(ChunkUploadRequestDto dto);
        Task<FileValidationResultDto> ValidateFile(FileValidationRequest req);
        Task<FileValidationResultDto> SaveAttachment(SaveAttachmentRequest req);
        Task<FileBytes> GetBytes(Guid id);
        Task<FileBytes> GetTempBytes(string path);
        Task<TempFileDto> GetFileName(Guid id);
        Task<AttachmentCategoryDto> GetCategoryInfo(int id);
    }
}
