using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chatting
{
    public class VoiceMessage : Message
    {
        public string VoiceUrl { get; set; }
        public bool IsTranslated { get; set; }
        public string? TranslatedVoiceUrl { get; set; }
        public Dictionary<string, string>? TranslatedVoiceUrls { get; set; }
        public int? DurationSeconds { get; set; }
        public MessageType Type => MessageType.Voice;
        
        public bool AreAllTranslationsComplete(List<string> targetLanguages)
        {
            if (TranslatedVoiceUrls == null || !targetLanguages.Any())
                return false;
                
            return targetLanguages.All(lang => TranslatedVoiceUrls.ContainsKey(lang) && !string.IsNullOrEmpty(TranslatedVoiceUrls[lang]));
        }
        
        public string GetVoiceUrlForLanguage(string targetLanguage)
        {
            if (TranslatedVoiceUrls != null && TranslatedVoiceUrls.ContainsKey(targetLanguage))
                return TranslatedVoiceUrls[targetLanguage];
                
            if (!string.IsNullOrEmpty(TranslatedVoiceUrl))
                return TranslatedVoiceUrl;
                
            return VoiceUrl;
        }
    }
}
