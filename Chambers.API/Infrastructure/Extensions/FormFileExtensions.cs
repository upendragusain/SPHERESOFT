using Microsoft.AspNetCore.Http;

namespace Chambers.API.Infrastructure.Extensions
{
    public static class FormFileExtensions
    {
        public static bool ValidateFileNameAndExtension(this IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(file.FileName) 
                || !file.FileName.ToLower().EndsWith(".pdf"))
            {
                return false;
            }

            return true;
        }

        public static bool ValidateFileSize(this IFormFile file, long maxFileSize)
        {
            if (file.Length <= 0 || file.Length > maxFileSize)
            {
                return false;
            }

            return true;
        }
    }
}
