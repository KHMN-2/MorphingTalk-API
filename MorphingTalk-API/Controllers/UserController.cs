using Application.DTOs.UserDto;
using Application.Interfaces.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("GetLoggedInUser")]
        [Authorize]
        public async Task<IActionResult> GetUserById()
        {
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IsFirstLogin = user.IsFirstLogin,
                NativeLanguage = user.NativeLanguage,
                AboutStatus = user.AboutStatus,
                Gender = user.Gender,
                ProfilePicturePath = user.ProfilePicturePath,
                PastProfilePicturePath = user.PastProfilePicturePaths
            };
            return Ok(user);
        }
    }
}
