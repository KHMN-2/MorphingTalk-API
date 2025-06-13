using Domain.Entities.Chatting;

namespace Application.DTOs.Chatting
{
    public class TextTranslationWebhookDto
    {
        public string MessageId { get; set; }
        public string TranslatedText { get; set; }
        public string TargetLanguage { get; set; }
        public string SourceLanguage { get; set; }
        public double Confidence { get; set; }
    }

    public class VoiceTranslationWebhookDto
    {
        public string MessageId { get; set; }
        public string TranslatedVoiceUrl { get; set; }
        public string TranslatedText { get; set; }
        public double Duration { get; set; }
        public string TargetLanguage { get; set; }
        public string SourceLanguage { get; set; }
        public double Confidence { get; set; }
    }

    public enum WebhookType
    {
        TextTranslation,
        VoiceTranslation
    }

    public class AIWebhookPayloadDto
    {
        public WebhookType Type { get; set; }
        public string RequestId { get; set; }
        public TextTranslationWebhookDto TextTranslation { get; set; }
        public VoiceTranslationWebhookDto VoiceTranslation { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
} 