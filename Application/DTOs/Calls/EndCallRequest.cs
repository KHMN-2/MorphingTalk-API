using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Calls
{
    public class EndCallRequest
    {
        [Required]
        public string ConversationId { get; set; } = string.Empty;
    }
}
