using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Codeshell.Abp.Attachments
{
    public class TempFileService : ITempFileService
    {
        private readonly ITempFileRepository repository;
        private readonly IBlobContainerFactory containerFactory;
        private readonly ILogger<TempFileService> logger;

        public TempFileService(ITempFileRepository repository, IBlobContainerFactory containerFactory, ILogger<TempFileService> logger)
        {
            this.repository = repository;
            this.containerFactory = containerFactory;
            this.logger = logger;
        }

        [UnitOfWork]
        public virtual async Task CleanUp(DateTime createdBefore)
        {
            logger.LogInformation("Checking files older than " + createdBefore.ToString("hh:mm:ss t"));
            if (await repository.AnyAsync(e => e.CreationTime < createdBefore))
            {
                var tmpContainer = containerFactory.GetTempContainer();
                List<TempFile> files = await repository.GetFilesBefore(createdBefore);
                logger.LogInformation($"Found {files.Count} files");
                foreach (var f in files)
                {
                    try
                    {
                        logger.LogInformation($"Deleting Temp file {f.Id}");
                        await tmpContainer.DeleteAsync(f.GetBlobName());
                        await repository.DeleteAsync(f);
                        logger.LogInformation($"Deleting Temp file {f.Id} success");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Deleting Temp file {f.Id} failed");
                        logger.LogException(ex, LogLevel.Error);
                    }

                }
            }
            else
            {
                logger.LogInformation("Found 0 files");
            }
        }
    }
}
