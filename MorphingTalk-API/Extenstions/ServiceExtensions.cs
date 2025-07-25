﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Data;
using Application.Interfaces.Repositories;
using Infrastructure.Repositories;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Application.Services.Authentication;
using Application.Interfaces.Services.Authentication;
using Application.Interfaces.Services.Chatting;
using Application.Services.Chatting;
using MorphingTalk_API.Hubs;
using Application.Interfaces.Services.FileService;
using Application.Services.FileService;
using Application.Interfaces.Services;
using Application.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace MorphingTalk_API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureService(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Identity services
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddMemoryCache();
			services.AddSignalR();
            services.AddScoped<IMessageHandler, VoiceMessageHandler>();
            services.AddScoped<IMessageHandler, TextMessageHandler>();
            services.AddScoped<IMessageHandler, ImageMessageHandler>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IConversationUserRepository, ConversationUserRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IChatNotificationService, SignalRChatNotificationService>();
            services.AddScoped<IFriendshipService, FriendshipService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IAIWebhookService, AIWebhookService>();
            services.AddScoped<ITextTranslationService, AzureTranslationService>();

            services.AddScoped<IFileValidator, FileValidator>();
            services.AddScoped<IFilePathProvider, FilePathProvider>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            
            services.AddHttpClient();


            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"])
                    )
                };
            });

            // Initialize Firebase Admin SDK
            var firebaseConfig = configuration.GetSection("Firebase");
            var serviceAccountKeyPath = firebaseConfig["ServiceAccountKeyPath"];
            
            if (!string.IsNullOrEmpty(serviceAccountKeyPath) && File.Exists(serviceAccountKeyPath))
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(serviceAccountKeyPath),
                    ProjectId = firebaseConfig["ProjectId"]
                });
            }

            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.AddTransient<EmailService>();

		}
	}
}






