using Application.DTOs.Chatting;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MorphingTalk_API.DTOs.Chatting;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Ensure endpoints require authentication
public class ChattingController : ControllerBase
{
    private readonly IConversationRepository _conversationRepo;
    private readonly IConversationUserRepository _conversationUserRepo;
    private readonly IMessageRepository _messageRepo;
    private readonly IUserRepository _userRepo;
    private readonly IMessageService _messageService;
    private readonly IChatNotificationService _chatNotificationService;

    public ChattingController(
        IConversationRepository conversationRepo,
        IConversationUserRepository conversationUserRepo,
        IUserRepository userRepository,
        IMessageRepository messageRepo,
        IMessageService messageService,
        IChatNotificationService chatNotificationService)
    {
        _conversationRepo = conversationRepo;
        _conversationUserRepo = conversationUserRepo;
        _messageRepo = messageRepo;
        _userRepo = userRepository;
        _messageService = messageService;
        _chatNotificationService = chatNotificationService;
    }

    // GET: api/Chatting/conversations
    [HttpGet("conversations")]
    public async Task<IActionResult> GetMyConversations()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var conversations = await _conversationRepo.GetConversationsForUserAsync(userId);

        var result = conversations.Select(conversation => {
            var name = conversation.Name;
            if (conversation.Type == ConversationType.OneToOne && conversation.ConversationUsers.Count == 2)
            {
                var otherUser = conversation.ConversationUsers.First(cu => cu.UserId != User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                name = otherUser.User?.FullName;
            }
            return new ConversationDto
            {
                Id = conversation.Id,
                Name = name,
                Type = conversation.Type,
                CreatedAt = conversation.CreatedAt,
                Users = conversation.ConversationUsers?.Select(cu => new ConversationUserDto
                {
                    UserId = cu.UserId,
                    DisplayName = cu.User?.FullName,
                }).ToList(),
                LastMessage = conversation.Messages?
                    .OrderByDescending(m => m.SentAt)
                    .Take(1)
                    .Select(m => new MessageSummaryDto
                    {
                        Id = m.Id,
                        Type = m is TextMessage ? "text" : m is VoiceMessage ? "voice" : "unknown",
                        SenderUserId = m.ConversationUser?.UserId,
                        SenderDisplayName = m.ConversationUser?.User?.FullName,
                        Text = m is TextMessage tm ? tm.Content : null,
                        SentAt = m.SentAt
                    }).FirstOrDefault()
            };
            }).ToList();

        return Ok(result);
    }

    // GET: api/Chatting/conversations/{conversationId}
    [HttpGet("conversations/{conversationId}")]
    public async Task<IActionResult> GetConversation(Guid conversationId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var conversation = await _conversationRepo.GetByIdAsync(conversationId);
        if (conversation == null)
            return NotFound();

        var name = conversation.Name;
        if(conversation.Type == ConversationType.OneToOne && conversation.ConversationUsers.Count == 2)
        {
            var otherUser = conversation.ConversationUsers.First(cu => cu.UserId != User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            name = otherUser.User?.FullName;
        }
        var result = new ConversationDto
        {
            Id = conversation.Id,
            Name = name,
            Type = conversation.Type,
            CreatedAt = conversation.CreatedAt,
            LoggedInConversationUser = conversation.ConversationUsers?
            .Where(cu => cu.UserId == userId)
            .Select(cu => new ConversationUserDto
            {
                ConversationUserId = cu.Id,
                UserId = cu.UserId,
                DisplayName = cu.User?.FullName,
            }).FirstOrDefault(),
            Users = conversation.ConversationUsers?.Select(cu => new ConversationUserDto
            {
                UserId = cu.UserId,
                DisplayName = cu.User?.FullName,
            }).ToList(),
            LastMessage = conversation.Messages?
                .OrderByDescending(m => m.SentAt)
                .Take(1)
                .Select(m => new MessageSummaryDto
                {
                    Id = m.Id,
                    Type = m is TextMessage ? "text" : m is VoiceMessage ? "voice" : "unknown",
                    SenderUserId = m.ConversationUser?.UserId,
                    SenderDisplayName = m.ConversationUser?.User?.FullName,
                    Text = m is TextMessage tm ? tm.Content : null,
                    SentAt = m.SentAt
                }).FirstOrDefault()
        };

        return Ok(result);
    }

    // POST: api/Chatting/conversations
    [HttpPost("conversations")]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto dto)
    {
        var creatorUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if(dto.Type == ConversationType.OneToOne)
        {
            if(dto.UserEmails.Count != 1)
            {
                return BadRequest("One-to-one conversations must have exactly 1 users.");
            }
            var friendUser = await _userRepo.GetUserByEmailAsync(dto.UserEmails[0]);
            if (friendUser == null)
            {
                return NotFound("user not found.");
            }
            if(creatorUserId == friendUser.Id)
            {
                return BadRequest("Cannot create a conversation with the same user.");
            }

            var existingConversation = await _conversationRepo.GetOneToOneConversationAsync(creatorUserId, friendUser.Id);
            if (existingConversation != null)
            {

                return Ok(new ConversationDto {
                    Id = existingConversation.Id,
                    Name = friendUser.FullName,
                    Type = existingConversation.Type,
                    CreatedAt = existingConversation.CreatedAt,
                    Users = existingConversation.ConversationUsers.Select(cu => new ConversationUserDto
                    {
                        UserId = cu.UserId,
                        DisplayName = cu.User?.FullName,
                    }).ToList(),
                });
            }

        }

        var UserIds = new List<string> { };
        foreach (var userEmail in dto.UserEmails)
        {
            var user = await _userRepo.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound($"User with email {userEmail} not found.");
            }
            UserIds.Add(user.Id);
        }
        
