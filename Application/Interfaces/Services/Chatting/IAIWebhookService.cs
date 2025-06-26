using Application.DTOs;
using Application.DTOs.Chatting;

namespace Application.Interfaces.Services.Chatting
{
    public interface IAIWebhookService
    {
        Task<ResponseViewModel<string>> HandleVoiceTranslationWebhookAsync(AIWebhookInferencePayloadDto payload);
        Task<ResponseViewModel<string>> HandleVoiceTrainingWebhookAsync(AIWebhookTrainingPayloadDto payload);
    }
}