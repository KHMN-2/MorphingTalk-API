namespace MorphingTalk_API.DTOs.Chatting
{
    public class CreateConversationDto
    {
        public string Name { get; set; }
        public List<string> UserEmails { get; set; } = new List<string>();
    }
}
