using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Users;
using static Application.Services.Authentication.OTPService;

namespace Application.Interfaces.Services.Authentication
{
    public interface IOTPService
    {
        public void SaveOTP(string email, OTP otp);
        public OTP? GetOTP(string email);
        public Task SendOTPToVerfiyEmail(string email);
        public bool VerifyOTP(string email, OTP otp);
        public Task<bool> SendOTPToResetPassword(User user);
        public Task<bool> SendOTPToChangeEmail(string newEmail, string token);

    }
}
