namespace Application.Services.FileService
{
    public static class FileServiceConfig
    {
        public static readonly List<string> ValidDocumentExtensions = new() { ".pdf" };
        public static readonly List<string> ValidImageExtensions = new() { ".jpg", ".jpeg", ".png" };
        public static readonly List<string> ValidVideoExtensions = new() { ".mp4", ".mov", ".avi", ".wmv", ".mkv" };
        public static readonly List<string> ValidAudioExtensions = new() { ".mp3", ".wav", ".ogg", ".m4a" };

        public static readonly List<string> ValidExtensions =
            ValidDocumentExtensions
            .Concat(ValidImageExtensions)
            .Concat(ValidVideoExtensions)
            .ToList();

        public const long MaxDocumentSize = 5 * 1024 * 1024; // 5MB for PDFs
        public const long MaxImageSize = 5 * 1024 * 1024; // 5MB for images
        public const long MaxVideoSize = 50 * 1024 * 1024; // 50MB for videos
        public const long MaxAudioSize = 10 * 1024 * 1024; // 10MB for audio files


        public const string UploadsFolder = "Uploads";
        public const string DocsFolder = "docs";
        public const string ImagesFolder = "images";
        public const string VideosFolder = "videos";
        public const string AudiosFolder = "audios";
        public const string DefaultContentType = "application/octet-stream";
    }
}