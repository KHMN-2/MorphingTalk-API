using Application.DTOs;
using Application.DTOs.Chatting;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.AIModels;
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
using Application.Services.Chatting; // Add this for VoiceTranslationTracker
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<AIWebhookService> _logger;

        public AIWebhookService(
            IMessageRepository messageRepository,
            IChatNotificationService chatNotificationService,
            IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IWebHostEnvironment env,
            IUserRepository userRepository,
            ILogger<AIWebhookService> logger)        {
            _messageRepository = messageRepository;
            _chatNotificationService = chatNotificationService;
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _env = env;
            _userRepository = userRepository;
            _logger = logger;
        }        
        public async Task<ResponseViewModel<string>> HandleVoiceTranslationWebhookAsync(AIWebhookInferencePayloadDto payload)
        {
            if (payload.Success != "true")
            {
                return new ResponseViewModel<string>("", $"Voice translation failed: {payload.ErrorMessage}", false, StatusCodes.Status400BadRequest);
            }

            try
            {
                var taskId = payload.RequestId;
                
                // Get the translation tracker from cache
                var translationTracker = _memoryCache.Get<VoiceTranslationTracker>(taskId);
                if (translationTracker == null)
                {
                    return new ResponseViewModel<string>("", "Translation tracker not found in cache", false, StatusCodes.Status404NotFound);
                }

                // Get the target language for this specific task ID
                var languageCacheKey = $"taskId_language_{taskId}";
                var targetLanguage = _memoryCache.Get<string>(languageCacheKey);
                if (string.IsNullOrEmpty(targetLanguage))
                {
                    return new ResponseViewModel<string>("", "Target language not found for this task", false, StatusCodes.Status404NotFound);
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
                var fileName = $"{translationTracker.MessageId}_{targetLanguage}_{Guid.NewGuid()}{fileExt}";
                var saveDir = Path.Combine(_env.WebRootPath, "translated_audio");
                Directory.CreateDirectory(saveDir);
                var savePath = Path.Combine(saveDir, fileName);

                using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    await resultResponse.Content.CopyToAsync(fs);
                }

                var relativePath = $"/translated_audio/{fileName}";
                
                // Update the translation tracker with this completed translation
                translationTracker.CompletedTranslations[targetLanguage] = relativePath;
                
                // Update the tracker in cache for other pending tasks
                foreach (var otherTaskId in translationTracker.TaskIds)
                {
                    _memoryCache.Set(otherTaskId, translationTracker, TimeSpan.FromMinutes(30));
                }

                // Check if all translations are complete
                if (translationTracker.IsComplete)
                {
                    // All translations are done - get the message from tracker and save it with all translations
                    var message = translationTracker.VoiceMessage;
                    if (message != null)
                    {
                        message.IsTranslated = true;
                        message.TranslatedVoiceUrls = new Dictionary<string, string>(translationTracker.CompletedTranslations);
                        
                        // Set backward compatibility field to first translation
                        message.TranslatedVoiceUrl = translationTracker.CompletedTranslations.Values.FirstOrDefault();

                        // NOW save the message to database (first time)
                        await _messageRepository.AddAsync(message);

                        // Reload the message with navigation properties for notification
                        var savedMessage = await _messageRepository.GetByIdAsync(message.Id);
                        if (savedMessage != null)
                        {
                            // Send notification as if it's a normal message being sent (not a translation update)
                            await _chatNotificationService.NotifyMessageSent(message.ConversationId, savedMessage);
                        }

                        // Clean up cache entries
                        foreach (var cleanupTaskId in translationTracker.TaskIds)
                        {
                            _memoryCache.Remove(cleanupTaskId);
                            _memoryCache.Remove($"taskId_language_{cleanupTaskId}");
                        }

                        // Return the dictionary of all translated voice URLs
                        var result = Newtonsoft.Json.JsonConvert.SerializeObject(translationTracker.CompletedTranslations);
                        return new ResponseViewModel<string>(result, 
                            $"All voice translations completed successfully. Total: {translationTracker.CompletedCount}/{translationTracker.TotalTranslations}", 
                            true, StatusCodes.Status200OK);
                    }
                }
                else
                {
                    // Partial completion - just log progress, don't send notifications yet
                    _logger.LogInformation($"Translation for {targetLanguage} completed. Progress: {translationTracker.CompletedCount}/{translationTracker.TotalTranslations}");

                    return new ResponseViewModel<string>(relativePath, 
                        $"Translation for {targetLanguage} completed. Progress: {translationTracker.CompletedCount}/{translationTracker.TotalTranslations}", 
                        true, StatusCodes.Status200OK);
                }

                return new ResponseViewModel<string>("", "Message not found", false, StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return new ResponseViewModel<string>("", $"Error processing voice translation: {ex.Message}", false, StatusCodes.Status500InternalServerError);
            }
        }
        
        public async Task<ResponseViewModel<string>> HandleVoiceTrainingWebhookAsync(AIWebhookTrainingPayloadDto payload)
        {
            try
            {
                // Find the user by matching the voice model ID with the model ID from payload
                var user = await _userRepository.GetUserByVoiceModelIdAsync(payload.modelId);
                
                if (user == null)
                {
                    return new ResponseViewModel<string>("", "User not found for this training task", false, StatusCodes.Status404NotFound);
                }

                if (payload.Success == "true")
                {
                    // Training completed successfully
                    user.IsTrainedVoice = true;
                    if (user.VoiceModel != null)
                    {
                        user.VoiceModel.Status = UserVoiceModelStatus.Completed;
                        // Keep the original name, don't overwrite with modelId (which is actually userId)
                        // user.VoiceModel.Name remains as originally set: user.FullName + "_" + user.NativeLanguage
                    }
                    
                    await _userRepository.UpdateUserAsync(user);
                    
                    // Notify the user via SignalR with the voice model ID (not the user ID)
                    await _chatNotificationService.NotifyVoiceTrainingCompleted(user.Id, true, user.VoiceModel?.Id ?? payload.modelId);
                    
                    return new ResponseViewModel<string>("Training completed successfully", "Voice training completed and user notified", true, StatusCodes.Status200OK);
                }
                else
                {
                    // Training failed
                    user.IsTrainedVoice = false;
                    if (user.VoiceModel != null)
                    {
                        user.VoiceModel.Status = UserVoiceModelStatus.Failed;
                    }
                    
                    await _userRepository.UpdateUserAsync(user);
                    
                    // Notify the user of the failure with the voice model ID
                    await _chatNotificationService.NotifyVoiceTrainingCompleted(user.Id, false, user.VoiceModel?.Id ?? payload.modelId, payload.ErrorMessage);
                    
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