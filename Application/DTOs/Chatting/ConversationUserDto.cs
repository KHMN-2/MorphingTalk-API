using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Chatting
{
    public class ConversationUserDto
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
    }

    public class AddUserToConversationDto
    {
        public string UserId { get; set; }
    }
}
