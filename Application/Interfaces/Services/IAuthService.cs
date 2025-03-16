using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Users;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<string> Login(string email, string password);
        public Task<string> Register(User user, string password);
        public Task<string> RefreshToken(string token);
        public Task<bool> ForgotPassword(string email);
        public Task<bool> SendOTP(string email);
        public Task<bool> VerifyOTP(string email, string otp);

    }
}
