using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Data;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Infrastructure.Repositories;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using MorphingTalk_API.Services;
using Microsoft.AspNetCore.Authentication;

namespace MorphingTalk_API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureService(this IServiceCollection services, IConfiguration configuration)
        {
            // Add database context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            // Add Identity services
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddMemoryCache();



            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<IdentityDbContext>();

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
        }
    }
}






