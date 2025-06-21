using Codeshell.Abp.Attachments.Categories;
using Codeshell.Abp.Files;
using Codeshell.Abp.Files.Uploads;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeshell.Abp.Attachments.Attachments;
using Codeshell.Abp.Attachments;

namespace Codeshell.Abp.Attachments.Attachments
{
    public interface IAttachmentFileService
    {
        Task<TempFileDto> ChunkUpload(ChunkUploadRequestDto dto);
        Task<FileValidationResultDto> ValidateFile(FileValidationRequest req);
        Task<FileValidationResultDto> SaveAttachment(SaveAttachmentRequestDto req);
        Task<string> GetFileBase64String(Guid id);
        Task<UploadedFileInfoDto> GetFileName(Guid id);
        Task<List<UploadedFileInfoDto>> GetFilesInfo(UploadedFileInfoRequestDto dto);
        Task<AttachmentCategoryDto> GetCategoryInfo(int id);
    }
}
