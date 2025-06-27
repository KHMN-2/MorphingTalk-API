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
    options.AddPolicy("reactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "http://localhost:3001")
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

app.UseCors("reactApp"); // Apply CORS policy
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
