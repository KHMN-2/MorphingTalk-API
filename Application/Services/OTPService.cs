using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Services;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using MorphingTalk_API.Services;

namespace Application.Services
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

        public string? GetOTP(string email)
        {
            _memoryCache.TryGetValue(email, out string? otp);
            return otp;
        }

        public void SaveOTP(string email, string otp)
        {
            var expirationTime = DateTimeOffset.UtcNow.AddMinutes(5);
            _memoryCache.Set(email, otp, expirationTime);
        }

        public async Task SendOTP(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            // Send verification email
            var subject = "Morphinh Talk - Email Verification";
            var body = $"Your OTP is: {otp}.";

            await _emailService.SendEmailAsync(email, subject, body);

            // Save OTP
            SaveOTP(email, otp);
        }

        public async Task<bool> SendOTPToResetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email");
            }

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

            await _emailService.SendEmailAsync(email, subject, body);
            SaveOTP(email, otp);

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
            SaveOTP(newEmail, otp);

            return true;
        }

        public bool VerifyOTP(string email, string otp)
        {
            string? originalOTP = GetOTP(email);

            if (originalOTP == null)
            {
                return false;
            }

            if (otp == originalOTP)
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
