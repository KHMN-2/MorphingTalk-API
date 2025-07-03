using Infrastructure.Repositories;
using Scalar.AspNetCore;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.AspNetCore.Identity;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MorphingTalk_API.Extensions;
using MorphingTalk_API.Hubs;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add API Explorer services which are required for OpenAPI generation
builder.Services.AddEndpointsApiExplorer();
// setup cors to allow any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins",
        builder =>
        {
            builder.SetIsOriginAllowed(origin =>
                {
                    if (string.IsNullOrEmpty(origin)) return false;
                    
                    var uri = new Uri(origin);
                    
                    // Allow localhost for development
                    if (uri.Host == "localhost") return true;
                    
                    // Allow specific IP for development
                    if (uri.Host == "74.243.249.143") return true;
                    
                    // Allow all Vercel deployments (main app and previews)
                    if (uri.Host.EndsWith(".vercel.app")) return true;
                    
                    return false;
                })
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.ConfigureService(builder.Configuration);

var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseCors("AllowedOrigins"); // Apply comprehensive CORS policy

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
