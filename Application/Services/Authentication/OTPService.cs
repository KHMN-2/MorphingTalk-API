using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Services.Authentication;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services.Authentication
{
    public class OTPService : IOTPService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly EmailService _emailService;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public OTPService(IMemoryCache memoryCache, EmailService emailService, UserManager<User> userManager, ITokenService tokenService)
        {
            _memoryCache = memoryCache;
            _emailService = emailService;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public enum OTPFor
        {
            VerifyEmail,
            ResetPassword,
            ChangeEmail
        }
        public class OTP
        {
            public string OTPValue { get; set; }
            public OTPFor OTPType { get; set; }
        }
        public OTP? GetOTP(string email)
        {
            _memoryCache.TryGetValue(email, out OTP? otp);
            return otp;
        }

        public void SaveOTP(string email, OTP otp)
        {
            var expirationTime = DateTimeOffset.UtcNow.AddMinutes(5);
            _memoryCache.Set(email, otp, expirationTime);
        }

        public async Task SendOTPToVerfiyEmail(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            // Send verification email
            var subject = "Morphinh Talk - Email Verification";
            var body = $"Your OTP is: {otp}.";

            await _emailService.SendEmailAsync(email, subject, body);

            // Save OTP
            SaveOTP(email, new OTP
            {
                OTPValue = otp,
                OTPType = OTPFor.VerifyEmail
            });
        }

        public async Task<bool> SendOTPToResetPassword(User user)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            // Send verification email
            var subject = "Morphinh Talk - Reset Password";
            var body = $@"
        <html>
        <head>
            <style>
                .container {{
                    font-family: Arial, sans-serif;
                    margin: 0 auto;
                    padding: 20px;
                    max-width: 600px;
                    background-color: #f9f9f9;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                }}
                .header {{
                    text-align: center;
                    padding-bottom: 20px;
                }}
                .otp {{
                    font-size: 24px;
                    font-weight: bold;
                    color: #333;
                }}
                .footer {{
                    text-align: center;
                    padding-top: 20px;
                    font-size: 12px;
                    color: #777;
                }}
                .button {{
                    display: inline-block;
                    padding: 10px 20px;
                    font-size: 16px;
                    color: #fff;
                    background-color: #007bff;
                    text-decoration: none;
                    border-radius: 5px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h2>Morphinh Talk - Reset Password</h2>
                </div>
                <p>Dear {user.FullName},</p>
                <p>We received a request to reset your password. Please use the following One-Time Password (OTP) to reset your password:</p>
                <p class='otp'>{otp}</p>
                <p>This OTP is valid for 5 minutes. If you did not request a password reset, please ignore this email.</p>
                <div class='footer'>
                    <p>&copy; 2025 Morphinh Talk. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";

            await _emailService.SendEmailAsync(user.Email, subject, body);
            SaveOTP(user.Email, new OTP
            {
                OTPValue = otp,
                OTPType = OTPFor.ResetPassword
            });
            var test = GetOTP(user.Email);
            
            return true;
        }

        public async Task<bool> SendOTPToChangeEmail(string newEmail, string token)
        {
            var user = await _tokenService.GetUserFromToken(token);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            var otp = new Random().Next(100000, 999999).ToString();

            // Send verification email
            var subject = "Morphinh Talk - Change Email";
            var body = $@"
        <html>
        <head>
            <style>
                .container {{
                    font-family: Arial, sans-serif;
                    margin: 0 auto;
                    padding: 20px;
                    max-width: 600px;
                    background-color: #f9f9f9;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                }}
                .header {{
                    text-align: center;
                    padding-bottom: 20px;
                }}
                .otp {{
                    font-size: 24px;
                    font-weight: bold;
                    color: #333;
                }}
                .footer {{
                    text-align: center;
                    padding-top: 20px;
                    font-size: 12px;
                    color: #777;
                }}
                .button {{
                    display: inline-block;
                    padding: 10px 20px;
                    font-size: 16px;
                    color: #fff;
                    background-color: #007bff;
                    text-decoration: none;
                    border-radius: 5px;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h2>Morphinh Talk - Change Email Verification</h2>
                </div>
                <p>Dear {user.FullName},</p>
                <p>We received a request to change your email address. Please use the following One-Time Password (OTP) to verify your new email address:</p>
                <p class='otp'>{otp}</p>
                <p>This OTP is valid for 5 minutes. If you did not request this change, please ignore this email.</p>
                <div class='footer'>
                    <p>&copy; 2025 Morphinh Talk. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";

            await _emailService.SendEmailAsync(newEmail, subject, body);
            SaveOTP(newEmail, new OTP {
                OTPType = OTPFor.ChangeEmail,
                OTPValue = otp
            });

            return true;
        }

        public bool VerifyOTP(string email, OTP otp)
        {
            OTP? originalOTP = GetOTP(email);

            if (originalOTP == null)
            {
                return false;
            }

            if (otp.OTPValue == originalOTP.OTPValue && otp.OTPType == originalOTP.OTPType)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
