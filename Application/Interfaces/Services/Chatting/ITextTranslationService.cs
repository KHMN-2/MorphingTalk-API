using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Chatting
{
    public interface ITextTranslationService
    {
        Task<Dictionary<string, string>> TranslateTextAsync(string text, string sourceLanguage, List<string> targetLanguages);
        Task<string> TranslateTextToLanguageAsync(string text, string sourceLanguage, string targetLanguage);
    }
} 