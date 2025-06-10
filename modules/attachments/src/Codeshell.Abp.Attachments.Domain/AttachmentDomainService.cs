using Codeshell.Abp.Files;
using Microsoft.Extensions.Options;
using Codeshell.Abp.Attachments.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using System.IO.Compression;
using Codeshell.Abp.Extensions;
using Codeshell.Abp.Files.Uploads;
using System.IO;

namespace Codeshell.Abp.Attachments
{
    public class AttachmentDomainService : DomainService, IAttachmentDomainService, ITransientDependency
    {
        IAttachmentRepository Repository;
        ITempFileRepository _tempFileRepo => LazyServiceProvider.LazyGetRequiredService<ITempFileRepository>();
        IAttachmentCategoryRepository categoryRepo => LazyServiceProvider.LazyGetRequiredService<IAttachmentCategoryRepository>();
        private readonly IBlobContainerFactory _containerFactory;
        AttachmentsModuleOptions Options;
        public AttachmentDomainService(
            IAttachmentRepository repository,
            IBlobContainerFactory containerFactory,
            IOptions<AttachmentsModuleOptions> _options)
        {
            Repository = repository;
            _containerFactory = containerFactory;
            Options = _options.Value;
        }

        public async Task<Attachment> SaveTemp(TempFile tmpFile)
        {
            var cat = tmpFile.AttachmentCategory;

            if (cat == null)
                cat = await categoryRepo.FindAsync(tmpFile.AttachmentCategoryId);

            var attachment = new Attachment(tmpFile.Id, tmpFile.FileName, tmpFile.AttachmentCategoryId, cat.ContainerName);

            attachment.SetSize(tmpFile.Size);
            attachment.SetBlobName(tmpFile.FullPath);
            await Repository.InsertAsync(attachment);
            await _tempFileRepo.DeleteAsync(tmpFile);

            return attachment;
        }

        public async Task<Attachment> SaveBytes(int attachmentTypeId, FileBytes tmpBytes, Guid? id = null)
        {
            var cat = await categoryRepo.FindAsync(attachmentTypeId, false);
            var attachment = new Attachment(id ?? DomainUtils.NewGuid(), tmpBytes.FileName, attachmentTypeId);
            IBlobContainer cont = _containerFactory.Create(cat.ContainerName ?? "default");

            var blobName = Utils.CombineUrl(cat.FolderPath ?? cat.Id.ToString(), attachment.Id.ToString() + tmpBytes.Extension);
            attachment.SetBlobName(blobName);
            attachment.SetSize((int)tmpBytes.Size);
            await cont.SaveAsync(blobName, tmpBytes.Bytes);
            await Repository.InsertAsync(attachment);
            return attachment;
        }

        public async Task<List<Attachment>> ImportAttachments(byte[] archive)
        {
            var outPath = Path.Combine(".", Guid.NewGuid().ToString());

            File.WriteAllBytes(outPath + ".zip", archive);
            FileUtils.DecompressDirectory(outPath + ".zip", outPath);
            File.Delete(outPath + ".zip");

            var dataString = await File.ReadAllTextAsync(Path.Combine(outPath, "_data.json"));
            var data = dataString.FromJson<List<TempFileDto>>().Where(e => e.Id != null).ToList();
            var result = new List<Attachment>();
            var ids = data.Select(e => e.Id).ToList();
            var existingIds = await Repository.GetAs(e => e.Id, e => ids.Contains(e.Id));

            foreach (var file in data)
            {
                if (!existingIds.Contains(file.Id.Value))
                {
                    var bytes = new FileBytes(Path.Combine(outPath, "files", file.Id.ToString()));
                    bytes.SetFileName(file.FileName);

                    var att = await SaveBytes(file.AttachmentTypeId, bytes, file.Id);
                    result.Add(att);
                }
            }
            FileUtils.DeleteDirectory(outPath);
            return result;
        }

        public async Task<AttachmentExportResult> ExportAttachments(IEnumerable<Guid> attachmentIds, string outputPath)
        {
            var attachments = await Repository.GetListAsync(e => attachmentIds.Contains(e.Id));
            List<TempFileDto> tempList = new();

            var factory = LazyServiceProvider.LazyGetRequiredService<IBlobContainerFactory>();
            var archivePath = Path.Combine(outputPath, $"Export-{Guid.NewGuid()}");
            foreach (var attachment in attachments)
            {
                var info = new TempFileDto
                {
                    AttachmentTypeId = attachment.AttachmentCategoryId,
                    FileName = attachment.FileName,
                    Size = attachment.Size,
                    Id = attachment.Id
                };
                tempList.Add(info);
                var file = await GetFile(attachment);
                var path = Path.Combine(archivePath, "files", attachment.Id.ToString());
                Utils.CreateFolderForFile(path);
                File.WriteAllBytes(path, file.Bytes);
            }
            File.WriteAllText(Path.Combine(archivePath, "_data.json"), tempList.ToJsonIndent());
            FileUtils.CompressDirectory(archivePath, archivePath + ".zip");
            FileUtils.DeleteDirectory(archivePath);
            return new AttachmentExportResult { Path = archivePath + ".zip" };
        }

        public async Task<FileBytes> GetFile(Attachment att)
        {
            if (att.BinaryAttachmentId != null)
            {
                var s = (await Repository.GetQueryableAsync()).Where(e => e.Id == att.Id).Select(e => e.BinaryAttachment).FirstOrDefault();
                if (s != null)
                    return new FileBytes(att.FileName, s.Bytes);
                else
                    return new FileBytes(att.FileName, new byte[0]);
            }
            else
            {
                var c = _containerFactory.GetContainer(att.ContainerName);
                byte[] byts = new byte[0];

                if (!Options.Mock)
                {
                    byts = await c.GetAllBytesOrNullAsync(att.FullPath);
                    if (byts == null)
                        return null;
                }
                return new FileBytes(att.FileName, byts);

            }
        }
    }
}
