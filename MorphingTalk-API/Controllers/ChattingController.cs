using Application.DTOs.Chatting;
using Application.DTOs;
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
    private readonly IConversationService _conversationService;

    public ChattingController(
        IConversationRepository conversationRepo,
        IConversationUserRepository conversationUserRepo,
        IUserRepository userRepository,
        IMessageRepository messageRepo,
        IMessageService messageService,
        IChatNotificationService chatNotificationService,
        IConversationService conversationService
        )
    {
        _conversationRepo = conversationRepo;
        _conversationUserRepo = conversationUserRepo;
        _messageRepo = messageRepo;
        _userRepo = userRepository;
        _messageService = messageService;
        _chatNotificationService = chatNotificationService;
        _conversationService = conversationService;
    }

    // GET: api/Chatting/conversations
    [HttpGet("conversations")]
    public async Task<IActionResult> GetMyConversations()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var result = await _conversationService.GetConversationsForUserAsync(userId);
        return StatusCode(result.StatusCode, result);
    }

    // GET: api/Chatting/conversations/{conversationId}
    [HttpGet("conversations/{conversationId}")]
    public async Task<IActionResult> GetConversation(Guid conversationId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var result = await _conversationService.GetConversationByIdAsync(conversationId, userId);
        return StatusCode(result.StatusCode, result);
    }

    // POST: api/Chatting/conversations
    [HttpPost("conversations")]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto dto)
    {
        var creatorUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var result = await _conversationService.CreateConversation(dto, creatorUserId);
        return StatusCode(result.StatusCode, result);
    }

    // PUT: api/Chatting/conversations/{conversationId}
    [HttpPut("conversations/{conversationId}")]
    public async Task<IActionResult> UpdateConversation(Guid conversationId, [FromBody] UpdateConversationDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var result = await _conversationService.UpdateConversationAsync(conversationId, dto, userId);
        return StatusCode(result.StatusCode, result);
    }

    // POST: api/Chatting/conversations/{conversationId}/users
    [HttpPost("conversations/{conversationId}/users")]
    public async Task<IActionResult> AddUserToConversation(Guid conversationId, [FromBody] AddUserToConversationDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var result = await _conversationService.AddUserToConversationAsync(conversationId, dto.Email, userId);
        return StatusCode(result.StatusCode, result);
    }

    // DELETE: api/Chatting/conversations/{conversationId}/users/{email}
    [HttpDelete("conversations/{conversationId}/users/{email}")]
    public async Task<IActionResult> RemoveUserFromConversation(Guid conversationId, string email)
    {
        try 
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            // Notify before removal to ensure user data is still available
            await _chatNotificationService.NotifyUserLeftConversation(
                conversationId,
                user.Id,
                user.FullName ?? "Unknown User");

            await _conversationUserRepo.RemoveAsync(conversationId, user.Id);
            return StatusCode(StatusCodes.Status204NoContent);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status404NotFound, 
                new ResponseViewModel<string>(null, "User not found", false, StatusCodes.Status404NotFound));
        }
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
            Role = cu.Role.ToString(),
            ProfileImagePath = cu.User?.ProfilePicturePath,
            bio = cu.User?.AboutStatus,
        }).ToList();

        return StatusCode(StatusCodes.Status200OK, 
            new ResponseViewModel<List<ConversationUserDto>>(result, "Users retrieved successfully", true, StatusCodes.Status200OK));
    }

    // POST: api/Chatting/conversations/{conversationId}/messages
    [HttpPost("conversations/{conversationId}/messages")]
    public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendMessageDto dto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return StatusCode(StatusCodes.Status401Unauthorized,
                new ResponseViewModel<string>(null, "User not authenticated", false, StatusCodes.Status401Unauthorized));
        }

        if (dto == null)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
                new ResponseViewModel<string>(null, "Message data is required", false, StatusCodes.Status400BadRequest));
        }

        try
        {
            var result = await _messageService.ProcessMessageAsync(dto, conversationId, userId);
            if(result != "")
            {
                return StatusCode(StatusCodes.Status200OK,
                    new ResponseViewModel<string>(result, "Message will be translated", true, StatusCodes.Status200OK));
            }
            return StatusCode(StatusCodes.Status200OK, 
                new ResponseViewModel<string>(result, "Message sent successfully", true, StatusCodes.Status200OK));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, 
                new ResponseViewModel<string>(null, e.Message, false, StatusCodes.Status400BadRequest));
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
            Type = m is TextMessage ? MessageType.Text.ToString() : m is VoiceMessage ? MessageType.Voice.ToString() : "unknown",
            SenderId = m.ConversationUser?.UserId,
            SenderDisplayName = m.ConversationUser?.User?.FullName,
            Text = m is TextMessage tm ? tm.Content : null,
            VoiceFileUrl = m is VoiceMessage vm ? (vm.IsTranslated ? vm.TranslatedVoiceUrl : vm.VoiceUrl) : null,
            Duration = m is VoiceMessage v ? v.DurationSeconds : null,
            SentAt = m.SentAt
        }).ToList();

        return StatusCode(StatusCodes.Status200OK, 
            new ResponseViewModel<List<MessageSummaryDto>>(result, "Messages retrieved successfully", true, StatusCodes.Status200OK));
    }

    // DELETE: api/Chatting/messages/{messageId}
    [HttpDelete("messages/{messageId}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
        try
        {
            await _messageRepo.DeleteAsync(messageId);
            return StatusCode(StatusCodes.Status204NoContent);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status404NotFound, 
                new ResponseViewModel<string>(null, "Message not found", false, StatusCodes.Status404NotFound));
        }
    }

}