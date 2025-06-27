using Application.DTOs;
using Application.DTOs.Friendship;

namespace Application.Interfaces.Services
{
    public interface IFriendshipService
    {
        Task<ResponseViewModel<List<FriendshipDto>>> GetFriendsAsync(string userId);
        Task<ResponseViewModel<List<FriendshipDto>>> GetBlockedUsersAsync(string userId);
        Task<ResponseViewModel<string>> AddFriendAsync(string userId, string friendEmail);
        Task<ResponseViewModel<string>> RemoveFriendAsync(string userId, string friendEmail);
        Task<ResponseViewModel<string>> BlockUserAsync(string userId, string userToBlockEmail);
        Task<ResponseViewModel<string>> UnblockUserAsync(string userId, string userToUnblockEmail);
        Task<bool> IsUserBlockedAsync(string userId, string otherUserId);
        Task<bool> DidUserBlockAsync(string blockerUserId, string blockedUserId);
    }
} 