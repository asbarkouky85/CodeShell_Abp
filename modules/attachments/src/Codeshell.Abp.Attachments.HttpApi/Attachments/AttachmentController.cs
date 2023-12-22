using Codeshell.Abp.Attachments.Categories;
using Codeshell.Abp.Files;
using Codeshell.Abp.Files.Uploads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Codeshell.Abp.Attachments
{
    [AllowAnonymous]
    public class AttachmentController : AttachmentsControllerBase, IAttachmentFileService
    {

        private readonly IAttachmentFileService fileAppService;

        public AttachmentController(
            IAttachmentFileService fileAppService
            )
        {
            this.fileAppService = fileAppService;
        }


        public Task<FileBytes> GetBytes(Guid id) => fileAppService.GetBytes(id);
        public Task<FileBytes> GetTempBytes(string path) => fileAppService.GetTempBytes(path);
        public Task<TempFileDto> GetFileName(Guid id) => fileAppService.GetFileName(id);
        public Task<FileValidationResultDto> SaveAttachment(SaveAttachmentRequest req) => fileAppService.SaveAttachment(req);
        public Task<FileValidationResultDto> ValidateFile([FromBody] FileValidationRequest req) => fileAppService.ValidateFile(req);
        public Task<UploadResult> Upload([FromForm] UploadRequestDto dto) => (dto.Files == null) ? UploadMultiPart(dto.AttachmentTypeId) : fileAppService.Upload(dto);

        //[AbpAllowAnonymous]
        public async Task<object> Get(Guid id)
        {
            var f = await fileAppService.GetBytes(id);
            return File(f.Bytes, f.MimeType);
        }

        public async Task<object> GetTemp(string path)
        {
            var f = await fileAppService.GetTempBytes(path);
            return File(f.Bytes, f.MimeType);
        }

        [Consumes("multipart/form-data")]
        public Task<UploadResult> UploadMultiPart(int catId)
        {
            var req = new UploadRequestDto
            {
                AttachmentTypeId = catId,
                Files = new List<FileBytes>()
            };

            foreach (var f in Request.Form.Files)
            {
                using MemoryStream str = new MemoryStream();
                f.CopyTo(str);
                var byts = new FileBytes(f.FileName, str.ToArray());
                if (byts.MimeType.ToLower().StartsWith("image"))
                {
                    try
                    {
                        using var im = Image.FromStream(str);
                        byts.Dimesion = new FileDimesion { Width = im.Width, Height = im.Height };
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
                req.Files.Add(byts);
            }
            return fileAppService.Upload(req);
        }

       
        public Task<AttachmentCategoryDto> GetCategoryInfo(int id)
        {
            return fileAppService.GetCategoryInfo(id);
        }

        public Task<TempFileDto> ChunkUpload(ChunkUploadRequestDto dto)
        {
            return fileAppService.ChunkUpload(dto);
        }
    }
}
