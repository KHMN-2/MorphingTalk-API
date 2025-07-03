using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MorphingTalk_API.DTOs;
using MorphingTalk_API.DTOs.Auth;
using Application.Interfaces.Services.Authentication;
using Application.Services.Authentication;
using static Application.Services.Authentication.OTPService;
using Application.DTOs.Auth;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;

        public AuthController(IAuthService authService, UserManager<User> userManager)
        {
            this._userManager = userManager;
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
                Gender = model.gender,
                NativeLanguage = model.nativeLanguage,
                AboutStatus = model.aboutStatus,
                ProfilePicturePath = model.profilePicturePath,
                IsDeactivated = false,
                IsFirstLogin = true,
                PastProfilePicturePaths = [model.profilePicturePath]
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
            catch (Exception e)
            {
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
            catch (Exception e)
            {
                return Unauthorized(new AuthResponseDto
                {
                    Success = false,
                    Message = e.Message
                });
            }
        }

        [HttpPost("login/firebase")]
        public async Task<IActionResult> LoginWithFirebase(FirebaseLoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid Firebase login details"
                });

            try
            {
                var token = await _authService.LoginWithFirebase(model.IdToken);
                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "Firebase login successful",
                    Token = token
                });
            }
            catch (Exception e)
            {
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
                string token = await _authService.VerifyOTP(OTPDto.email, new OTP
                {
                    OTPValue = OTPDto.OTP,
                    OTPType = OTPFor.VerifyEmail
                });
                if (token != null)
                {

                    return Ok(new AuthResponseDto
                    {
                        Success = true,
                        Message = "Email confirmed",
                        Token = token
                    });
                }
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
        [HttpPost("CheckAccount")]
        public async Task<IActionResult> CheckAccount(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }
                if (user.EmailConfirmed)
                {
                    return Ok(new AuthResponseDto
                    {
                        Success = true,
                        Message = "User exists and is confirmed"
                    });
                }
                else
                {
                    return Ok(new AuthResponseDto
                    {
                        Success = true,
                        Message = "User exists but is not confirmed"
                    });
                }
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

        [HttpGet("ForgetEmail")]
        public async Task<IActionResult> ForgetEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }
                await _authService.ForgetPassword(user);

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




        [HttpPost("VerifyOTPToForgetEmail")]
        public async Task<IActionResult> VerifyOTPToForgetEmail(OTPDto OTPDto)
        {
            try
            {
                string token = await _authService.VerifyOTP(OTPDto.email, new OTP
                {
                    OTPValue = OTPDto.OTP,
                    OTPType = OTPFor.ResetPassword
                });
                if (token != null)
                {

                    return Ok(new AuthResponseDto
                    {
                        Success = true,
                        Message = "Correct OTP",
                        Token = token
                    });
                }
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
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid password reset details"
                });
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                var result = await _authService.ResetPassword(user, model.NewPassword, model.Token);
                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "Password reset successfully"
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
