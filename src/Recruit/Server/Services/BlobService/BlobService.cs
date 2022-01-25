using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace Recruit.Server.Services.BlobService
{
    public class BlobService : IBlobService
    {
        private readonly ILogger<BlobService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;

        private readonly string ResumesContainerName;
        private readonly string PhotosContainerName;

        public BlobService(
            ILogger<BlobService> logger,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            _logger = logger;
            _env = env;
            _configuration = configuration;
            _blobServiceClient = new BlobServiceClient(_configuration["AzureBlobStorageSettings:ConnectionString"]);

            ResumesContainerName = _configuration["AzureBlobStorageSettings:ResumesContainerName"];
            PhotosContainerName = _configuration["AzureBlobStorageSettings:PhotosContainerName"];

            CreateContainersIfNotExists();
        }

        private void CreateContainersIfNotExists()
        {
            BlobContainerClient resumesBlobContainer = _blobServiceClient
                .GetBlobContainerClient(ResumesContainerName);

            resumesBlobContainer.CreateIfNotExists();

            BlobContainerClient photosBlobContainer = _blobServiceClient
                .GetBlobContainerClient(PhotosContainerName);

            photosBlobContainer.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<byte[]> GetAsync(string blobName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient
                    .GetBlobContainerClient(ResumesContainerName);

            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            using var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream);

            return stream.ToArray();
        }

        public async Task<bool> UploadAsync(IFormFile file, string containerName, string blobName, string contentType)
        {
            try
            {
                BlobContainerClient blobContainerClient = _blobServiceClient
                    .GetBlobContainerClient(containerName);

                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, new BlobHttpHeaders() { ContentType = contentType });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading {file.FileName} to blob storage.", ex.Message);
                return false;
            }
        }

        public Task<bool> UploadResumeAsync(IFormFile file, string blobName)
        {
            return UploadAsync(file, ResumesContainerName, blobName, "application/pdf");
        }

        public Task<bool> UploadPhotoAsync(IFormFile file, string blobName)
        {
            return UploadAsync(file, PhotosContainerName, blobName, "image/jpeg");
        }

        public async Task<string> CopyBlobAsync(string blobName)
        {
            try
            {
                BlobContainerClient blobContainerClient = _blobServiceClient
                    .GetBlobContainerClient(ResumesContainerName);

                BlobClient sourceBlob = blobContainerClient.GetBlobClient(blobName);

                if (await sourceBlob.ExistsAsync())
                {
                    // Acquire an infinite lease
                    BlobLeaseClient lease = sourceBlob.GetBlobLeaseClient();
                    await lease.AcquireAsync(TimeSpan.FromSeconds(-1));

                    var newBlobName = Guid.NewGuid().ToString("N") + Path.GetExtension(sourceBlob.Name);
                    BlobClient destBlob = blobContainerClient.GetBlobClient(newBlobName);

                    // Start copy
                    await destBlob.StartCopyFromUriAsync(sourceBlob.Uri);

                    // Break the lease on source blob
                    await lease.BreakAsync();

                    return newBlobName;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error copying blob: {blobName}.", ex.Message);
            }

            return string.Empty;
        }

        public async Task DeleteAsync(string containerName, string blobName)
        {
            try
            {
                BlobContainerClient blobContainerClient = _blobServiceClient
                    .GetBlobContainerClient(containerName);

                BlobClient blobClient = blobContainerClient.GetBlobClient(Path.GetFileName(blobName).Split("?")[0]);

                await blobClient.DeleteIfExistsAsync();
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.ContainerBeingDeleted ||
                ex.ErrorCode == BlobErrorCode.ContainerNotFound)
            {
                _logger.LogError($"Error deleting {blobName}. Container {containerName} is beign deleted or not found.", ex.Message);
            }
        }

        public async Task DeleteResumeAsync(string blobName)
        {
            await DeleteAsync(ResumesContainerName, blobName);
        }

        public async Task DeletePhotoAsync(string blobName)
        {
            await DeleteAsync(PhotosContainerName, blobName);
        }

        public async Task DeleteAllAsync(string container, List<string> blobNames)
        {
            try
            {
                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(container);

                // Since the Azure Storage Emulator does not support batch delete, we must use different approach.
                if (_env.IsDevelopment())
                    await DeleteFromAzureStorageEmulator(blobNames, blobContainerClient);
                else
                    await DeleteFromCloud(blobNames, blobContainerClient);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error batch deleting blobs for container {container}", ex.Message);
            }
        }

        private async Task DeleteFromCloud(List<string> blobNames, BlobContainerClient blobContainerClient)
        {
            var blobUris = new List<Uri>();
            foreach (var blobName in blobNames)
            {
                if (!string.IsNullOrEmpty(blobName))
                {
                    BlobClient blobClient = blobContainerClient.GetBlobClient(Path.GetFileName(blobName).Split("?")[0]);
                    blobUris.Add(blobClient.Uri);
                }
            }

            BlobBatchClient blobBatchClient = _blobServiceClient.GetBlobBatchClient();
            await blobBatchClient.DeleteBlobsAsync(blobUris);
        }
        private async Task DeleteFromAzureStorageEmulator(List<string> blobNames, BlobContainerClient blobContainerClient)
        {
            foreach (var blobName in blobNames)
            {
                if (!string.IsNullOrEmpty(blobName))
                {
                    BlobClient blobClient = blobContainerClient.GetBlobClient(Path.GetFileName(blobName).Split("?")[0]);
                    await blobClient.DeleteIfExistsAsync();
                }
            }
        }

        public async Task DeleteResumesAsync(List<string> blobNames)
        {
            await DeleteAllAsync(ResumesContainerName, blobNames);
        }

        public async Task DeletePhotosAsync(List<string> blobNames)
        {
            await DeleteAllAsync(PhotosContainerName, blobNames);
        }
    }
}
