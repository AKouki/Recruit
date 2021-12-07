namespace Recruit.Server.Services.BlobService
{
    public interface IBlobService
    {
        Task<byte[]> GetAsync(string blobName);
        Task<bool> UploadAsync(IFormFile file, string containerName, string blobName, string contentType);
        Task<bool> UploadResumeAsync(IFormFile file, string blobName);
        Task<bool> UploadPhotoAsync(IFormFile file, string blobName);
        Task<string> CopyBlobAsync(string blobName);
        Task DeleteAsync(string container, string blobName);
        Task DeleteResumeAsync(string blobName);
        Task DeletePhotoAsync(string blobName);

    }
}
