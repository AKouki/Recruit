using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Recruit.Server.Services.BlobService
{
    public class BlobService : IBlobService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BlobService> _logger;
        private readonly BlobServiceClient _blobServiceClient;

        private readonly string ResumesContainerName;
        private readonly string PhotosContainerName;

        public BlobService(ILogger<BlobService> logger, 
            IConfiguration configuration)
        {
            _logger = logger;
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

        public Task DeleteResumeAsync(string blobName)
        {
            return DeleteAsync(ResumesContainerName, blobName);
        }

        public Task DeletePhotoAsync(string blobName)
        {
            return DeleteAsync(PhotosContainerName, blobName);
        }
    }
}
