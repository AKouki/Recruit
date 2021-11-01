using Microsoft.AspNetCore.Http;

namespace Recruit.Shared.Validators
{
    public class FileValidator : IFileValidator
    {
        public bool IsValidFile(IFormFile? file, int maxAllowedFileSize, string allowedFileExtensions)
        {
            if (file != null)
            {
                string[] AllowedExtenstions = allowedFileExtensions.Split(',');

                if (file.Length > maxAllowedFileSize)
                    return false;

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !AllowedExtenstions.Any(s => s.Contains(extension)))
                    return false;

                if (file.FileName.Length > 255)
                    return false;

                return true;
            }

            return false;
        }

        public bool IsValidPhoto(IFormFile? file)
        {
            return IsValidFile(file, 1 * 1024 * 1024, ".jpg,.jpeg");
        }

        public bool IsValidResume(IFormFile? file)
        {
            return IsValidFile(file, 2 * 1024 * 1024, ".pdf");
        }
    }
}
