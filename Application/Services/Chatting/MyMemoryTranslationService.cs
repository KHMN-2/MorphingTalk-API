using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
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
                //var encodedText = HttpUtility.UrlEncode(text);
                var langPair = $"en|ar";
                var url = $"https://api.mymemory.translated.net/get?q=my name is&langpair={langPair}";

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
        [JsonPropertyName("responseData")]
        public MyMemoryResponseData ResponseData { get; set; }
        
        [JsonPropertyName("responseStatus")]
        public int ResponseStatus { get; set; }
        
        [JsonPropertyName("matches")]
        public MyMemoryMatch[] Matches { get; set; }
        
        [JsonPropertyName("quotaFinished")]
        public bool QuotaFinished { get; set; }
        
        [JsonPropertyName("mtLangSupported")]
        public object MtLangSupported { get; set; }
        
        [JsonPropertyName("responseDetails")]
        public string ResponseDetails { get; set; }
        
        [JsonPropertyName("responderId")]
        public object ResponderId { get; set; }
        
        [JsonPropertyName("exception_code")]
        public object ExceptionCode { get; set; }
    }

    public class MyMemoryResponseData
    {
        [JsonPropertyName("translatedText")]
        public string TranslatedText { get; set; }
        
        [JsonPropertyName("match")]
        public decimal Match { get; set; }
    }

    public class MyMemoryMatch
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("segment")]
        public string Segment { get; set; }
        
        [JsonPropertyName("translation")]
        public string Translation { get; set; }
        
        [JsonPropertyName("source")]
        public string Source { get; set; }
        
        [JsonPropertyName("target")]
        public string Target { get; set; }
        
        [JsonPropertyName("quality")]
        public JsonElement Quality { get; set; }
        
        [JsonPropertyName("reference")]
        public object Reference { get; set; }
        
        [JsonPropertyName("usage-count")]
        public int UsageCount { get; set; }
        
        [JsonPropertyName("subject")]
        public string Subject { get; set; }
        
        [JsonPropertyName("created-by")]
        public string CreatedBy { get; set; }
        
        [JsonPropertyName("last-updated-by")]
        public string LastUpdatedBy { get; set; }
        
        [JsonPropertyName("create-date")]
        public string CreateDate { get; set; }
        
        [JsonPropertyName("last-update-date")]
        public string LastUpdateDate { get; set; }
        
        [JsonPropertyName("match")]
        public decimal MatchValue { get; set; }
        
        [JsonPropertyName("penalty")]
        public int Penalty { get; set; }
        
        // Helper property to get quality as string regardless of JSON type
        public string QualityValue => Quality.ValueKind switch
        {
            JsonValueKind.Number => Quality.GetInt32().ToString(),
            JsonValueKind.String => Quality.GetString(),
            _ => "0"
        };
    }
} 