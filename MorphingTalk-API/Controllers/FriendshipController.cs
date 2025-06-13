using Application.DTOs.Friendship;
using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class FriendshipController : Controller
    {
        private readonly IFriendshipService _friendshipService;

        public FriendshipController(IFriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        [HttpGet("")]
        public async Task<ActionResult<ResponseViewModel<List<FriendshipDto>>>> GetFriends()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await _friendshipService.GetFriendsAsync(userId);
            return Ok(response);
        }

        [HttpGet("blocked")]
        public async Task<ActionResult<ResponseViewModel<List<FriendshipDto>>>> GetBlockedUsers()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await _friendshipService.GetBlockedUsersAsync(userId);
            return Ok(response);
        }

        [HttpPost("add/{friendEmail}")]
        public async Task<ActionResult<ResponseViewModel<string>>> AddFriend(string friendEmail)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await _friendshipService.AddFriendAsync(userId, friendEmail);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("remove/{friendEmail}")]
        public async Task<ActionResult<ResponseViewModel<string>>> RemoveFriend(string friendEmail)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await _friendshipService.RemoveFriendAsync(userId, friendEmail);
            return StatusCode(response.StatusCode, response);
        }
    }
}
