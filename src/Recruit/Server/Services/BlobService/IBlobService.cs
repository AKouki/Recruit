namespace Recruit.Server.Services.BlobService
{
    public interface IBlobService
    {
        Task<byte[]> GetAsync(string blobName);
        Task<bool> UploadAsync(IFormFile file, string containerName, string blobName, string contentType);
        Task<bool> UploadResumeAsync(IFormFile file, string blobName);
        Task<bool> UploadPhotoAsync(IFormFile file, string blobName);
        Task<string> CopyBlobAsync(string containerName, string blobName);
        Task<string> CopyResumeAsync(string blobName);
        Task<string> CopyPhotoAsync(string blobName);
        Task DeleteAsync(string container, string blobName);
        Task DeleteResumeAsync(string blobName);
        Task DeletePhotoAsync(string blobName);
        Task DeleteAllAsync(string container, List<string> blobNames);
        Task DeleteResumesAsync(List<string> blobNames);
        Task DeletePhotosAsync(List<string> blobNames);

    }
}
