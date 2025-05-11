using Application.Interfaces.Services.FileService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MorphingTalk_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        public FileController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        }

        [HttpPost("document")]
        [Authorize]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var relativePath = await _fileStorageService.UploadDocumentAsync(file, token);
            return new JsonResult(new
            {
                success = true,
                status = StatusCodes.Status200OK,
                message = "Added successfully",
                data = relativePath
            });
        }

        [HttpPost("image")]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var relativePath = await _fileStorageService.UploadImageAsync(file, token);
            return new JsonResult(new
            {
                success = true,
                status = StatusCodes.Status200OK,
                message = "Added successfully",
                data = relativePath
            });
        }

        [HttpPost("video")]
        [Authorize]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var relativePath = await _fileStorageService.UploadVideoAsync(file, token);
            return new JsonResult(new
            {
                success = true,
                status = StatusCodes.Status200OK,
                message = "Added successfully",
                data = relativePath
            });
        }

        [HttpPost("public/document")]
        public async Task<IActionResult> UploadDocumentWithoutAuth(IFormFile file)
        {
            try
            {
                var relativePath = await _fileStorageService.UploadDocumentWithoutAuthAsync(file);
                return Ok(new { RelativePath = relativePath });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while uploading the document.");
            }
        }

        [HttpPost("public/image")]
        public async Task<IActionResult> UploadImageWithoutAuth(IFormFile file)
        {
            try
            {
                var relativePath = await _fileStorageService.UploadImageWithoutAuthAsync(file);
                return Ok(new { RelativePath = relativePath });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while uploading the image.");
            }
        }

        [HttpPost("public/video")]
        public async Task<IActionResult> UploadVideoWithoutAuth(IFormFile file)
        {
            try
            {
                var relativePath = await _fileStorageService.UploadVideoWithoutAuthAsync(file);
                return Ok(new { RelativePath = relativePath });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while uploading the video.");
            }
        }



        [HttpDelete("{*relativePath}")]
        [Authorize]
        public IActionResult DeleteFile(string relativePath)
        {
            try
            {
                var success = _fileStorageService.DeleteFile(relativePath);
                if (success)
                {
                    return NoContent();
                }
                return StatusCode(500, "Failed to delete the file.");
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the file.");
            }
        }
    }
}