using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chatting
{
   
    public class TextMessage : Message
    {
        public string Content { get; set; }
        public bool IsTranslated { get; set; }
        public string? TranslatedContent { get; set; }
        public Dictionary<string, string>? TranslatedContents { get; set; }
        public MessageType Type => MessageType.Text;

        public bool AreAllTranslationsComplete(List<string> targetLanguages)
        {
            if (TranslatedContents == null || !targetLanguages.Any())
                return false;
                
            return targetLanguages.All(lang => TranslatedContents.ContainsKey(lang) && !string.IsNullOrEmpty(TranslatedContents[lang]));
        }
        
        public string GetContentForLanguage(string targetLanguage)
        {
            if (TranslatedContents != null && TranslatedContents.ContainsKey(targetLanguage))
                return TranslatedContents[targetLanguage];
                
            if (!string.IsNullOrEmpty(TranslatedContent))
                return TranslatedContent;
                
            return Content;
        }
    }
}
