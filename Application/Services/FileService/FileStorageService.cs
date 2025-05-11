using Application.Interfaces.Services.Authentication;
using Application.Interfaces.Services.FileService;
using Microsoft.AspNetCore.Http;


namespace Application.Services.FileService
{

    public class FileStorageService : IFileStorageService
    {
        private readonly IFileValidator _fileValidator;
        private readonly IFilePathProvider _filePathProvider;
        private readonly ITokenService tokenService;

        public FileStorageService(
            IFileValidator fileValidator,
            IFilePathProvider filePathProvider,
            ITokenService tokenService)


        {
            _fileValidator = fileValidator ?? throw new ArgumentNullException(nameof(fileValidator));
            _filePathProvider = filePathProvider ?? throw new ArgumentNullException(nameof(filePathProvider));
            this.tokenService = tokenService;
        }

        public async Task<string> UploadDocumentAsync(IFormFile file, string token)
        {
            var user = await tokenService.GetUserFromToken(token);
            return await UploadFileAsync(file, user.Id, FileServiceConfig.DocsFolder, _fileValidator.ValidateDocument);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string token)
        {
            var user = await tokenService.GetUserFromToken(token);
            return await UploadFileAsync(file, user.Id, FileServiceConfig.ImagesFolder, _fileValidator.ValidateImage);
        }

        public async Task<string> UploadVideoAsync(IFormFile file, string token)
        {
            var user = await tokenService.GetUserFromToken(token);
            return await UploadFileAsync(file, user.Id, FileServiceConfig.VideosFolder, _fileValidator.ValidateVideo);
        }

        public async Task<string> UploadDocumentWithoutAuthAsync(IFormFile file)
        {
            return await UploadFileWithoutAuthAsync(file, FileServiceConfig.DocsFolder, _fileValidator.ValidateDocument);
        }

        public async Task<string> UploadImageWithoutAuthAsync(IFormFile file)
        {
            return await UploadFileWithoutAuthAsync(file, FileServiceConfig.ImagesFolder, _fileValidator.ValidateImage);
        }

        public async Task<string> UploadVideoWithoutAuthAsync(IFormFile file)
        {
            return await UploadFileWithoutAuthAsync(file, FileServiceConfig.VideosFolder, _fileValidator.ValidateVideo);
        }


        public bool DeleteFile(string relativePath)
        {
            var filePath = _filePathProvider.BuildFilePath(relativePath);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.");
            }

            try
            {
                File.Delete(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> UploadFileAsync(IFormFile file, string fileName, string subFolder, Action<IFormFile> validate)
        {
            validate(file);

            var fullPath = _filePathProvider.BuildUploadPath(file, fileName, subFolder);
            await SaveFileAsync(file, fullPath);

            var relativePath = _filePathProvider.GetRelativePath(fullPath);

            return relativePath;
        }

        private async Task<string> UploadFileWithoutAuthAsync(IFormFile file, string subFolder, Action<IFormFile> validate)
        {
            validate(file);

            var fullPath = _filePathProvider.BuildUploadPath(file, subFolder);
            await SaveFileAsync(file, fullPath);

            return _filePathProvider.GetRelativePath(fullPath);
        }

        private static async Task SaveFileAsync(IFormFile file, string fullPath)
        {
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
        }
    }

}
