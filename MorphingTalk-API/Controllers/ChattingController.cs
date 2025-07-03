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
using System.Linq;

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

    // DELETE: api/Chatting/conversations/{conversationId}
    [HttpDelete("conversations/{conversationId}")]
    public async Task<IActionResult> DeleteOrLeaveConversation(Guid conversationId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return StatusCode(StatusCodes.Status401Unauthorized,
                new ResponseViewModel<string>(null, "User not authenticated", false, StatusCodes.Status401Unauthorized));
        }

        var result = await _conversationService.DeleteOrLeaveConversationAsync(conversationId, userId);
        return StatusCode(result.StatusCode, result);
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
        var result = messages.Select(MessageSummaryDto.FromMessage).ToList();

        return StatusCode(StatusCodes.Status200OK, 
            new ResponseViewModel<List<MessageSummaryDto>>(result, "Messages retrieved successfully", true, StatusCodes.Status200OK));
    }

    // DELETE: api/Chatting/messages/{messageId}
    [HttpDelete("messages/{messageId}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return StatusCode(StatusCodes.Status401Unauthorized,
                new ResponseViewModel<string>(null, "User not authenticated", false, StatusCodes.Status401Unauthorized));
        }

        try
        {
            // Get the message with its conversation and sender information
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, 
                    new ResponseViewModel<string>(null, "Message not found", false, StatusCodes.Status404NotFound));
            }

            // Check if the user is part of the conversation
            var conversation = await _conversationRepo.GetByIdAsync(message.ConversationId);
            if (conversation == null || !conversation.ConversationUsers.Any(cu => cu.UserId == userId))
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ResponseViewModel<string>(null, "You don't have permission to delete this message", false, StatusCodes.Status403Forbidden));
            }

            // Check if the user is the sender of the message
            if (message.ConversationUser?.UserId != userId)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ResponseViewModel<string>(null, "You can only delete your own messages", false, StatusCodes.Status403Forbidden));
            }

            // Delete the message
            await _messageRepo.DeleteAsync(messageId);
            
            return StatusCode(StatusCodes.Status200OK,
                new ResponseViewModel<string>(null, "Message deleted successfully", true, StatusCodes.Status200OK));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ResponseViewModel<string>(null, "An error occurred while deleting the message", false, StatusCodes.Status500InternalServerError));
        }
    }

}