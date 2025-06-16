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
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public AIWebhookService(
            IMessageRepository messageRepository,
            IChatNotificationService chatNotificationService,
            IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _messageRepository = messageRepository;
            _chatNotificationService = chatNotificationService;
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _env = env;
        }

        public async Task<ResponseViewModel<string>> HandleVoiceTranslationWebhookAsync(AIWebhookInferencePayloadDto payload)
        {
            if (!payload.Success)
            {
                return new ResponseViewModel<string>(null, $"Voice translation failed: {payload.ErrorMessage}", false, StatusCodes.Status400BadRequest);
            }

            try
            {
                var taskId = payload.TaskId;
                var message = _memoryCache.Get<VoiceMessage>(taskId);
                if (message == null)
                {
                    return new ResponseViewModel<string>(null, "Message not found in cache", false, StatusCodes.Status404NotFound);
                }

                // Get config values
                string aiBaseLink = _configuration["AIBaseLink"];
                string aiJwtSecret = _configuration["AIJWTSecret"];
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", aiJwtSecret);

                // GET request to fetch the audio result
                var resultUrl = $"{aiBaseLink}/voice/result/{taskId}";
                var resultResponse = await client.GetAsync(resultUrl);

                if (!resultResponse.IsSuccessStatusCode)
                {
                    return new ResponseViewModel<string>(null, "Failed to fetch translated audio file", false, (int)resultResponse.StatusCode);
                }

                // Determine file extension from content type
                var contentType = resultResponse.Content.Headers.ContentType?.MediaType;
                string fileExt = contentType switch
                {
                    "audio/wav" => ".wav",
                    "audio/m4a" => ".m4a",
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
                // Here you can update your message or database with the new file path if needed
                message.TranslatedVoiceUrl = $"/translated_audio/{fileName}";

                // Save the message to the repository
                await _messageRepository.AddAsync(message);
                // Notify connected clients about the new translated voice message
                await _chatNotificationService.NotifyMessageSent(message.ConversationId, message);

                // Return the relative path or URL to the saved file
                var relativePath = $"/translated_audio/{fileName}";
                return new ResponseViewModel<string>(relativePath, "Voice translation processed and file saved successfully", true, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ResponseViewModel<string>(null, $"Error processing voice translation: {ex.Message}", false, StatusCodes.Status500InternalServerError);
            }
        }
    }
} 