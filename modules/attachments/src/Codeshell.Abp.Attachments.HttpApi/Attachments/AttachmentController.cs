using Codeshell.Abp.Attachments.Categories;
using Codeshell.Abp.Files.Uploads;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeshell.Abp.Attachments.Attachments;

namespace Codeshell.Abp.Attachments
{
    public class AttachmentController : AttachmentsControllerBase, IAttachmentFileService
    {

        private readonly IAttachmentFileService fileAppService;
        private readonly IInternalAttachmentService internalAttachmentService;

        public AttachmentController(
            IAttachmentFileService fileAppService,
            IInternalAttachmentService internalAttachmentService
            )
        {
            this.fileAppService = fileAppService;
            this.internalAttachmentService = internalAttachmentService;
        }

        public async Task<string> GetFileBase64String(Guid id)
        {
            return await fileAppService.GetFileBase64String(id);
        }

        public async Task<UploadedFileInfoDto> GetFileName(Guid id)
        {
            return await fileAppService.GetFileName(id);
        }

        public async Task<FileValidationResultDto> SaveAttachment(SaveAttachmentRequestDto req)
        {
            return await fileAppService.SaveAttachment(req);
        }

        public async Task<FileValidationResultDto> ValidateFile([FromBody] FileValidationRequest req)
        {
            return await fileAppService.ValidateFile(req);
        }

        public async Task<UploadResult> Upload([FromForm] UploadRequestDto dto)
        {
            return await UploadMultiPart(dto.AttachmentTypeId);
        }

        public async Task<UploadResult> UploadAndSave(UploadRequestDto req)
        {
            return await UploadMultiPart(req.AttachmentTypeId, true);
        }

        public async Task<object> Get(Guid id)
        {
            var f = await internalAttachmentService.GetBytes(id);
            return File(f.Bytes, f.MimeType, f.FileName);
        }

        public async Task<object> GetTemp(string path)
        {
            var f = await internalAttachmentService.GetTempBytes(path);
            return File(f.Bytes, f.MimeType, f.FileName);
        }

        [Consumes("multipart/form-data")]
        private async Task<UploadResult> UploadMultiPart(int catId, bool save = false)
        {
            var lst = new List<TempFileDto>();
            foreach (var f in Request.Form.Files)
            {

                using (var st = f.OpenReadStream())
                {

                    var byts = new UploadedStream(f.FileName, st);
                    if (save)
                    {
                        var dto = await internalAttachmentService.UploadAndSaveFile(byts, catId);
                        lst.Add(dto);
                    }
                    else
                    {
                        var dto = await internalAttachmentService.UploadFile(byts, catId);
                        lst.Add(dto);
                    }
                    //if (byts.MimeType.ToLower().StartsWith("image"))
                    //{
                    //    try
                    //    {

                    //        //using var im = SKImage.From(str);
                    //        //byts.Dimesion = new FileDimesion { Width = im.Width, Height = im.Height };
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Console.WriteLine(e.Message);
                    //    }

                    //}
                }
            }
            return new UploadResult
            {
                Data = lst.ToArray()
            };
        }

        public async Task<AttachmentCategoryDto> GetCategoryInfo(int id)
        {
            return await fileAppService.GetCategoryInfo(id);
        }

        public async Task<TempFileDto> ChunkUpload(ChunkUploadRequestDto dto)
        {
            return await fileAppService.ChunkUpload(dto);
        }

        public async Task<List<UploadedFileInfoDto>> GetFilesInfo(UploadedFileInfoRequestDto dto)
        {
            return await fileAppService.GetFilesInfo(dto);
        }
    }
}
