using Microsoft.Extensions.Logging;
using Codeshell.Abp.Attachments.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Settings;
using Codeshell.Abp.Attachments.Extensions;
using Volo.Abp.Uow;

namespace Codeshell.Abp.Attachments
{
    public class TempFileService : ITempFileService
    {
        private readonly ITempFileRepository repository;
        private readonly IBlobContainerFactory containerFactory;
        private readonly ILogger<TempFileService> logger;
        readonly ISettingProvider _settings;

        public TempFileService(ITempFileRepository repository, IBlobContainerFactory containerFactory, ILogger<TempFileService> logger, ISettingProvider settings)
        {
            this.repository = repository;
            this.containerFactory = containerFactory;
            this.logger = logger;
            _settings = settings;
        }

        [UnitOfWork]
        public virtual async Task CleanUp(DateTime createdBefore)
        {
            var requestInterval = await _settings.GetAsync(AttachmentsSettings.CleanupRequestsIntervalSeconds, 5);
            logger.LogInformation("Checking files older than " + createdBefore.ToString("hh:mm:ss t"));

            if (await repository.AnyAsync(e => e.CreationTime < createdBefore))
            {

                List<TempFile> files = await repository.GetCleanUpFiles(createdBefore, 200);
                logger.LogInformation($"Found {files.Count} files");
                foreach (var f in files)
                {
                    logger.LogInformation($"Deleting Temp file {f.Id}");
                    try
                    {
                        if (f.FullPath != null)
                        {
                            var container = containerFactory.GetContainer(f.AttachmentCategory.ContainerName);
                            await container.DeleteAsync(f.FullPath);
                        }
                        logger.LogInformation($"Deleting Temp file {f.Id} success");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Deleting Temp file {f.Id} failed");
                        logger.LogException(ex, LogLevel.Error);
                    }

                    await repository.DeleteAsync(f);
                    if (requestInterval > 0)
                        Thread.Sleep(requestInterval);
                }
            }
            else
            {
                logger.LogInformation("Found 0 files");
            }
        }
    }
}
