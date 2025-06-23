using Domain.Entities.Chatting;

namespace Application.DTOs.Chatting
{    public class AIWebhookInferencePayloadDto
    {
        public string RequestId { get; set; } = "";
        public string? modelId { get; set; }
        public string Success { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
    }    public class  AIWebhookTrainingPayloadDto
    {
        public string RequestId { get; set; } = "";
        public string modelId { get; set; } = "";
        public string success { get; set; } = "";  // Changed to string to match AI service
        public string errorMessage { get; set; } = "";

        // Helper property to get boolean value
        public bool Success => success?.ToLower() == "true";
        public string ErrorMessage => errorMessage ?? "";
    }

} 