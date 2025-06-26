using Application.DTOs;
using Application.DTOs.Chatting;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;

namespace Application.Services.Chatting
{
    public class AIWebhookService : IAIWebhookService
    {        private readonly IMessageRepository _messageRepository;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IUserRepository _userRepository;

        public AIWebhookService(
            IMessageRepository messageRepository,
            IChatNotificationService chatNotificationService,
            IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IWebHostEnvironment env,
            IUserRepository userRepository)        {
            _messageRepository = messageRepository;
            _chatNotificationService = chatNotificationService;
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _env = env;
            _userRepository = userRepository;
        }        public async Task<ResponseViewModel<string>> HandleVoiceTranslationWebhookAsync(AIWebhookInferencePayloadDto payload)
        {
            if (payload.Success != "true")
            {
                return new ResponseViewModel<string>("", $"Voice translation failed: {payload.ErrorMessage}", false, StatusCodes.Status400BadRequest);
            }

            try
            {
                var taskId = payload.RequestId;
                var message = _memoryCache.Get<VoiceMessage>(taskId);
                if (message == null)
                {
                    return new ResponseViewModel<string>("", "Message not found in cache", false, StatusCodes.Status404NotFound);
                }

                // Get config values
                string aiBaseLink = _configuration["AIBaseLink"] ?? "";
                string aiJwtSecret = _configuration["AIJWTSecret"] ?? "";
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", aiJwtSecret);

                // GET request to fetch the audio result
                var resultUrl = $"{aiBaseLink}/voice/result/{taskId}";
                var resultResponse = await client.GetAsync(resultUrl);

                if (!resultResponse.IsSuccessStatusCode)
                {
                    return new ResponseViewModel<string>("", "Failed to fetch translated audio file", false, (int)resultResponse.StatusCode);
                }

                // Determine file extension from content type
                var contentType = resultResponse.Content.Headers.ContentType?.MediaType;
                
                string fileExt = contentType switch
                {
                    "audio/wav" => ".wav",
                    "audio/mp4" => ".m4a",
                    _ => ".dat"
                };

                // Save the file to a location in wwwroot/translated_audio/
                var fileName = $"{Guid.NewGuid()}{fileExt}";
                var saveDir = Path.Combine(_env.WebRootPath, "translated_audio");
                Directory.CreateDirectory(saveDir);
                var savePath = Path.Combine(saveDir, fileName);

                using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    await resultResponse.Content.CopyToAsync(fs);
                }
                message.IsTranslated = true;
                message.TranslatedVoiceUrl = $"/translated_audio/{fileName}";

                // Check if message exists in database, if not add it, otherwise update it
                var existingMessage = await _messageRepository.GetByIdAsync(message.Id);
                if (existingMessage == null)
                {
                    // Message was only cached, save it to database now
                    await _messageRepository.AddAsync(message);
                }
                else
                {
                    // Message exists, update it with translation details
                    await _messageRepository.UpdateAsync(message);
                }
                
                // **IMPORTANT: Send TWO notifications for translation completion**
                
                // 1. Notify that the message translation is complete (clients can update UI)
                await _chatNotificationService.NotifyMessageTranslated(
                    message.ConversationId, 
                    message.Id, 
                    message.ConversationUser?.UserId ?? "Unknown",
                    payload.RequestId // Using requestId as context, could be target language
                );
                
                // 2. Send updated message with translated voice URL (clients get the new file URL)
                await _chatNotificationService.NotifyMessageSent(message.ConversationId, message);

                // Return the relative path or URL to the saved file
                var relativePath = $"/translated_audio/{fileName}";
                return new ResponseViewModel<string>(relativePath, "Voice translation processed and file saved successfully", true, StatusCodes.Status200OK);            }
            catch (Exception ex)
            {
                return new ResponseViewModel<string>("", $"Error processing voice translation: {ex.Message}", false, StatusCodes.Status500InternalServerError);
            }
        }public async Task<ResponseViewModel<string>> HandleVoiceTrainingWebhookAsync(AIWebhookTrainingPayloadDto payload)
        {
            try
            {
                // Find the user by the task ID (stored in VoiceModel.Id)
                var users = await _userRepository.GetAllUsersAsync();
                var user = users.FirstOrDefault(u => u.VoiceModel != null && u.VoiceModel.Id == payload.RequestId);
                
                if (user == null)
                {
                    return new ResponseViewModel<string>("", "User not found for this training task", false, StatusCodes.Status404NotFound);
                }

                if (payload.Success)
                {
                    // Training completed successfully
                    user.IsTrainedVoice = true;
                    if (user.VoiceModel != null)
                    {
                        user.VoiceModel.Name = payload.modelId; // Update with the final model ID from AI service
                    }
                    
                    await _userRepository.UpdateUserAsync(user);
                    
                    // Notify the user via SignalR
                    await _chatNotificationService.NotifyVoiceTrainingCompleted(user.Id, true, payload.modelId);
                    
                    return new ResponseViewModel<string>("Training completed successfully", "Voice training completed and user notified", true, StatusCodes.Status200OK);
                }
                else
                {
                    // Training failed
                    user.IsTrainedVoice = false;
                    user.VoiceModel = null!; // Remove the failed training attempt
                    
                    await _userRepository.UpdateUserAsync(user);
                    
                    // Notify the user of the failure
                    await _chatNotificationService.NotifyVoiceTrainingCompleted(user.Id, false, payload.modelId, payload.ErrorMessage);
                    
                    return new ResponseViewModel<string>("Training failed", $"Voice training failed: {payload.ErrorMessage}", false, StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception ex)
            {
                return new ResponseViewModel<string>("", $"Error processing voice training webhook: {ex.Message}", false, StatusCodes.Status500InternalServerError);
            }
        }
    }
}