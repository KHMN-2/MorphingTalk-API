using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Interfaces.Services.Chatting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services.Chatting
{
    public class AzureTranslationService : ITextTranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AzureTranslationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _subscriptionKey;
        private readonly string _subscriptionRegion;
        private readonly string _endpoint;

        public AzureTranslationService(
            IHttpClientFactory httpClientFactory, 
            ILogger<AzureTranslationService> logger,
            IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _configuration = configuration;
            
            // Get configuration values
            _subscriptionKey = _configuration["AzureTranslator:SubscriptionKey"];
            _subscriptionRegion = _configuration["AzureTranslator:SubscriptionRegion"];
            _endpoint = _configuration["AzureTranslator:Endpoint"] ?? "https://api.cognitive.microsofttranslator.com";

            if (string.IsNullOrEmpty(_subscriptionKey))
            {
                throw new ArgumentException("Azure Translator subscription key is required. Please set AzureTranslator:SubscriptionKey in configuration.");
            }
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
            
            if (!filteredTargetLanguages.Any())
            {
                _logger.LogWarning("No target languages specified after filtering source language");
                return translations;
            }

            try
            {
                var result = await TranslateToMultipleLanguagesAsync(text, sourceLanguage, filteredTargetLanguages);
                
                // Process the result to extract translations
                if (result?.Length > 0)
                {
                    var translationResult = result[0];
                    if (translationResult?.Translations != null)
                    {
                        foreach (var translation in translationResult.Translations)
                        {
                            if (!string.IsNullOrEmpty(translation.To) && !string.IsNullOrEmpty(translation.Text))
                            {
                                translations[translation.To] = translation.Text;
                            }
                        }
                    }
                }
                
                _logger.LogInformation("Successfully translated text to {Count} languages", translations.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to translate text to multiple languages");
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
                var result = await TranslateToMultipleLanguagesAsync(text, sourceLanguage, new List<string> { targetLanguage });
                
                if (result?.Length > 0 && result[0]?.Translations?.Length > 0)
                {
                    var translatedText = result[0].Translations[0].Text;
                    
                    if (!string.IsNullOrEmpty(translatedText))
                    {
                        _logger.LogInformation("Successfully translated text from {SourceLanguage} to {TargetLanguage}", sourceLanguage, targetLanguage);
                        return translatedText;
                    }
                }
                
                _logger.LogWarning("No translation result returned from Azure Translator");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during translation from {SourceLanguage} to {TargetLanguage}", sourceLanguage, targetLanguage);
            }

            // Return original text if translation fails
            return text;
        }

        private async Task<AzureTranslationResult[]> TranslateToMultipleLanguagesAsync(string text, string sourceLanguage, List<string> targetLanguages)
        {
            try
            {
                // Build the URL with query parameters
                var queryParams = new List<string>
                {
                    "api-version=3.0"
                };

                if (!string.IsNullOrEmpty(sourceLanguage))
                {
                    queryParams.Add($"from={sourceLanguage}");
                }

                foreach (var targetLang in targetLanguages)
                {
                    queryParams.Add($"to={targetLang}");
                }

                var url = $"{_endpoint}/translate?{string.Join("&", queryParams)}";

                // Create the request body
                var body = new[]
                {
                    new { Text = text }
                };

                var requestBody = JsonSerializer.Serialize(body);

                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                
                // Add required headers
                request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
                
                // Add region header if specified (required for multi-service or regional resources)
                if (!string.IsNullOrEmpty(_subscriptionRegion))
                {
                    request.Headers.Add("Ocp-Apim-Subscription-Region", _subscriptionRegion);
                }

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<AzureTranslationResult[]>(responseContent);
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Azure Translator API returned error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during Azure Translator API call");
            }

            return null;
        }
    }

    // Azure Translator API response models
    public class AzureTranslationResult
    {
        [JsonPropertyName("detectedLanguage")]
        public DetectedLanguage DetectedLanguage { get; set; }

        [JsonPropertyName("translations")]
        public Translation[] Translations { get; set; }
    }

    public class DetectedLanguage
    {
        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("score")]
        public float Score { get; set; }
    }

    public class Translation
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("to")]
        public string To { get; set; }
    }
} 