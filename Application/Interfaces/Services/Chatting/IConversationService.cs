using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chatting;
using Application.DTOs;

namespace Application.Interfaces.Services.Chatting
{
    public interface IConversationService
    {
        Task<ResponseViewModel<String>> AddUserToConversationAsync(Guid conversationId, string Email, string userId);
        Task<ResponseViewModel<ConversationDto>> CreateConversation(CreateConversationDto dto, string? creatorUserId);
        Task<ResponseViewModel<ConversationDto>> GetConversationByIdAsync(Guid conversationId, string? userId);
        Task<ResponseViewModel<ICollection<ConversationDto>>> GetConversationsForUserAsync(string? userId);
        Task<ResponseViewModel<ConversationDto>> UpdateConversationAsync(Guid conversationId, UpdateConversationDto dto, string? userId);
        Task<ResponseViewModel<string>> DeleteOrLeaveConversationAsync(Guid conversationId, string userId);
    }
}


