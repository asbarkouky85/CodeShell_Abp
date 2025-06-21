using Codeshell.Abp.Attachments.BlobStoring;
using Codeshell.Abp.Attachments.Categories;
using Codeshell.Abp.Files;
using Codeshell.Abp.Files.Uploads;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Validation;
using Codeshell.Abp.Attachments.Extensions;
using Codeshell.Abp.Attachments.Attachments;

namespace Codeshell.Abp.Attachments
{
    [ExposeServices(typeof(IAttachmentFileService), typeof(IInternalAttachmentService))]
    public class AttachmentFileService : AttachmentsAppService, IAttachmentFileService, IInternalAttachmentService
    {
        private readonly IAttachmentCategoryRepository _categories;
        private readonly IBlobContainerFactory _containerFactory;
        private readonly ITempFileRepository _tmpRepo;
        protected IChunkUploaderFactory ChunkUploaderFactory => LazyServiceProvider.LazyGetRequiredService<IChunkUploaderFactory>();
        private IRepository<TempFileChunk, Guid> ChunkRepo => LazyServiceProvider.LazyGetRequiredService<IRepository<TempFileChunk, Guid>>();

        protected AttachmentsModuleOptions Options { get; private set; }
        private IAttachmentRepository Repository => LazyServiceProvider.LazyGetService<IAttachmentRepository>();
        private IAttachmentDomainService DomainService => LazyServiceProvider.LazyGetService<IAttachmentDomainService>();

        public AttachmentFileService(
            IAttachmentCategoryRepository categories,
            IOptions<AttachmentsModuleOptions> _options,
            ITempFileRepository tmpRepo,
            IBlobContainerFactory containerFactory)
        {
            _tmpRepo = tmpRepo;
            _categories = categories;
            _containerFactory = containerFactory;
            Options = _options.Value;
        }

        public async Task<AttachmentCategoryDto> GetCategoryInfo(int id)
        {
            var repo = LazyServiceProvider.LazyGetRequiredService<IRepository<AttachmentCategory, int>>();
            var cat = await repo.FindAsync(id, false);
            return ObjectMapper.Map(cat, new AttachmentCategoryDto());
        }

        public async Task<UploadedFileInfoDto> GetFileName(Guid id)
        {
            UploadedFileInfo inf = await Repository.GetInfo(id);
            if (inf == null)
            {
                return new UploadedFileInfoDto
                {
                    Id = id,
                    FileName = "(N/A)"
                };
            }
            return ObjectMapper.Map(inf, new UploadedFileInfoDto());
        }

        public async Task<List<UploadedFileInfoDto>> GetFilesInfo(UploadedFileInfoRequestDto dto)
        {
            var data = await Repository.GetInfo(dto.Ids);
            return ObjectMapper.Map(data, new List<UploadedFileInfoDto>());
        }

        public virtual async Task<TempFileDto> ChunkUpload(ChunkUploadRequestDto dto)
        {
            var cat = await _categories.FindAsync(dto.AttachmentTypeId);
            var chunkUploader = ChunkUploaderFactory.Create(cat.ContainerName ?? "default");
            TempFile tmp;
            if (!dto.Id.HasValue)
            {
                ValidateFiles(dto.AttachmentTypeId, cat, new[] { dto });
                dto.Id = DomainUtils.NewGuid();
                tmp = new TempFile(dto.Id.Value, dto.AttachmentTypeId, dto.FileName, dto.TotalChunkCount);
                var blobName = chunkUploader.NormalizeName(cat.GenerateBlobName(tmp));
                tmp.SetFullPath(blobName);
                tmp.SetFileSize(dto.Size);
                string referenceId = null;
                if (!Options.Mock)
                    referenceId = await chunkUploader.StartFile(tmp.FullPath);
                else
                    referenceId = Utils.RandomAlphaNumeric(20, CharType.Small);

                tmp.SetRefernceId(referenceId);
                await _tmpRepo.InsertAsync(tmp);
            }
            else
            {
                tmp = await _tmpRepo.FindAsync(dto.Id.Value);
            }

            var tmpDto = new TempFileDto
            {
                FileName = dto.FileName,
                AttachmentTypeId = dto.AttachmentTypeId,
                Id = dto.Id.Value,
                FileTempPath = tmp.FullPath
            };

            var bytes = Convert.FromBase64String(dto.Chunk);

            string chunkReference = null;

            if (!Options.Mock)
                chunkReference = await chunkUploader.UploadPart(tmp.FullPath, tmp.ReferenceId, dto.CurrentChunkIndex, bytes);
            else
                chunkReference = Utils.RandomAlphaNumeric(20, CharType.Small);

            var addedChunk = new TempFileChunk(dto.Id.Value, dto.CurrentChunkIndex, chunkReference);

            await ChunkRepo.InsertAsync(addedChunk);
            await CurrentUnitOfWork.SaveChangesAsync();

            var complete = await _tmpRepo.IsChunksComplete(dto.Id.Value);

            if (complete)
            {
                var temp = await _tmpRepo.GetWithChunks(dto.Id.Value);
                var chunkData = temp.Chunks.ToDictionary(e => e.ChunkIndex, e => e.ReferenceId);
                if (!Options.Mock)
                    await chunkUploader.Finish(temp.FullPath, tmp.ReferenceId, chunkData);

            }

            return tmpDto;
        }

