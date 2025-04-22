using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Authentication
{
    public interface IOTPService
    {
        public void SaveOTP(string email, string otp);
        public string? GetOTP(string email);
        public Task SendOTP(string email);
        public bool VerifyOTP(string email, string otp);
        public Task<bool> SendOTPToResetPassword(string email);
        public Task<bool> SendOTPToChangeEmail(string newEmail, string token);

    }
}
