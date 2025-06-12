namespace Application.DTOs.Calls
{
    public class CallStatusResponse
    {
        public string ConversationId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "initiated", "connected", "ended"
        public DateTime Timestamp { get; set; }
        public List<string> Participants { get; set; } = new();
    }
}
