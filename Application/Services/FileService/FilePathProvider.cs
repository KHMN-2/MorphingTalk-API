using Application.Interfaces.Services.FileService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Application.Services.FileService
{
    public class FilePathProvider : IFilePathProvider
    {
        private readonly IWebHostEnvironment _environment;

        public FilePathProvider(IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public string GetWebRootPath()
        {
            return string.IsNullOrWhiteSpace(_environment.WebRootPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                : _environment.WebRootPath;
        }

        public string BuildFilePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            var uploadsIndex = relativePath.IndexOf(FileServiceConfig.UploadsFolder, StringComparison.OrdinalIgnoreCase);
            if (uploadsIndex == -1)
            {
                throw new FileNotFoundException("Invalid path, 'uploads' directory not found.");
            }

            var cleanRelativePath = relativePath.Substring(uploadsIndex);
            return Path.Combine(GetWebRootPath(), cleanRelativePath);
        }

        public string BuildUploadPath(IFormFile file, string subFolder, string fileNameUser = null)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            var basePath = Path.Combine(GetWebRootPath(), FileServiceConfig.UploadsFolder);
            var path = fileNameUser != null
                ? Path.Combine(basePath, subFolder, fileNameUser)
                : Path.Combine(basePath, subFolder);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            return Path.Combine(path, fileName);
        }

        public string GetRelativePath(string fullPath)
        {
            var webRootPath = GetWebRootPath();
            return fullPath
                .Replace(webRootPath, "")
                .Replace("\\", "/")
                .TrimStart('/');
        }
    }
}