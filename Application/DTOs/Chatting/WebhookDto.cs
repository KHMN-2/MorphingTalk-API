using Domain.Entities.Chatting;

namespace Application.DTOs.Chatting
{    public class AIWebhookInferencePayloadDto
    {
        public string RequestId { get; set; } = "";
        public string? modelId { get; set; }
        public string Success { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
    } 
    public class  AIWebhookTrainingPayloadDto
    {
        public string RequestId { get; set; } = "";
        public string modelId { get; set; } = "";
        public string Success { get; set; } = "";  // Changed to string to match AI service
        public string ErrorMessage { get; set; } = "";

    }

} 