        public async Task<FileBytes> GetBytes(Guid id)
        {
            await AuthorizeDownload(id);

            var att = await Repository.GetAsync(e => e.Id == id, true);
            if (att == null)
            {
                throw new UserFriendlyException("not found");
            }
            var file = await DomainService.GetFile(att);
            if (file == null)
            {
                throw new UserFriendlyException(L["MSG__Not_found_on_provider"]);
            }
            return file;
        }

        public async Task<string> GetFileBase64String(Guid id)
        {
            await AuthorizeDownload(id);

            var att = await Repository.GetAsync(e => e.Id == id, true);

            if (att == null)
                throw new UserFriendlyException("not found");

            var fileBytes = await DomainService.GetFile(att);
            if (fileBytes == null)
                throw new UserFriendlyException(L["MSG__Not_found_on_provider"]);

            var fileBase64String = Convert.ToBase64String(fileBytes.Bytes);
            return $"data:image/{fileBytes.MimeType};base64,{fileBase64String}";
        }

        public async Task<FileBytes> GetTempBytes(string path)
        {
            if (Guid.TryParse(path, out Guid id))
            {
                var tmpFile = await _tmpRepo.GetWithCategory(id);
                if (tmpFile == null)
                {
                    throw new UserFriendlyException("not found");
                }
                var c = _containerFactory.GetContainer(tmpFile.AttachmentCategory.ContainerName);
                await AuthorizeDownloadByCategory(tmpFile.AttachmentCategoryId);
                byte[] str = new byte[0];
                if (!Options.Mock)
                {
                    str = await c.GetAllBytesOrNullAsync(tmpFile.FullPath);
                }

                return new FileBytes(tmpFile.FileName, str);
            }
            throw new UserFriendlyException("not found");
        }

        public virtual async Task<FileValidationResultDto> SaveAttachment(SaveAttachmentRequestDto req)
        {
            if (req.Id == null)
                throw new AbpValidationException("Id is required");

            var tmpFile = await _tmpRepo.GetWithCategory(req.Id.Value);
            if (tmpFile == null)
            {
                return new FileValidationResultDto { Message = L.GetString("MSG_file_is_not_in_tmp", req.FileName) };
            }
            
            return new FileValidationResultDto { Code = "0", Message = "Success" };
        }

        [DisableValidation]
        public async Task<TempFileDto> UploadFile(UploadedStream stream, int attachmentTypeId)
        {
            var cat = await _categories.FindAsync(attachmentTypeId);
            ValidateFiles(attachmentTypeId, cat, new[] { stream });
            if (!MagicNumbersData.ValidateMagic(stream.Extension, stream.Stream))
            {
                throw new UserFriendlyException(L.GetString("MSG__Content_does_not_match_a_{0}_file", stream.Extension));
            }
            var id = DomainUtils.NewGuid();
            var tmp = new TempFile(id, cat.Id, stream.FileName);
            var blobName = cat.GenerateBlobName(tmp);
            tmp.SetFullPath(blobName);
            tmp.SetFileSize(stream.Size);

            var tmContainer = _containerFactory.GetContainer(cat.ContainerName);
            if (!Options.Mock)
                await tmContainer.SaveAsync(blobName, stream.Stream);

            var tmpDto = new TempFileDto
            {
                FileTempPath = tmp.FullPath,
                FileName = stream.FileName,
                AttachmentTypeId = cat.Id,
                Id = id,
                Size = stream.Size
            };

            await _tmpRepo.InsertAsync(tmp);
            return tmpDto;
        }

