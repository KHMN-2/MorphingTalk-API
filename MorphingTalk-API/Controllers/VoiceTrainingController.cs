using Application.DTOs;
using Application.DTOs.Chatting;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.AIModels;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VoiceTrainingController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IChatNotificationService _chatNotificationService;

        public VoiceTrainingController(
            IUserRepository userRepository,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IChatNotificationService chatNotificationService)
        {
            _userRepository = userRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _chatNotificationService = chatNotificationService;
        }

        [HttpPost("train")]
        public async Task<IActionResult> TrainVoiceModel(IFormFile file)
        {
            try
            {
                // Get the authenticated user
                string? userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }                // Validate required parameters
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new ResponseViewModel<string>("", "Audio file is required", false, StatusCodes.Status400BadRequest));
                }

                // Use defaults if not provided

                // Validate file type
                var allowedExtensions = new[] { ".wav", ".m4a", ".mp3", ".flac" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new ResponseViewModel<string>("", "Unsupported audio format. Supported formats: wav, m4a, mp3, flac", false, StatusCodes.Status400BadRequest));
                }

                // Get AI service configuration
                string aiBaseLink = _configuration["AIBaseLink"] ?? "";
                string aiJwtSecret = _configuration["AIJWTSecret"] ?? "";

                if (string.IsNullOrEmpty(aiBaseLink) || string.IsNullOrEmpty(aiJwtSecret))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, 
                        new ResponseViewModel<string>("", "AI service configuration missing", false, StatusCodes.Status500InternalServerError));
                }

                // Prepare the HTTP client
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", aiJwtSecret);

                // Prepare multipart form data
                using var form = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                
                // Set content type based on file extension
                string mimeType = fileExtension switch
                {
                    ".wav" => "audio/wav",
                    ".m4a" => "audio/m4a",
                    ".mp3" => "audio/mpeg",
                    ".flac" => "audio/flac",
                    _ => "application/octet-stream"
                };
                
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                form.Add(fileContent, "file", file.FileName);
                form.Add(new StringContent(userId), "model_id");
                form.Add(new StringContent(user.NativeLanguage ?? "en"), "speaker_lang");

                // Send training request to AI service
                var url = $"{aiBaseLink}/voice/train";
                var response = await client.PostAsync(url, form);                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, 
                        new ResponseViewModel<string>("", $"AI service error: {errorContent}", false, (int)response.StatusCode));
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseString);
                var taskId = json["task_id"]?.ToString();

                if (string.IsNullOrEmpty(taskId))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new ResponseViewModel<string>("", "AI service did not return a task_id", false, StatusCodes.Status500InternalServerError));
                }

                // Update user to indicate training is in progress
                user.IsTrainedVoice = false; // Training in progress
                user.VoiceModel = new UserVoiceModel
                {
                    TaskId = taskId,
                    Name = user.FullName + "_" + user.NativeLanguage,
                    CreatedAt = DateTime.UtcNow,
                    Status = UserVoiceModelStatus.Training
                };

                await _userRepository.UpdateUserAsync(user);

                return Ok(new ResponseViewModel<object>(
                    new { 
                        taskId = taskId, 
                        name = user.VoiceModel.Name,
                        status = "training_started",
                        message = "Voice training started successfully. You will be notified when training is complete."
                    }, 
                    "Voice training started successfully", 
                    true, 
                    StatusCodes.Status200OK));            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseViewModel<string>("", $"Internal server error: {ex.Message}", false, StatusCodes.Status500InternalServerError));
            }
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetTrainingStatus()
        {
            try
            {
                string? userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var status = new
                {
                    isTrainedVoice = user.IsTrainedVoice,
                    voiceModel = user.VoiceModel != null ? new
                    {
                        id = user.VoiceModel.Id,
                        name = user.VoiceModel.Name,
                        createdAt = user.VoiceModel.CreatedAt,
                        status = user.VoiceModel.Status
                    } : null,
                    status = user.IsTrainedVoice ? "completed" : 
                            user.VoiceModel != null ? "training" : "not_started"
                };

                return Ok(new ResponseViewModel<object>(status, "Training status retrieved successfully", true, StatusCodes.Status200OK));            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseViewModel<string>("", $"Internal server error: {ex.Message}", false, StatusCodes.Status500InternalServerError));
            }
        }
    }
}
