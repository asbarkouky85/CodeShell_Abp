using Codeshell.Abp.Attachments.Attachments;
using Codeshell.Abp.Attachments.Categories;
using Codeshell.Abp.Attachments.Paths;
using Codeshell.Abp.Files;
using Codeshell.Abp.Files.Uploads;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Codeshell.Abp.Attachments
{
    [ExposeServices(typeof(IAttachmentFileService), typeof(IInternalAttachmentAppService))]
    public class AttachmentFileService : AttachmentsAppService, IAttachmentFileService, IInternalAttachmentAppService
    {

        private readonly IAttachmentCategoryRepository _categories;
        private readonly IBlobContainerFactory _containerFactory;
        private readonly IRepository<TempFile, Guid> _tmpRepo;
        private bool _authorize = true;

        private IPathProvider PathProvider => LazyServiceProvider.LazyGetRequiredService<IPathProvider>();
        private IAttachmentRepository Repository => LazyServiceProvider.LazyGetRequiredService<IAttachmentRepository>();

        public AttachmentFileService(
            IAttachmentCategoryRepository categories,
            IRepository<TempFile, Guid> tmpRepo,
            IBlobContainerFactory containerFactory)
        {
            // _paths = paths;
            _tmpRepo = tmpRepo;
            _categories = categories;
            _containerFactory = containerFactory;
            //_authorize = false;
        }

        public async Task<AttachmentCategoryDto> GetCategoryInfo(int id)
        {
            var repo = LazyServiceProvider.LazyGetRequiredService<IRepository<AttachmentCategory, int>>();
            var cat = await repo.FindAsync(id, false);
            return ObjectMapper.Map(cat, new AttachmentCategoryDto());
        }

        public async Task<TempFileDto> GetFileName(Guid id)
        {
            string name = await Repository.GetFileName(id);
            if (name == null)
            {
                throw new UserFriendlyException("not found");
            }
            return new TempFileDto { FileName = name };
        }

        public async Task<FileBytes> GetBytes(Guid id)
        {
            await AuthorizeDownload(id);

            var att = await Repository.GetAsync(e => e.Id == id, true);
            if (att == null)
            {
                throw new UserFriendlyException("not found");
            }
            if (att.BinaryAttachmentId != null)
            {
                AttachmentBinary s = await Repository.GetBinaryAttachment(id);
                return new FileBytes(att.FileName, s.Bytes);
            }
            else
            {
                var c = _containerFactory.GetContainer(att.ContainerName);
                var b = await c.GetAllBytesOrNullAsync(att.FullPath);
                if (b == null)
                {
                    throw new UserFriendlyException(L["MSG__Not_found_on_provider"]);
                }
                var f = new FileBytes(att.FileName, b);
                return f;
            }
        }

        public async Task<FileBytes> GetTempBytes(string path)
        {
            if (Guid.TryParse(path, out Guid id))
            {
                var c = _containerFactory.GetTempContainer();
                var tmpFile = await _tmpRepo.FirstOrDefaultAsync(e => e.Id == id);
                if (tmpFile == null)
                {
                    throw new UserFriendlyException("not found");
                }
                var str = await c.GetAllBytesAsync(tmpFile.GetBlobName());
                var b = new FileBytes(tmpFile.FileName, str);
                return b;
            }
            throw new UserFriendlyException("not found");
        }

        public async Task SaveAttachmentsByPhysicalPath(Dictionary<Guid, string> files, int attachmentTypeId)
        {
            var cat = await _categories.FindAsync(attachmentTypeId);

            foreach (var file in files)
            {
                var tmpBytes = new FileBytes(file.Value);
                var att = new Attachment(file.Key, tmpBytes.FileName, attachmentTypeId, cat.ContainerName);
                IBlobContainer cont = _containerFactory.GetContainer(cat.ContainerName);

                var blobName = Utils.CombineUrl(cat.FolderPath ?? cat.Id.ToString(), att.Id.ToString() + tmpBytes.Extension);
                att.SetBlobName(blobName);

                await cont.SaveAsync(blobName, tmpBytes.Bytes);
                await Repository.InsertAsync(att);
            }
        }

        public async Task<FileValidationResultDto> SaveAttachment(SaveAttachmentRequest req)
        {
            var cat = await _categories.FindAsync(req.AttachmentTypeId);

            var tmpFile = await _tmpRepo.FirstOrDefaultAsync(e => e.Id == req.Id);
            if (tmpFile == null)
            {
                return new FileValidationResultDto { Message = L.GetString("MSG_file_is_not_in_tmp", req.FileName) };
            }

            var tmpContainer = _containerFactory.GetTempContainer();
            var tmpBytes = await tmpContainer.GetAllBytesOrNullAsync(tmpFile.GetBlobName());

            var dto = new FileBytes(tmpFile.FileName, tmpBytes);

            ValidateFiles(cat, new[] { dto });

            var att = new Attachment(req.Id ?? DomainUtils.NewGuid(), req.FileName, req.AttachmentTypeId, cat.ContainerName);
            IBlobContainer cont = _containerFactory.GetContainer(cat.ContainerName);

            var blobName = Utils.CombineUrl(cat.FolderPath ?? cat.Id.ToString(), att.Id.ToString() + dto.Extension);
            att.SetBlobName(blobName);

            await cont.SaveAsync(blobName, dto.Bytes);
            await Repository.InsertAsync(att);

            await tmpContainer.DeleteAsync(tmpFile.GetBlobName());
            await _tmpRepo.DeleteAsync(tmpFile);

            return new FileValidationResultDto { Code = "0", Message = "Success" };
        }

        public async Task<TempFileDto> ChunkUpload(ChunkUploadRequestDto dto)
        {
            if (!dto.Id.HasValue)
            {
                var cat = await _categories.FindAsync(dto.AttachmentTypeId);
                ValidateFiles(cat, new[] { dto });
                dto.Id = DomainUtils.NewGuid();
            }

            TempFile tmp = null;
            tmp = new TempFile(dto.Id.Value, dto.FileName);

            var blobName = tmp.GetBlobName();

            var tmpDto = new TempFileDto
            {
                FileTempPath = blobName,
                FileName = dto.FileName,
                AttachmentTypeId = dto.AttachmentTypeId,
                Id = tmp.Id
            };

            var path = await PathProvider.GetTempFolderPath();
            var filePath = Path.Combine(path, blobName);
            var bytes = Convert.FromBase64String(dto.Chunk);
            if (!File.Exists(filePath))
            {
                if (dto.CurrentChunkIndex > 0)
                {
                    throw new UserFriendlyException("FileDoesNotExists", "FileDoesNotExists");
                }
                Utils.CreateFolderForFile(filePath);
                File.WriteAllBytes(filePath, bytes);
            }
            else
            {
                using (var file = new FileStream(filePath, FileMode.Append))
                {
                    foreach (var b in bytes)
                    {
                        file.WriteByte(b);
                    }
                }
            }

            if (dto.CurrentChunkIndex >= (dto.TotalChunkCount - 1))
            {
                var tmContainer = _containerFactory.GetTempContainer();
                await tmContainer.SaveAsync(blobName, File.ReadAllBytes(filePath));
                await _tmpRepo.InsertAsync(tmp);
            }

            return tmpDto;
        }

        public async Task<UploadResult> Upload(UploadRequestDto dto)
        {
            var cat = await _categories.FindAsync(dto.AttachmentTypeId);
            ValidateFiles(cat, dto.Files);

            List<TempFileDto> lst = new List<TempFileDto>();
            var tmContainer = _containerFactory.GetTempContainer();

            foreach (var d in dto.Files)
            {
                if (!MagicNumbersData.ValidateMagic(d.Extension, d.Bytes))
                {
                    throw new UserFriendlyException(L.GetString("MSG__Content_does_not_match_a_{0}_file", d.Extension));
                }
                var id = DomainUtils.NewGuid();
                var tmp = new TempFile(id, d.FileName);
                var blobName = tmp.GetBlobName();
                await tmContainer.SaveAsync(blobName, d.Bytes);
                var tmpDto = new TempFileDto
                {
                    FileTempPath = blobName,
                    FileName = d.FileName,
                    AttachmentTypeId = dto.AttachmentTypeId,
                    Id = id
                };

                lst.Add(tmpDto);
                await _tmpRepo.InsertAsync(tmp);
            }
            return new UploadResult
            {
                Data = lst.ToArray()
            };
        }

        public Task<UploadResult> Upload()
        {

            throw new NotImplementedException();
        }

        public async Task<FileValidationResultDto> ValidateFile(FileValidationRequest req)
        {
            var res = new FileValidationResultDto();
            var cat = await _categories.FindAsync(req.AttachmentType);
            if (cat == null)
            {
                res.Message = L.GetString("MSG_unknown_category", req.AttachmentType);
                res.Code = "1";
                return res;
            }
            ValidateFiles(cat, new[] { req });

            return res;
        }

        private string GetCategoryName(AttachmentCategory cat)
        {
            return CurrentCulture.Lang == "ar" ? (cat.NameAr ?? cat.NameEn) : cat.NameEn;// L["AttachmentTypes_" + ((AttachmentTypes)cat.Id).ToString()];
        }

        private void AuthorizeUpload(AttachmentCategory cat)
        {
            if (!_authorize)
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
            if (!_authorize)
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

        private bool HasDoubleExtension(string fileName)
        {
            var regex = new Regex("\\.");

            var coll = regex.Matches(fileName);
            return coll.Count > 1;
        }

        private void ValidateFiles(AttachmentCategory cat, IEnumerable<IFileInfo> lst)
        {



            if (cat == null)
            {
                throw new UserFriendlyException(L.GetString("MSG_unknown_category", ""));
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
                    pars.AddRange(res.Parameters?.Select(e => e).ToArray());

                    throw new UserFriendlyException(L.GetString(res.Message, pars.ToArray()));
                }
            }
        }


    }
}
