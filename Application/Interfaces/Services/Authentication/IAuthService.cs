using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services.Authentication;
using Domain.Entities.Users;
using static Application.Services.Authentication.AuthService;
using static Application.Services.Authentication.OTPService;

namespace Application.Interfaces.Services.Authentication
{
    
    public interface IAuthService
    {
        public Task<string> Login(string email, string password);
        public Task<string> LoginWithFirebase(string idToken);
        public Task<string> Register(User user, string password);
        public Task<bool> ResetPassword(User user, string password, string token);
        public Task<bool> SendOTP(string email);
        public Task<string?> VerifyOTP(string email, OTP otp);
        public Task<bool> ForgetPassword(User user);


    }
}
