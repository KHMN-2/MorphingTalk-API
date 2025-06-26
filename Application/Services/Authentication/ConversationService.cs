using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.Chatting;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Domain.Entities.Users;

namespace Application.Services.Authentication
{
    public class ConversationService : IConversationService

    {
        private readonly IConversationRepository _conversationRepo;
        private readonly IUserRepository _userRepo;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly IConversationUserRepository _conversationUserRepo;


        public ConversationService(IConversationRepository conversationRepo, IUserRepository userRepo, IChatNotificationService chatNotificationService, IConversationUserRepository conversationUserRepo)
        {
            _conversationRepo = conversationRepo;
            _userRepo = userRepo;
            _chatNotificationService = chatNotificationService;
            _conversationUserRepo = conversationUserRepo;
        }
        public async Task<ResponseViewModel<ICollection<ConversationDto>>> GetConversationsForUserAsync(string? userId)
        {
            var conversations = await _conversationRepo.GetConversationsForUserAsync(userId);

            var result = conversations.Select(conversation => ConversationDto.fromCoversation(conversation,userId)).ToList();

            return new ResponseViewModel<ICollection<ConversationDto>>(result, "Conversations retrieved successfully.", true, 200);
        }

        public async Task<ResponseViewModel<ConversationDto>> CreateConversation(CreateConversationDto dto, string? creatorUserId)
        {
            if (dto.Type == ConversationType.OneToOne)
            {
                if (dto.UserEmails.Count != 1)
                {
                    return new ResponseViewModel<ConversationDto>(null, "One-to-one conversations must have exactly 1 user.", false, 400);
                }
                var friendUser = await _userRepo.GetUserByEmailAsync(dto.UserEmails[0]);
                if (friendUser == null)
                {
                    return new ResponseViewModel<ConversationDto>(null, "Friend user not found.", false, 404);
                }
                if (creatorUserId == friendUser.Id)
                {
                    return new ResponseViewModel<ConversationDto>(null, "Cannot create a conversation with the same user.", false, 400);
                }

                var existingConversation = await _conversationRepo.GetOneToOneConversationAsync(creatorUserId, friendUser.Id);
                if (existingConversation != null)
                {
                    var res = ConversationDto.fromCoversation(existingConversation, creatorUserId);
                    return new ResponseViewModel<ConversationDto>(res, "One-to-one conversation already exists.", true, 200);
                }

            }

            var UserIds = new List<string> { };
            foreach (var userEmail in dto.UserEmails)
            {
                var user = await _userRepo.GetUserByEmailAsync(userEmail);
                if (user == null)
                {
                    return new ResponseViewModel<ConversationDto>(null, $"User with email {userEmail} not found.", false, 404);
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
                    .Select(userId => new ConversationUser { UserId = userId, Role = userId == creatorUserId ? Roles.Admin : Roles.Member })
                    .ToList(),
                GroupImageUrl = dto.GroupImageUrl,
                Description = dto.Description
            };

            var created = await _conversationRepo.AddAsync(conversation);

            var created_updated = await _conversationRepo.GetByIdAsync(created.Id);

            // Optional: project to DTO
            var result = ConversationDto.fromCoversation(created_updated, creatorUserId);

            return new ResponseViewModel<ConversationDto>(result, "Conversation created successfully.", true, 201);
        }

        public async Task<ResponseViewModel<ConversationDto>> GetConversationByIdAsync(Guid conversationId, string? userId)
        {
            var conversation = await _conversationRepo.GetByIdAsync(conversationId);
            if (conversation == null)
                return new ResponseViewModel<ConversationDto>(null, "Conversation not found.", false, 404);
            if (userId == null || !conversation.ConversationUsers.Any(cu => cu.UserId == userId))
                return new ResponseViewModel<ConversationDto>(null, "User not part of this conversation.", false, 403);

            var name = conversation.Name;
            var image = conversation.GroupImageUrl;
            if (conversation.Type == ConversationType.OneToOne)
            {
                var otherUser = conversation.ConversationUsers.First(cu => cu.UserId != userId);
                name = otherUser.User?.FullName;
                image = otherUser.User?.ProfilePicturePath;

            }
            var result = ConversationDto.fromCoversation(conversation, userId);
            return new ResponseViewModel<ConversationDto>(result, "Conversation retrieved successfully.", true, 200);
        }

        public async Task<ResponseViewModel<string>> AddUserToConversationAsync(Guid conversationId, string Email, string AdminUserId)
        {
            // Check if user exists
            var user = await _userRepo.GetUserByEmailAsync(Email);
            if (user == null)
            {
                return new ResponseViewModel<string>(null, "User not found.", false, 404);
            }
            // Check if conversation exists
            var conversation = await _conversationRepo.GetByIdAsync(conversationId);
            if (conversation == null)
            {
                return new ResponseViewModel<string>(null, "Conversation not found.", false, 404);
            }
            // Check if user is creator or admin of the conversation
            if (!conversation.ConversationUsers.Any(cu => cu.UserId == AdminUserId && (cu.Role == Roles.Admin )))
            {
                return new ResponseViewModel<string>(null, "Only admins can add users to the conversation.", false, 403);
            }

            // Check if user already in conversation
            var existingUser = await _conversationUserRepo.GetByIdsAsync(conversationId, user.Id);
            if (existingUser != null)
            {
                return new ResponseViewModel<string>(null, "User already in conversation.", false, 400);
            }
            // Add user to conversation
            var cu = new ConversationUser
            {
                ConversationId = conversationId,
                UserId = user.Id
            };
            await _conversationUserRepo.AddAsync(cu);

            // Notify other users in the conversation
            await _chatNotificationService.NotifyUserJoinedConversation(
                conversationId,
                user.Id,
                user?.FullName ?? "Unknown User");
            return new ResponseViewModel<string>(null, "User added to conversation successfully.", true, 200);
        }

    }
}
