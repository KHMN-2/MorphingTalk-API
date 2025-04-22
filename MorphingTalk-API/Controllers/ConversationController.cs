using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Authentication;
using AutoMapper;
using Domain.Entities.Chatting;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MorphingTalk.API.Hubs;
using MorphingTalk_API.DTOs.Chatting;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IConversationUserRepository _conversationUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public ConversationController(IConversationRepository conversationRepository, IConversationUserRepository conversationUserRepository, IUserRepository userRepository, ITokenService tokenService)
        {
            _conversationRepository = conversationRepository;
            _conversationUserRepository = conversationUserRepository;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateGroupConversation([FromBody] CreateConversationDto chatDto)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            User logged_in;
            try
            {
                logged_in = await _tokenService.GetUserFromToken(token);
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
            if (logged_in == null)
            {
                return Unauthorized("Invalid token");
            }

            Guid ConversationId = Guid.NewGuid();
            List<User> users = [];
            foreach (var email in chatDto.UserEmails)
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound($"User with Email {email} not found.");
                }
                users.Add(user);

            }
            Conversation newConversation = new()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow,
                Type = ConversationType.Group,
            };
            await _conversationRepository.AddConversationAsync(newConversation);

            foreach (var user in users)
            {

                ConversationUser conversationUser = new ConversationUser
                {
                    UserId = user.Id,
                    ConversationId = ConversationId,
                    JoinedAt = DateTime.UtcNow,
                    Role = user.Email == logged_in.Email ? Roles.Admin : Roles.Member
                };
            
                await _conversationUserRepository.AddConverstaionUserAsync(conversationUser);
            }
            
            return Ok();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePrivateConversation([FromBody] CreateConversationDto chatDto)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            User logged_in;
            try
            {
                logged_in = await _tokenService.GetUserFromToken(token);
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
            if (logged_in == null)
            {
                return Unauthorized("Invalid token");
            }

            if (chatDto.UserEmails.Count != 1) {
                return BadRequest("Add exactly one user`s email");
            }
            Guid ConversationId = Guid.NewGuid();
            var user = await _userRepository.GetUserByEmailAsync(chatDto.UserEmails[0]);
            if (user == null)
            {
                return NotFound($"User with email {user.Email} not found.");
            }
                     
            Conversation newConversation = new()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow,
                Type = ConversationType.Group,
            };
            await _conversationRepository.AddConversationAsync(newConversation);

            ConversationUser conversationUser = new ConversationUser
            {
                UserId = user.Id,
                ConversationId = ConversationId,
                JoinedAt = DateTime.UtcNow,
                Role = user.Email == logged_in.Email ? Roles.Admin : Roles.Member
            };

            await _conversationUserRepository.AddConverstaionUserAsync(conversationUser);

            return Ok();
        }

        //[HttpPost("{chatId}/messages")]
        //public async Task<IActionResult> SendMessage(int chatId, [FromBody] MessageDto messageDto)
        //{
        //    var message = _mapper.Map<TextMessage>(messageDto);
        //    message.ChatId = chatId;
        //    await _chatRepository.AddMessageAsync(message);

        //    // Send message to all clients in the chat group
        //    await _hubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", message.UserId, message.Content);
        //    return Ok();
        //}

        //[HttpPost("{chatId}/users")]
        //public async Task<IActionResult> AddUserToChat(int chatId, [FromBody] string userId)
        //{
        //    await _chatRepository.AddUserToChatAsync(chatId, userId);
        //    await _hubContext.Clients.Group(chatId.ToString()).SendAsync("UserJoined", userId);
        //    return Ok();
        //}

        //[HttpDelete("{chatId}/users/{userId}")]
        //public async Task<IActionResult> RemoveUserFromChat(int chatId, string userId)
        //{
        //    await _chatRepository.RemoveUserFromChatAsync(chatId, userId);
        //    await _hubContext.Clients.Group(chatId.ToString()).SendAsync("UserLeft", userId);
        //    return Ok();
        //}

        //[HttpGet("{chatId}/users")]
        //public async Task<IActionResult> GetUsersInChat(int chatId)
        //{
        //    var users = await _chatRepository.GetUsersInChatAsync(chatId);

        //    var userDtos = _mapper.Map<List<UserDto>>(users);
        //    return Ok(userDtos);
        //}
    }
}
