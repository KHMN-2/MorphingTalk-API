using Microsoft.AspNetCore.Http;
namespace Application.Interfaces.Services.FileService
{
    public interface IFileValidator
    {
        void ValidateFile(IFormFile file);
        void ValidateDocument(IFormFile file);
        void ValidateImage(IFormFile file);
        void ValidateVideo(IFormFile file);
    }
}

