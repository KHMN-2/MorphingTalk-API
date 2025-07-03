using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Application.Interfaces.Services.Chatting;
using Microsoft.Extensions.Logging;

namespace Application.Services.Chatting
{
    public class MyMemoryTranslationService : ITextTranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MyMemoryTranslationService> _logger;

        public MyMemoryTranslationService(IHttpClientFactory httpClientFactory, ILogger<MyMemoryTranslationService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
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
                // MyMemory API endpoint
                var encodedText = HttpUtility.UrlEncode(text);
                var langPair = $"{sourceLanguage}|{targetLanguage}";
                var url = $"https://api.mymemory.translated.net/get?q={encodedText}&langpair={langPair}";

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var translationResponse = JsonSerializer.Deserialize<MyMemoryResponse>(responseContent);

                    if (translationResponse?.ResponseData?.TranslatedText != null)
                    {
                        var translatedText = translationResponse.ResponseData.TranslatedText;
                        
                        // Check if translation was successful (MyMemory returns original text if translation fails)
                        if (!string.IsNullOrEmpty(translatedText) && !translatedText.Equals(text, StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogInformation("Successfully translated text from {SourceLanguage} to {TargetLanguage}", sourceLanguage, targetLanguage);
                            return translatedText;
                        }
                        else
                        {
                            _logger.LogWarning("Translation may have failed - returned text is same as original for {SourceLanguage} to {TargetLanguage}", sourceLanguage, targetLanguage);
                            return translatedText; // Return anyway, might be valid
                        }
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("MyMemory API returned error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during translation from {SourceLanguage} to {TargetLanguage}", sourceLanguage, targetLanguage);
            }

            // Return original text if translation fails
            return text;
        }
    }

    // MyMemory API response models
    public class MyMemoryResponse
    {
        public MyMemoryResponseData ResponseData { get; set; }
        public string ResponseStatus { get; set; }
        public MyMemoryMatch[] Matches { get; set; }
    }

    public class MyMemoryResponseData
    {
        public string TranslatedText { get; set; }
        public string Match { get; set; }
    }

    public class MyMemoryMatch
    {
        public string Id { get; set; }
        public string Segment { get; set; }
        public string Translation { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string Quality { get; set; }
        public string Reference { get; set; }
        public string UsageCount { get; set; }
        public string Subject { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public string CreateDate { get; set; }
        public string LastUpdateDate { get; set; }
        public string Match { get; set; }
    }
} 