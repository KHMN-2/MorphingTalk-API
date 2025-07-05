using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.Chatting;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
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
        private readonly IFriendshipService _friendshipService;


        public ConversationService(IConversationRepository conversationRepo, IUserRepository userRepo, IChatNotificationService chatNotificationService, IConversationUserRepository conversationUserRepo, IFriendshipService friendshipService)
        {
            _conversationRepo = conversationRepo;
            _userRepo = userRepo;
            _chatNotificationService = chatNotificationService;
            _conversationUserRepo = conversationUserRepo;
            _friendshipService = friendshipService;
        }
        public async Task<ResponseViewModel<ICollection<ConversationDto>>> GetConversationsForUserAsync(string? userId)
        {
            var conversations = await _conversationRepo.GetConversationsForUserAsync(userId);

            var result = new List<ConversationDto>();
            
            foreach (var conversation in conversations)
            {
                var dto = ConversationDto.fromCoversation(conversation, userId);
                
                // Check if other user is blocked in one-to-one conversations
                if (conversation.Type == ConversationType.OneToOne)
                {
                    var otherUser = conversation.ConversationUsers.First(cu => cu.UserId != userId);
                    dto.IsOtherUserBlocked = await _friendshipService.IsUserBlockedAsync(userId, otherUser.UserId);
                }
                
                result.Add(dto);
            }

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

                // Check if users are blocked
                var isBlocked = await _friendshipService.IsUserBlockedAsync(creatorUserId, friendUser.Id);
                if (isBlocked)
                {
                    return new ResponseViewModel<ConversationDto>(null, "Cannot create conversation with blocked user.", false, 403);
                }

                var existingConversation = await _conversationRepo.GetOneToOneConversationAsync(creatorUserId, friendUser.Id);
                if (existingConversation != null && existingConversation.Type == ConversationType.OneToOne)
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
            var result = ConversationDto.fromCoversation(conversation, userId);
            
            if (conversation.Type == ConversationType.OneToOne)
            {
                var otherUser = conversation.ConversationUsers.First(cu => cu.UserId != userId);
                name = otherUser.User?.FullName;
                image = otherUser.User?.ProfilePicturePath;

                // Check if the other user is blocked
                result.IsOtherUserBlocked = await _friendshipService.IsUserBlockedAsync(userId, otherUser.UserId);
            }
            
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
                UserId = user.Id,
                Role = Roles.Member,
            };
            await _conversationUserRepo.AddAsync(cu);

            // Notify other users in the conversation
            await _chatNotificationService.NotifyUserJoinedConversation(
                conversationId,
                user.Id,
                user?.FullName ?? "Unknown User");
            return new ResponseViewModel<string>(null, "User added to conversation successfully.", true, 200);
        }

        public async Task<ResponseViewModel<ConversationDto>> UpdateConversationAsync(Guid conversationId, UpdateConversationDto dto, string? userId)
        {
            // Check if conversation exists
            var conversation = await _conversationRepo.GetByIdAsync(conversationId);
            if (conversation == null)
            {
                return new ResponseViewModel<ConversationDto>(null, "Conversation not found.", false, 404);
            }

            // Check if user is part of the conversation
            if (userId == null || !conversation.ConversationUsers.Any(cu => cu.UserId == userId))
            {
                return new ResponseViewModel<ConversationDto>(null, "User not part of this conversation.", false, 403);
            }

            var userConversation = conversation.ConversationUsers.FirstOrDefault(cu => cu.UserId == userId);
            if (userConversation == null)
            {
                return new ResponseViewModel<ConversationDto>(null, "User conversation data not found.", false, 404);
            }

            // Determine if this is a conversation-level update or user-level update
            bool hasConversationLevelUpdates = !string.IsNullOrEmpty(dto.Name) || 
                                               dto.GroupImageUrl != null || 
                                               dto.Description != null;
            
            bool hasUserLevelUpdates = dto.UseRobotVoice.HasValue || 
                                       dto.TranslateMessages.HasValue || 
                                       dto.MuteNotifications.HasValue;

            // Handle conversation-level updates (require admin permissions)
            if (hasConversationLevelUpdates)
            {
                // Check if user has permission to update conversation (only admins can update group conversations)
                if (conversation.Type == ConversationType.Group && userConversation.Role != Roles.Admin)
                {
                    return new ResponseViewModel<ConversationDto>(null, "Only admins can update group conversation details.", false, 403);
                }

                // One-to-one conversations cannot have conversation-level updates
                if (conversation.Type == ConversationType.OneToOne)
                {
                    return new ResponseViewModel<ConversationDto>(null, "One-to-one conversation details cannot be updated.", false, 400);
                }

                // Update conversation properties
                if (!string.IsNullOrEmpty(dto.Name))
                {
                    conversation.Name = dto.Name;
                }

                if (dto.GroupImageUrl != null) // Allow setting to null/empty
                {
                    conversation.GroupImageUrl = dto.GroupImageUrl;
                }

                if (dto.Description != null) // Allow setting to null/empty
                {
                    conversation.Description = dto.Description;
                }

                conversation.LastActivityAt = DateTime.UtcNow;

                // Save conversation changes
                await _conversationRepo.UpdateAsync(conversation);
            }

            // Handle user-level updates (any user can update their own settings)
            if (hasUserLevelUpdates)
            {
                if (dto.UseRobotVoice.HasValue)
                {
                    userConversation.UseRobotVoice = dto.UseRobotVoice.Value;
                }

                if (dto.TranslateMessages.HasValue)
                {
                    userConversation.TranslateMessages = dto.TranslateMessages.Value;
                }

                if (dto.MuteNotifications.HasValue)
                {
                    userConversation.muteNotifications = dto.MuteNotifications.Value;
                }

                // Save user conversation settings
                await _conversationUserRepo.UpdateAsync(userConversation);
            }

            // If no updates were provided
            if (!hasConversationLevelUpdates && !hasUserLevelUpdates)
            {
                return new ResponseViewModel<ConversationDto>(null, "No updates provided.", false, 400);
            }

            // Refresh conversation data
            var updatedConversation = await _conversationRepo.GetByIdAsync(conversationId);
            var result = ConversationDto.fromCoversation(updatedConversation, userId);

            string message = hasConversationLevelUpdates && hasUserLevelUpdates 
                ? "Conversation and user settings updated successfully." 
                : hasConversationLevelUpdates 
                    ? "Conversation updated successfully." 
                    : "User settings updated successfully.";

            return new ResponseViewModel<ConversationDto>(result, message, true, 200);
        }

        public async Task<ResponseViewModel<string>> DeleteOrLeaveConversationAsync(Guid conversationId, string userId)
        {
            // Check if conversation exists
            var conversation = await _conversationRepo.GetByIdAsync(conversationId);
            if (conversation == null)
            {
                return new ResponseViewModel<string>(null, "Conversation not found.", false, 404);
            }

            // Check if user is part of the conversation
            if (!conversation.ConversationUsers.Any(cu => cu.UserId == userId))
            {
                return new ResponseViewModel<string>(null, "User not part of this conversation.", false, 403);
            }

            var allUsers = conversation.ConversationUsers.ToList();
            var currentUser = allUsers.FirstOrDefault(cu => cu.UserId == userId);

            // Determine if we should delete the entire conversation or just remove the user
            bool shouldDeleteEntireConversation = false;

            if (conversation.Type == ConversationType.OneToOne)
            {
                // For one-to-one conversations, always delete the entire conversation
                shouldDeleteEntireConversation = true;
            }
            else if (allUsers.Count <= 1)
            {
                // If user is the last person in the conversation, delete it
                shouldDeleteEntireConversation = true;
            }

            if (shouldDeleteEntireConversation)
            {
                // Delete the entire conversation (this will cascade delete all messages and conversation users)
                await _conversationRepo.DeleteAsync(conversationId);
                
                return new ResponseViewModel<string>(null, "Conversation deleted successfully.", true, 200);
            }
            else
            {
                // Just remove the user from the conversation
                // Notify other users before removal
                await _chatNotificationService.NotifyUserLeftConversation(
                    conversationId,
                    userId,
                    currentUser?.User?.FullName ?? "Unknown User");

                // Remove user from conversation
                await _conversationUserRepo.RemoveAsync(conversationId, userId);

                return new ResponseViewModel<string>(null, "Left conversation successfully.", true, 200);
            }
        }

    }
}
