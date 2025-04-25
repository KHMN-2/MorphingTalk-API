using Application.DTOs.Friendship;
using Application.Interfaces.Repositories;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class FriendshipController : Controller
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConversationRepository _conversationRepository;
        public FriendshipController(IFriendshipRepository friendshipRepository, IUserRepository userRepository, IConversationRepository conversationRepository)
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
            _conversationRepository = conversationRepository;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetFriends()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var friends = await _friendshipRepository.GetFriendsAsync(userId);
            var results = friends.Select(fr => new FriendshipDto
            {
                UserId = fr.UserId1 == userId ? fr.UserId2 : fr.UserId1,
                Username = fr.UserId1 == userId ? fr.User2?.FullName : fr.User1?.FullName,
                Email = fr.UserId1 == userId ? fr.User2?.Email : fr.User1?.Email,
            }).ToList();

            return Ok(results);
        }
        [HttpGet("blocked")]
        public async Task<IActionResult> GetBlockedUsers()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var blockedUsers = await _friendshipRepository.GetBlockedUsersAsync(userId);
            return Ok(blockedUsers);
        }

        [HttpPost("add/{friendEmail}")]
        public async Task<IActionResult> AddFriend(string friendEmail)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            User friendUser;
            try
            {
                friendUser = await _userRepository.GetUserByEmailAsync(friendEmail);
            }
            catch (Exception e)
            {
                return NotFound("Friend not found");
            }
            
    
            var friendId = friendUser.Id;
            var isFriend = await _friendshipRepository.IsFriendAsync(userId, friendId);
            if (isFriend)
            {
                return BadRequest("Already friends");
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
            return Ok();
        }
        [HttpDelete("remove/{friendEmail}")]
        public async Task<IActionResult> RemoveFriend(string friendEmail)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            User friendUser;
            try { 
                friendUser = await _userRepository.GetUserByEmailAsync(friendEmail);
     
            }catch (Exception e) { 
                return NotFound("Friend not found");
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
                return NotFound();
            }
            await _friendshipRepository.RemoveFriendRelationAsync(friendship);

            var conversation = await _conversationRepository.GetOneToOneConversationAsync(userId, friendId);
            if (conversation != null)
            {
                await _conversationRepository.DeleteAsync(conversation.Id);
            }
            return Ok();
        }
    }
}
