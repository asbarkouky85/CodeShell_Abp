using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Aws;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;
using Codeshell.Abp.Extensions;
using Codeshell.Abp.Attachments.BlobStoring;

namespace Volo.Abp.Attachments.BlobStoring
{
    public class AwsChunkUploader : AwsBlobProvider, IChunkUploader, ITransientDependency
    {
        protected string ContainerName;
        protected BlobContainerConfiguration Configuration;
        ICancellationTokenProvider CancellationTokenProvider;
        public AwsChunkUploader(
            IAwsBlobNameCalculator awsBlobNameCalculator,
            IAmazonS3ClientFactory amazonS3ClientFactory,
            IBlobNormalizeNamingService blobNormalizeNamingService,
            ICancellationTokenProvider cancellationTokenProvider) :
            base(awsBlobNameCalculator, amazonS3ClientFactory, blobNormalizeNamingService)
        {
            CancellationTokenProvider = cancellationTokenProvider;
        }

        public void SetUp(string containerName, BlobContainerConfiguration configuration)
        {
            ContainerName = containerName;
            Configuration = configuration;
        }

        public string NormalizeName(string name)
        {
            return name.GetBeforeLast(".");
        }

        public async Task<string> StartFile(string name, CancellationToken cancellationToken = default)
        {

            BlobNormalizeNaming blobNormalizeNaming = BlobNormalizeNamingService.NormalizeNaming(Configuration, ContainerName, name);
            var args = new BlobProviderGetArgs(blobNormalizeNaming.ContainerName, Configuration, blobNormalizeNaming.BlobName, CancellationTokenProvider.FallbackToProvider(cancellationToken));
            var cl = await GetAmazonS3Client(args);
            string blobName = AwsBlobNameCalculator.Calculate(args);
            AwsBlobProviderConfiguration configuration = args.Configuration.GetAwsConfiguration();
            string containerName = GetContainerName(args);
            using AmazonS3Client amazonS3Client = await GetAmazonS3Client(args).ConfigureAwait(continueOnCapturedContext: false);

            if (configuration.CreateContainerIfNotExists)
            {
                await CreateContainerIfNotExists(amazonS3Client, containerName).ConfigureAwait(continueOnCapturedContext: false);
            }

            var init = await cl.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest
            {
                BucketName = containerName,
                Key = blobName
            });
            return init.UploadId;
        }

        public async Task<string> UploadPart(string name, string uploadId, int partNumber, byte[] bytes, CancellationToken cancellationToken = default)
        {
            using Stream stream = new MemoryStream(bytes);

            BlobNormalizeNaming blobNormalizeNaming = BlobNormalizeNamingService.NormalizeNaming(Configuration, ContainerName, name);
            var args = new BlobProviderSaveArgs(blobNormalizeNaming.ContainerName, Configuration, blobNormalizeNaming.BlobName, stream, true, CancellationTokenProvider.FallbackToProvider(cancellationToken));

            var cl = await GetAmazonS3Client(args);
            string blobName = AwsBlobNameCalculator.Calculate(args);
            AwsBlobProviderConfiguration configuration = args.Configuration.GetAwsConfiguration();
            string containerName = GetContainerName(args);
            using AmazonS3Client amazonS3Client = await GetAmazonS3Client(args).ConfigureAwait(continueOnCapturedContext: false);

            if (configuration.CreateContainerIfNotExists)
            {
                await CreateContainerIfNotExists(amazonS3Client, containerName).ConfigureAwait(continueOnCapturedContext: false);
            }

            var init = await cl.UploadPartAsync(new UploadPartRequest
            {
                BucketName = containerName,
                Key = blobName,
                InputStream = stream,
                PartNumber = partNumber,
                PartSize = bytes.Length,
                UploadId = uploadId,
            });
            return init.ETag;
        }

        public async Task Finish(string name, string uploadId, Dictionary<int, string> partIds, CancellationToken cancellationToken = default)
        {
            BlobNormalizeNaming blobNormalizeNaming = BlobNormalizeNamingService.NormalizeNaming(Configuration, ContainerName, name);
            var args = new BlobProviderGetArgs(blobNormalizeNaming.ContainerName, Configuration, blobNormalizeNaming.BlobName, CancellationTokenProvider.FallbackToProvider(cancellationToken));

            var cl = await GetAmazonS3Client(args);
            string blobName = AwsBlobNameCalculator.Calculate(args);
            AwsBlobProviderConfiguration configuration = args.Configuration.GetAwsConfiguration();
            string containerName = GetContainerName(args);
            using AmazonS3Client amazonS3Client = await GetAmazonS3Client(args).ConfigureAwait(continueOnCapturedContext: false);

            if (configuration.CreateContainerIfNotExists)
            {
                await CreateContainerIfNotExists(amazonS3Client, containerName).ConfigureAwait(continueOnCapturedContext: false);
            }

            List<PartETag> parts = new List<PartETag>();
            foreach (var t in partIds)
            {
                parts.Add(new PartETag { ETag = t.Value, PartNumber = t.Key });
            }

            var init = await cl.CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest
            {
                BucketName = containerName,
                Key = blobName,
                UploadId = uploadId,
                PartETags = parts
            });
        }


    }
}
