namespace MorphingTalk_API.DTOs.Chatting
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
    }
}
