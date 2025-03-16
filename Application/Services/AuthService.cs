using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using MorphingTalk_API.Services;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IOTPService _OTPService;
        private readonly IUserRepository _userRepository;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, IOTPService OTPService, IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _OTPService = OTPService;
            _userRepository = userRepository;
        }

        public async Task<bool> ForgotPassword(string email)
        {
            throw new NotImplementedException();

        }
        public async Task<bool> SendOTP(string email) {
            await _OTPService.SendOTP(email);
            return true;
        }
        public async Task<bool> VerifyOTP(string email, string otp)
        {
            if (_OTPService.VerifyOTP(email, otp)) {
                await _userRepository.ConfirmEmail(email);
                return true;
            }

            return false;
        }


        public async Task<string> Login(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if(result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (!user.EmailConfirmed) {
                    throw new Exception("Email not confirmed");
                }
                var token = _tokenService.GenerateJwtToken(user);
                return token;
            }
            throw new Exception("Invalid login details");
        }

        public Task<string> RefreshToken(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Register(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var token = _tokenService.GenerateJwtToken(user);
                return token;
            }
            await _OTPService.SendOTP(user.Email);

            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }


    }
}
