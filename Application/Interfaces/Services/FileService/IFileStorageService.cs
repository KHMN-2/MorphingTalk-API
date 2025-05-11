using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services.FileService
{
    public interface IFileStorageService
    {
        Task<string> UploadDocumentAsync(IFormFile file, string fileName);
        Task<string> UploadImageAsync(IFormFile file, string fileName);
        Task<string> UploadVideoAsync(IFormFile file, string fileName);
        Task<string> UploadDocumentWithoutAuthAsync(IFormFile file);
        Task<string> UploadImageWithoutAuthAsync(IFormFile file);
        Task<string> UploadVideoWithoutAuthAsync(IFormFile file);
        // Task<IActionResult> DownloadFileAsync(string relativePath);
        bool DeleteFile(string relativePath);
    }

}
