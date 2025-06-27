using Application.DTOs;
using Application.DTOs.Friendship;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities.Users;

namespace Application.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConversationRepository _conversationRepository;

        public FriendshipService(
            IFriendshipRepository friendshipRepository,
            IUserRepository userRepository,
            IConversationRepository conversationRepository)
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
            _conversationRepository = conversationRepository;
        }

        public async Task<ResponseViewModel<List<FriendshipDto>>> GetFriendsAsync(string userId)
        {
            var friends = await _friendshipRepository.GetFriendsAsync(userId);
            var results = friends.Select(fr => new FriendshipDto
            {
                UserId = fr.UserId1 == userId ? fr.UserId2 : fr.UserId1,
                Name = fr.UserId1 == userId ? fr.User2?.FullName : fr.User1?.FullName,
                Email = fr.UserId1 == userId ? fr.User2?.Email : fr.User1?.Email,
                IsOnline = true,
                LastSeen = DateTime.UtcNow,
                ProfileImagePath = fr.UserId1 == userId ? fr.User2?.ProfilePicturePath : fr.User1?.ProfilePicturePath
            }).ToList();

            return new ResponseViewModel<List<FriendshipDto>>(results, "Friends retrieved successfully", true, 200);
        }

        public async Task<ResponseViewModel<List<FriendshipDto>>> GetBlockedUsersAsync(string userId)
        {
            var blockedUsers = await _friendshipRepository.GetBlockedUsersAsync(userId);
            var results = blockedUsers.Select(fr => new FriendshipDto
            {
                UserId = fr.UserId1 == userId ? fr.UserId2 : fr.UserId1,
                Name = fr.UserId1 == userId ? fr.User2?.FullName : fr.User1?.FullName,
                Email = fr.UserId1 == userId ? fr.User2?.Email : fr.User1?.Email,
                IsOnline = true,
                LastSeen = DateTime.UtcNow,
                ProfileImagePath = fr.UserId1 == userId ? fr.User2?.ProfilePicturePath : fr.User1?.ProfilePicturePath
            }).ToList();

            return new ResponseViewModel<List<FriendshipDto>>(results, "Blocked users retrieved successfully", true, 200);
        }

        public async Task<ResponseViewModel<string>> AddFriendAsync(string userId, string friendEmail)
        {
            User friendUser;
            try
            {
                friendUser = await _userRepository.GetUserByEmailAsync(friendEmail);
            }
            catch (Exception)
            {
                return new ResponseViewModel<string>(null, "Friend not found", false, 404);
            }

            var friendId = friendUser.Id;
            var isFriend = await _friendshipRepository.IsFriendAsync(userId, friendId);
            if (isFriend)
            {
                return new ResponseViewModel<string>(null, "Already friends", false, 400);
            }

            if (userId.CompareTo(friendId) > 0)
            {
                var temp = userId;
                userId = friendId;
                friendId = temp;
            }

            var friendship = new Friendship
            {
                UserId1 = userId,
                UserId2 = friendId,
                IsBlocked = false
            };

            await _friendshipRepository.AddFriendRelationAsync(friendship);
            return new ResponseViewModel<string>("Friendship created successfully", "Friend added successfully", true, 200);
        }

        public async Task<ResponseViewModel<string>> RemoveFriendAsync(string userId, string friendEmail)
        {
            User friendUser;
            try
            {
                friendUser = await _userRepository.GetUserByEmailAsync(friendEmail);
            }
            catch (Exception)
            {
                return new ResponseViewModel<string>(null, "Friend not found", false, 404);
            }

            var friendId = friendUser.Id;

            if (userId.CompareTo(friendId) > 0)
            {
                var temp = userId;
                userId = friendId;
                friendId = temp;
            }

            var friendship = await _friendshipRepository.GetFriendshipAsync(userId, friendId);
            if (friendship == null)
            {
                return new ResponseViewModel<string>(null, "Friendship not found", false, 404);
            }

            await _friendshipRepository.RemoveFriendRelationAsync(friendship);

            var conversation = await _conversationRepository.GetOneToOneConversationAsync(userId, friendId);
            if (conversation != null)
            {
                await _conversationRepository.DeleteAsync(conversation.Id);
            }

            return new ResponseViewModel<string>("Friendship removed successfully", "Friend removed successfully", true, 200);
        }

        public async Task<ResponseViewModel<string>> BlockUserAsync(string userId, string userToBlockEmail)
        {
            User userToBlock;
            try
            {
                userToBlock = await _userRepository.GetUserByEmailAsync(userToBlockEmail);
            }
            catch (Exception)
            {
                return new ResponseViewModel<string>(null, "User not found", false, 404);
            }

            var userToBlockId = userToBlock.Id;

            if (userId == userToBlockId)
            {
                return new ResponseViewModel<string>(null, "Cannot block yourself", false, 400);
            }

            // Check if already blocked
            var isBlocked = await _friendshipRepository.IsBlockedAsync(userId, userToBlockId);
            if (isBlocked)
            {
                return new ResponseViewModel<string>(null, "User is already blocked", false, 400);
            }

            // Get existing friendship or create new one
            var existingFriendship = await _friendshipRepository.GetFriendshipAsync(userId, userToBlockId);
            
            if (existingFriendship != null)
            {
                // Update existing relationship to blocked
                existingFriendship.IsBlocked = true;
                existingFriendship.UpdatedAt = DateTime.UtcNow;
                await _friendshipRepository.UpdateFriendRelationAsync(existingFriendship);
            }
            else
            {
                // Create new blocked relationship
                if (userId.CompareTo(userToBlockId) > 0)
                {
                    var temp = userId;
                    userId = userToBlockId;
                    userToBlockId = temp;
                }

                var blockedRelationship = new Friendship
                {
                    UserId1 = userId,
                    UserId2 = userToBlockId,
                    IsBlocked = true,
                    UpdatedAt = DateTime.UtcNow
                };

                await _friendshipRepository.AddFriendRelationAsync(blockedRelationship);
            }

            return new ResponseViewModel<string>("User blocked successfully", "User blocked successfully", true, 200);
        }

        public async Task<ResponseViewModel<string>> UnblockUserAsync(string userId, string userToUnblockEmail)
        {
            User userToUnblock;
            try
            {
                userToUnblock = await _userRepository.GetUserByEmailAsync(userToUnblockEmail);
            }
            catch (Exception)
            {
                return new ResponseViewModel<string>(null, "User not found", false, 404);
            }

            var userToUnblockId = userToUnblock.Id;

            // Check if blocked
            var isBlocked = await _friendshipRepository.IsBlockedAsync(userId, userToUnblockId);
            if (!isBlocked)
            {
                return new ResponseViewModel<string>(null, "User is not blocked", false, 400);
            }

            // Get existing friendship
            var existingFriendship = await _friendshipRepository.GetFriendshipAsync(userId, userToUnblockId);
            
            if (existingFriendship != null)
            {
                // Remove the blocked relationship entirely since there's no friend relationship
                await _friendshipRepository.RemoveFriendRelationAsync(existingFriendship);
            }

            return new ResponseViewModel<string>("User unblocked successfully", "User unblocked successfully", true, 200);
        }

        public async Task<bool> IsUserBlockedAsync(string userId, string otherUserId)
        {
            return await _friendshipRepository.IsBlockedAsync(userId, otherUserId);
        }
    }
} 