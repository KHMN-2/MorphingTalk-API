using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Authentication;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using static Application.Services.Authentication.OTPService;

namespace Application.Services.Authentication
{
    public class AuthService : IAuthService
    {

         private readonly IMemoryCache _memoryCache;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IOTPService _OTPService;
        private readonly IUserRepository _userRepository;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, IOTPService OTPService, IUserRepository userRepository, IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _OTPService = OTPService;
            _userRepository = userRepository;
            _memoryCache = memoryCache;
        }

        public async Task<bool> ForgetPassword(User user)
        {
            return await _OTPService.SendOTPToResetPassword(user); ;
        }


        public async Task<bool> ResetPassword(User user, string newPassword, string token)
        {
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                return true;
            }
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        }
        public async Task<bool> SendOTP(string email)
        {
            await _OTPService.SendOTPToVerfiyEmail(email);
            return true;
        }
        public async Task<string?> VerifyOTP(string email, OTP otp)
        {
            if (_OTPService.VerifyOTP(email, otp))
            {
                var user = await _userManager.FindByEmailAsync(email);
                string token = "";
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                if (otp.OTPType == OTPFor.VerifyEmail)
                {
                    await _userRepository.ConfirmEmail(email);
                    token = _tokenService.GenerateJwtToken(user);
                }else if (otp.OTPType == OTPFor.ResetPassword)
                {
                    token = await _userManager.GeneratePasswordResetTokenAsync(user);
                }
                return token;
            }
            return null;
        }


        public async Task<string> Login(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (!user.EmailConfirmed)
                {
                    throw new Exception("Email not confirmed");
                }
                var token = _tokenService.GenerateJwtToken(user);
                return token;
            }
            throw new Exception("Invalid login details");
        }

        public async Task<string> Register(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                //await _OTPService.SendOTP(user.Email);
                return "";
            }

            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
