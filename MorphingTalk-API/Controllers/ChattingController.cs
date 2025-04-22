using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities.Chatting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MorphingTalk.API.Hubs;
using MorphingTalk_API.DTOs.Chatting;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChattingController : ControllerBase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMapper _mapper;

        public ChattingController(IChatRepository chatRepository, IHubContext<ChatHub> hubContext, IMapper mapper)
        {
            _chatRepository = chatRepository;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] ChatDto chatDto)
        {
            var chat = _mapper.Map<Chat>(chatDto);


            await _chatRepository.AddChatAsync(chat);
            return Ok();
        }

        [HttpPost("{chatId}/messages")]
        public async Task<IActionResult> SendMessage(int chatId, [FromBody] MessageDto messageDto)
        {
            var message = _mapper.Map<Message>(messageDto);
            message.ChatId = chatId;
            await _chatRepository.AddMessageAsync(message);

            // Send message to all clients in the chat group
            await _hubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", message.UserId, message.Content);
            return Ok();
        }

        [HttpPost("{chatId}/users")]
        public async Task<IActionResult> AddUserToChat(int chatId, [FromBody] string userId)
        {
            await _chatRepository.AddUserToChatAsync(chatId, userId);
            await _hubContext.Clients.Group(chatId.ToString()).SendAsync("UserJoined", userId);
            return Ok();
        }

        [HttpDelete("{chatId}/users/{userId}")]
        public async Task<IActionResult> RemoveUserFromChat(int chatId, string userId)
        {
            await _chatRepository.RemoveUserFromChatAsync(chatId, userId);
            await _hubContext.Clients.Group(chatId.ToString()).SendAsync("UserLeft", userId);
            return Ok();
        }

        [HttpGet("{chatId}/users")]
        public async Task<IActionResult> GetUsersInChat(int chatId)
        {
            var users = await _chatRepository.GetUsersInChatAsync(chatId);

            var userDtos = _mapper.Map<List<UserDto>>(users);
            return Ok(userDtos);
        }
    }
}
