using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Domain.Entities.Users;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly ApplicationDbContext _context;
        public FriendshipRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> IsFriendAsync(string userId, string friendId)
        {
            if(userId.CompareTo(friendId) > 0)
            {
                var temp = userId;
                userId = friendId;
                friendId = temp;
            }
            return await _context.Friendships
                .AnyAsync(fr => (fr.UserId1 == userId && fr.UserId2 == friendId) || (fr.UserId2 == userId && fr.UserId1 == friendId) && !fr.IsBlocked);
        }
        public async Task<bool> IsBlockedAsync(string userId, string friendId)
        {
            return await _context.Friendships
                .AnyAsync(fr => ((fr.UserId1 == userId && fr.UserId2 == friendId) || (fr.UserId2 == userId && fr.UserId1 == friendId)) && fr.IsBlocked);
        }

        public async Task<bool> IsBlockedByUserAsync(string blockerUserId, string blockedUserId)
        {
            return await _context.Friendships
                .AnyAsync(fr => ((fr.UserId1 == blockerUserId && fr.UserId2 == blockedUserId) || (fr.UserId2 == blockerUserId && fr.UserId1 == blockedUserId)) && fr.IsBlocked && fr.BlockedByUserId == blockerUserId);
        }
        public async Task AddFriendRelationAsync(Friendship friendRelation)
        {
            await _context.Friendships.AddAsync(friendRelation);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateFriendRelationAsync(Friendship friendRelation)
        {
            _context.Friendships.Update(friendRelation);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveFriendRelationAsync(Friendship friendRelation)
        {
            _context.Friendships.Remove(friendRelation);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Friendship>> GetFriendsAsync(string userId)
        {
            return await _context.Friendships
                .Where(fr => (fr.UserId1 == userId || fr.UserId2 == userId) && !fr.IsBlocked)
                .Include(fr => fr.User1)
                .Include(fr => fr.User2)
                .ToListAsync();
        }
        public async Task<List<Friendship>> GetBlockedUsersAsync(string userId)
        {
            return await _context.Friendships
                .Where(fr => ((fr.UserId1 == userId || fr.UserId2 == userId) && fr.IsBlocked && fr.BlockedByUserId == userId))
                .Include(fr => fr.User1)
                .Include(fr => fr.User2)
                .ToListAsync();
        }

        public async Task<Friendship> GetFriendshipAsync(string userId, string friendId) {
            if (userId.CompareTo(friendId) > 0)
            {
                var temp = userId;
                userId = friendId;
                friendId = temp;
            }
            return await _context.Friendships
                .FirstOrDefaultAsync(fr => (fr.UserId1 == userId && fr.UserId2 == friendId) ||(fr.UserId2 == userId && fr.UserId1 == friendId));
        }

    }
}
