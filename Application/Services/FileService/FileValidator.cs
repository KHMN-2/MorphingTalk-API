using Application.Interfaces.Services.FileService;
using Microsoft.AspNetCore.Http;

namespace Application.Services.FileService
{
    public class FileValidator : IFileValidator
    {
        public void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentNullException(nameof(file), "No file uploaded.");
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!FileServiceConfig.ValidExtensions.Contains(extension))
            {
                //throw new BadHttpRequestException(
                //    $"File extension {extension} is not valid. Allowed: {string.Join(", ", FileServiceConfig.ValidExtensions)}");
            }

            // Determine the maximum file size based on file type
            long maxFileSize = extension switch
            {
                var ext when FileServiceConfig.ValidDocumentExtensions.Contains(ext) => FileServiceConfig.MaxDocumentSize,
                var ext when FileServiceConfig.ValidImageExtensions.Contains(ext) => FileServiceConfig.MaxImageSize,
                var ext when FileServiceConfig.ValidVideoExtensions.Contains(ext) => FileServiceConfig.MaxVideoSize,
                _ => throw new InvalidOperationException($"Unexpected file extension: {extension}")
            };

            if (file.Length > maxFileSize)
            {
                //throw new BadHttpRequestException(
                //    $"File size exceeds {maxFileSize / (1024 * 1024)}MB limit for {GetFileTypeName(extension)}.");
            }
        }

        public void ValidateDocument(IFormFile file)
        {
            //ValidateFile(file);
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!FileServiceConfig.ValidDocumentExtensions.Contains(extension))
            {
                throw new BadHttpRequestException(
                   $"File is not a valid document. Allowed extensions: {string.Join(", ", FileServiceConfig.ValidDocumentExtensions)}");
            }
            if (file.Length > FileServiceConfig.MaxDocumentSize)
            {
                throw new BadHttpRequestException(
                    $"Document size exceeds {FileServiceConfig.MaxDocumentSize / (1024 * 1024)}MB limit.");
            }
        }

        public void ValidateImage(IFormFile file)
        {
            // ValidateFile(file);
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!FileServiceConfig.ValidImageExtensions.Contains(extension))
            {
                throw new BadHttpRequestException(
                    $"File is not a valid image. Allowed extensions: {string.Join(", ", FileServiceConfig.ValidImageExtensions)}");
            }
            if (file.Length > FileServiceConfig.MaxImageSize)
            {
                throw new BadHttpRequestException(
                    $"Image size exceeds {FileServiceConfig.MaxImageSize / (1024 * 1024)}MB limit.");
            }
        }

        public void ValidateVideo(IFormFile file)
        {
            //ValidateFile(file);
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!FileServiceConfig.ValidVideoExtensions.Contains(extension))
            {
                throw new BadHttpRequestException(
                    $"File is not a valid video. Allowed extensions: {string.Join(", ", FileServiceConfig.ValidVideoExtensions)}");
            }
            if (file.Length > FileServiceConfig.MaxVideoSize)
            {
                throw new BadHttpRequestException(
                   $"Video size exceeds {FileServiceConfig.MaxVideoSize / (1024 * 1024)}MB limit.");
            }
        }

        private static string GetFileTypeName(string extension)
        {
            return extension switch
            {
                var ext when FileServiceConfig.ValidDocumentExtensions.Contains(ext) => "documents",
                var ext when FileServiceConfig.ValidImageExtensions.Contains(ext) => "images",
                var ext when FileServiceConfig.ValidVideoExtensions.Contains(ext) => "videos",
                _ => "files"
            };
        }
    }
}