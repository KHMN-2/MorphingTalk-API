using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MorphingTalk_API.DTOs;
using MorphingTalk_API.DTOs.Auth;
using Application.Interfaces.Services.Authentication;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            this._authService = authService;
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

            try
            {
                var token = await _authService.Register(user, model.Password);
                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "User registered successfully",
                    Token = token
                });
            }
            catch(Exception e) {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = e.Message
                });
            }
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

            try
            {
                var token = await _authService.Login(model.Email, model.Password);
                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token
                });

            }
            catch (Exception e) {
                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Message = e.Message
                });
            }
        }

        [HttpGet("SendOTP")]
        public async Task<IActionResult> SendOTP(string email)
        {
            try
            {
                await _authService.SendOTP(email);
                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "OTP sent successfully"
                });
            }
            catch (Exception e)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = e.Message
                });
            }
        }

        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP(OTPDto OTPDto)
        {
            try
            {
                if (await _authService.VerifyOTP(OTPDto.email, OTPDto.OTP))
                    return Ok(new AuthResponseDto
                    {
                        Success = true,
                        Message = "Email confirmed"
                    });
                else
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Wrong or expired OTP"
                    });
            }
            catch (Exception e)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = e.Message
                });
            }
        }
    }
}
