using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Calls
{
    public class CallInvitationResponse
    {
        [Required]
        public string ConversationId { get; set; } = string.Empty;

        [Required]
        public string CallerId { get; set; } = string.Empty;

        [Required]
        public bool Accepted { get; set; }
    }
}
