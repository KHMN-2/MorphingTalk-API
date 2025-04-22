namespace MorphingTalk_API.DTOs.Chatting
{
    public class ChatDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
        public List<string> UserIds { get; set; } = new List<string>(); 
    }
}
