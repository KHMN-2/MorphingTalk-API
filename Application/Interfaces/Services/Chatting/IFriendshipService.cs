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
    }
} 