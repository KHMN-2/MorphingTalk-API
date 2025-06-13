using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Calls
{
    public class StartCallRequest
    {
        [Required]
        public string ConversationId { get; set; } = string.Empty;

        [Required]
        public string TargetUserId { get; set; } = string.Empty;

        [Required]
        public string CallType { get; set; } = string.Empty; // "audio" or "video"
    }
}
