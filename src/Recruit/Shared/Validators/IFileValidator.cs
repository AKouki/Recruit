using Microsoft.AspNetCore.Http;

namespace Recruit.Shared.Validators
{
    public interface IFileValidator
    {
        bool IsValidFile(IFormFile? file, int maxAllowedFileSize, string allowedFileExtensions);
        bool IsValidResume(IFormFile? file);
        bool IsValidPhoto(IFormFile? file);
    }
}
