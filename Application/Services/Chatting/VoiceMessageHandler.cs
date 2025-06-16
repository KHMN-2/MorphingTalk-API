using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chatting;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Microsoft.Extensions.Logging;
using MorphingTalk_API.DTOs.Chatting;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace Application.Services.Chatting
{
    public class VoiceMessageHandler : IMessageHandler
    {
        private readonly ILogger<VoiceMessageHandler> _logger;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public VoiceMessageHandler(
            ILogger<VoiceMessageHandler> logger,
            IMessageRepository messageRepository,
            IUserRepository userRepository,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _env = env;
        }

        public bool CanHandle(MessageType messageType) => messageType == MessageType.Voice;

        public async Task HandleMessageAsync(SendMessageDto message, Guid conversationId, string userId)
        {
            var voiceMessage = new VoiceMessage
            {
                ConversationId = conversationId,
                ConversationUserId = message.SenderConversationUserId,
                VoiceUrl = message.VoiceFileUrl, // Assuming SendMessageDto has this property
                DurationSeconds = 0, // <-- FIXED
                SentAt = DateTime.UtcNow,
                IsTranslated = false,
                TranslatedVoiceUrl = null,
                Status = MessageStatus.Sent,
            };
            if (voiceMessage == null) throw new InvalidOperationException("Invalid message type");

            // Save the message to the repository
            await _messageRepository.AddAsync(voiceMessage);


            // Add text-specific processing logic here
            _logger.LogInformation($"Processing voice message: {voiceMessage.VoiceUrl} with duration {voiceMessage.DurationSeconds} seconds");
            await Task.CompletedTask;
        }

        public async Task<string> HandleTranslationAsync(SendMessageDto message, Guid conversationId, string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            string modelId = (!user.IsTrainedVoice || message.UseRobotVoice == true) ? "DRRamly" : user.VoiceModel.Name;
            string sourceLanguage = user.NativeLanguage;
            string targetLanguage = message.TargetLanguage;

            // Get config values directly
            string aiBaseLink = _configuration["AIBaseLink"];
            string aiJwtSecret = _configuration["AIJWTSecret"];

            // Resolve the physical path of the file
            string relativePath = message.VoiceFileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            string filePath = Path.Combine(_env.WebRootPath, relativePath);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Voice file not found", filePath);

            string fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
            string mimeType = fileExtension switch
            {
                ".wav" => "audio/wav",
                ".m4a" => "audio/m4a",
                _ => "application/octet-stream" // fallback
            };

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", aiJwtSecret);

            using var form = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(filePath);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            form.Add(fileContent, "file", Path.GetFileName(filePath));

            form.Add(new StringContent(modelId), "model_id");
            form.Add(new StringContent(sourceLanguage), "speaker_lang");
            form.Add(new StringContent(targetLanguage), "target_lang");
            form.Add(new StringContent(user.Gender ?? "male"), "gender");

            var url = $"{aiBaseLink}/voice/process";
            var response = await client.PostAsync(url, form);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            // Extract task_id from response JSON
            var json = JObject.Parse(responseString);
            var taskId = json["task_id"]?.ToString();
            if (string.IsNullOrEmpty(taskId))
                throw new Exception("AI service did not return a task_id");
            return taskId;
        }
    }
}
