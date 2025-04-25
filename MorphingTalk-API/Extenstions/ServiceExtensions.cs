using Microsoft.EntityFrameworkCore;
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
using MorphingTalk.API.Hubs;
using Application.Automapper;

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
            //services.AddScoped<IMessageHandler, VoiceMessageHandler>();
            //services.AddScoped<IMessageHandler, TextMessageHandler>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IConversationUserRepository, ConversationUserRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();


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
            }).AddJwtBearer(options =>
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

            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.AddTransient<EmailService>();

			services.AddAutoMapper(typeof(MappingProfile));

		}
	}
}






