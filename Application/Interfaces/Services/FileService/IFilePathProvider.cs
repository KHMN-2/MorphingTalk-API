using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services.FileService
{
    public interface IFilePathProvider
    {
        string GetWebRootPath();
        string BuildFilePath(string relativePath);
        string BuildUploadPath(IFormFile file, string subFolder, string userId = null);
        string GetRelativePath(string fullPath);
    }
}
