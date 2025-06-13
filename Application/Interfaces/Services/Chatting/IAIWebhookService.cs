using Application.DTOs;
using Application.DTOs.Chatting;

namespace Application.Interfaces.Services.Chatting
{
    public interface IAIWebhookService
    {
        Task<ResponseViewModel<string>> HandleTextTranslationWebhookAsync(AIWebhookPayloadDto payload);
        Task<ResponseViewModel<string>> HandleVoiceTranslationWebhookAsync(AIWebhookPayloadDto payload);
    }
} 