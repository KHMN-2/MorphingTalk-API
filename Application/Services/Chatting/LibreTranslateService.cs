using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Interfaces.Services.Chatting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services.Chatting
{
    public class LibreTranslateService : ITextTranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LibreTranslateService> _logger;
        private readonly string _baseUrl;
        private readonly string? _apiKey;

        public LibreTranslateService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<LibreTranslateService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _baseUrl = configuration["LibreTranslate:BaseUrl"] ?? "https://libretranslate.com"; // Default to public instance
            _apiKey = configuration["LibreTranslate:ApiKey"]; // Optional API key
        }

        public async Task<Dictionary<string, string>> TranslateTextAsync(string text, string sourceLanguage, List<string> targetLanguages)
        {
            var translations = new Dictionary<string, string>();
            
            if (string.IsNullOrEmpty(text))
            {
                _logger.LogWarning("Empty text provided for translation");
                return translations;
            }

            // Remove source language from target languages to avoid translating to the same language
            var filteredTargetLanguages = targetLanguages.Where(lang => !lang.Equals(sourceLanguage, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var targetLanguage in filteredTargetLanguages)
            {
                try
                {
                    var translatedText = await TranslateTextToLanguageAsync(text, sourceLanguage, targetLanguage);
                    translations[targetLanguage] = translatedText;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to translate text to {TargetLanguage}", targetLanguage);
                    // Continue with other languages even if one fails
                }
            }

            return translations;
        }

        public async Task<string> TranslateTextToLanguageAsync(string text, string sourceLanguage, string targetLanguage)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // If source and target languages are the same, return original text
            if (sourceLanguage.Equals(targetLanguage, StringComparison.OrdinalIgnoreCase))
            {
                return text;
            }

            try
            {
                var requestBody = new
                {
                    q = text,
                    source = sourceLanguage,
                    target = targetLanguage,
                    format = "text",
                    alternatives = 1,
                    api_key = _apiKey
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{_baseUrl}/translate";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var translationResponse = JsonSerializer.Deserialize<LibreTranslateResponse>(responseContent);

                    if (translationResponse?.TranslatedText != null)
                    {
                        var translatedText = translationResponse.TranslatedText;
                        _logger.LogInformation("Successfully translated text from {SourceLanguage} to {TargetLanguage} using LibreTranslate", sourceLanguage, targetLanguage);
                        return translatedText;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("LibreTranslate API returned error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during translation from {SourceLanguage} to {TargetLanguage} using LibreTranslate", sourceLanguage, targetLanguage);
            }

            // Return original text if translation fails
            return text;
        }
    }

    // LibreTranslate API response models
    public class LibreTranslateResponse
    {
        public string TranslatedText { get; set; }
    }
} 