        [DisableValidation]
        public async Task<TempFileDto> UploadAndSaveFile(UploadedStream stream, int attachmentTypeId)
        {
            var cat = await _categories.FindAsync(attachmentTypeId);
            ValidateFiles(attachmentTypeId, cat, new[] { stream });
            if (!MagicNumbersData.ValidateMagic(stream.Extension, stream.Stream))
            {
                throw new UserFriendlyException(L.GetString("MSG__Content_does_not_match_a_{0}_file", stream.Extension));
            }
            var id = DomainUtils.NewGuid();
            var att = new Attachment(Guid.NewGuid(), stream.FileName, attachmentTypeId, cat.ContainerName);
            var blobName = cat.GenerateBlobName(att);
            att.SetBlobName(blobName);
            att.SetSize(stream.Size);

            var tmContainer = _containerFactory.GetContainer(cat.ContainerName);
            if (!Options.Mock)
                await tmContainer.SaveAsync(blobName, stream.Stream);
            var tmpDto = new TempFileDto
            {
                FileTempPath = att.FullPath,
                FileName = stream.FileName,
                AttachmentTypeId = cat.Id,
                Id = id,
                Size = stream.Size
            };

            await Repository.InsertAsync(att);
            return tmpDto;
        }

        public async Task<FileValidationResultDto> ValidateFile(FileValidationRequest req)
        {
            var res = new FileValidationResultDto();
            var cat = await _categories.FindAsync(req.AttachmentType);
            ValidateFiles(req.AttachmentType, cat, new[] { req });
            return res;
        }

        private string GetCategoryName(AttachmentCategory cat)
        {
            return CurrentCulture.Lang == "ar" ? cat.NameAr : cat.NameEn;
        }

        private void AuthorizeUpload(AttachmentCategory cat)
        {
            if (!Options.Authorize)
                return;
            if (!CurrentUser.IsAuthenticated)
            {
                if (!cat.AllowAnonymousUpload())
                    throw new UserFriendlyException(L.GetString("MSG_unauthorized_upload", GetCategoryName(cat)));
            }
            else
            {
                if (!cat.CanUpload(CurrentUser.Roles.ToList()))
                    throw new UserFriendlyException(L.GetString("MSG_unauthorized_upload", GetCategoryName(cat)));
            }
        }

        private async Task AuthorizeDownload(Guid attachmentId)
        {
            if (!Options.Authorize)
                return;
            if (CurrentUser == null || !CurrentUser.IsAuthenticated)
            {
                if (!await _categories.AnonymousDownloadAllowed(attachmentId))
                {
                    throw new AbpAuthorizationException(L["MSG__Unauthorized_download"]);
                }
            }
            else
            {
                if (!await _categories.DownloadAllowed(CurrentUser.Roles, attachmentId))
                {
                    throw new AbpAuthorizationException(L["MSG__Unauthorized_download"]);
                }
            }
        }

        private async Task AuthorizeDownloadByCategory(int attachmentCategoryId)
        {
            if (!Options.Authorize)
                return;
            if (CurrentUser == null || !CurrentUser.IsAuthenticated)
            {
                if (!await _categories.AnonymousDownloadAllowed(attachmentCategoryId))
                {
                    throw new AbpAuthorizationException(L["MSG__Unauthorized_download"]);
                }
            }
            else
            {
                if (!await _categories.DownloadAllowed(CurrentUser.Roles, attachmentCategoryId))
                {
                    throw new AbpAuthorizationException(L["MSG__Unauthorized_download"]);
                }
            }
        }

        private bool HasDoubleExtension(string fileName)
        {
            var regex = new Regex("\\.", RegexOptions.IgnoreCase, new TimeSpan(0, 0, 2));

            var coll = regex.Matches(fileName);
            return coll.Count > 1;
        }

        private void ValidateFiles(int id, AttachmentCategory cat, IEnumerable<IFileInfo> lst)
        {

            if (cat == null)
            {
                throw new UserFriendlyException(L.GetString("MSG_unknown_category", id));
            }

            AuthorizeUpload(cat);

            if (cat.MaxCount.HasValue && lst.Count() > cat.MaxCount)
            {
                throw new UserFriendlyException(L.GetString("MSG_file_count_exceeds_maximum_allowed", GetCategoryName(cat), cat.MaxCount));
            }

            foreach (var d in lst)
            {
                if (HasDoubleExtension(d.FileName))
                {
                    throw new UserFriendlyException(L.GetString("MSG__File_can_not_have_two_dots_in_its_name"));
                }
                if (!cat.ValidateFile(d, out ValidationResult res))
                {
                    var pars = new List<object> { GetCategoryName(cat) };

                    throw new UserFriendlyException(L.GetString(res.ErrorMessage, pars.ToArray()));
                }
            }
        }


    }
}
