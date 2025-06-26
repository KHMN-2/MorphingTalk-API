namespace Application.DTOs.Chatting
{
    public class VoiceTrainingRequestDto
    {
        public string? ModelId { get; set; }
        public string? SpeakerLanguage { get; set; }
    }

    public class VoiceTrainingResponseDto
    {
        public string TaskId { get; set; } = "";
        public string ModelId { get; set; } = "";
        public string Status { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public class VoiceTrainingStatusDto
    {
        public bool IsTrainedVoice { get; set; }
        public VoiceModelDto? VoiceModel { get; set; }
        public string Status { get; set; } = "";
    }

    public class VoiceModelDto
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
