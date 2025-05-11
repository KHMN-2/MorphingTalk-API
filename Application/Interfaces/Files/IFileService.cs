using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces.Services.Files
{
    public interface IFileService
    {
        public Task<string> UploadFileAsync(IFormFile file, string token);
        public Task<IActionResult> DownloadFileAsync(string relativePath);
        public bool DeleteFileAsync(string relativePath);
        Task<string> UploadFileWithoutAuthAsync(IFormFile file);
    }
}
