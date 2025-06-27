using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Users;

namespace Application.Interfaces.Repositories
{
    public interface IFriendshipRepository
    {
        Task<bool> IsFriendAsync(string userId, string friendId);
        Task<bool> IsBlockedAsync(string userId, string friendId);
        Task AddFriendRelationAsync(Friendship friendRelation);
        Task UpdateFriendRelationAsync(Friendship friendRelation);
        Task RemoveFriendRelationAsync(Friendship friendRelation);
        Task<List<Friendship>> GetFriendsAsync(string userId);
        Task<List<Friendship>> GetBlockedUsersAsync(string userId);
        Task<Friendship> GetFriendshipAsync(string userId, string friendId);

    }
}