            // Create Conversation entity
        var conversation = new Conversation
        {
            Name = dto.Name,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow,
            ConversationUsers = UserIds
                .Append(creatorUserId)
                .Distinct()
                .Select(userId => new ConversationUser { UserId = userId })
                .ToList()
        };

        var created = await _conversationRepo.AddAsync(conversation);

        // Optional: project to DTO
        var result = new ConversationDto
        {
            Id = created.Id,
            Name = created.Name,
            Type = created.Type,
            CreatedAt = created.CreatedAt,
            Users = created.ConversationUsers.Select(cu => new ConversationUserDto
            {
                UserId = cu.UserId,
                DisplayName = cu.User?.FullName,
            }).ToList()
        };

        return CreatedAtAction(nameof(GetConversation), new { conversationId = created.Id }, result);
    }


    // POST: api/Chatting/conversations/{conversationId}/users
    [HttpPost("conversations/{conversationId}/users")]
    public async Task<IActionResult> AddUserToConversation(Guid conversationId, [FromBody] AddUserToConversationDto dto)
    {
        var cu = new ConversationUser
        {
            ConversationId = conversationId,
            UserId = dto.UserId
        };
        await _conversationUserRepo.AddAsync(cu);

        // Get user details for notification
        var user = await _userRepo.GetUserByIdAsync(dto.UserId);

        // Notify other users in the conversation
        await _chatNotificationService.NotifyUserJoinedConversation(
            conversationId,
            dto.UserId,
            user?.FullName ?? "Unknown User");

        return Ok();
    }

    // DELETE: api/Chatting/conversations/{conversationId}/users/{email}
    [HttpDelete("conversations/{conversationId}/users/{email}")]
    public async Task<IActionResult> RemoveUserFromConversation(Guid conversationId, string email)
    {

        var user = await _userRepo.GetUserByEmailAsync(email);
        // Notify before removal to ensure user data is still available
        await _chatNotificationService.NotifyUserLeftConversation(
            conversationId,
            user.Id,
            user.FullName ?? "Unknown User");

        await _conversationUserRepo.RemoveAsync(conversationId, user.Id);
        return NoContent();
    }

    // GET: api/Chatting/conversations/{conversationId}/users
    [HttpGet("conversations/{conversationId}/users")]
    public async Task<IActionResult> GetUsersForConversation(Guid conversationId)
    {
        var users = await _conversationUserRepo.GetUsersForConversationAsync(conversationId);

        var result = users.Select(cu => new ConversationUserDto
        {
            UserId = cu.UserId,
            DisplayName = cu.User?.FullName,
        }).ToList();

        return Ok(result);
    }

    // POST: api/Chatting/conversations/{conversationId}/messages
    [HttpPost("conversations/{conversationId}/messages")]
    public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendMessageDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        try{

            return Ok(await _messageService.ProcessMessageAsync(dto, conversationId, userId));

        }  catch (Exception ex) { 

            return BadRequest("Unsupported message type.");
        }
    }

    // GET: api/Chatting/conversations/{conversationId}/messages
    [HttpGet("conversations/{conversationId}/messages")]
    public async Task<IActionResult> GetMessages(Guid conversationId, [FromQuery] int count = 50, [FromQuery] int skip = 0)
    {
        var messages = await _messageRepo.GetMessagesForConversationAsync(conversationId, count, skip);

        var result = messages.Select(m => new MessageSummaryDto
        {
            Id = m.Id,
            Type = m is TextMessage ? "text" : m is VoiceMessage ? "voice" : "unknown",
            SenderUserId = m.ConversationUser?.UserId,
            SenderDisplayName = m.ConversationUser?.User?.FullName,
            Text = m is TextMessage tm ? tm.Content : null,
            SentAt = m.SentAt
        }).ToList();

        return Ok(result);
    }

    // DELETE: api/Chatting/messages/{messageId}
    [HttpDelete("messages/{messageId}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
        await _messageRepo.DeleteAsync(messageId);
        return NoContent();
    }
}