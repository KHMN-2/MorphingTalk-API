using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MorphingTalk_API.DTOs;
using Application.Interfaces.Services;
using MorphingTalk_API.DTOs.Auth;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid registration details"
                });

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                CreatedOn = DateTime.Now,
                LastUpdatedOn = DateTime.Now,
                IsDeactivated = false,
                IsFirstLogin = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                });

            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid login details"
                });

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (!result.Succeeded)
                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid login attempt"
                });

            var user = await _userManager.FindByEmailAsync(model.Email);
            var token = _tokenService.GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token
            });
        }
    }
}
