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
using MorphingTalk.API.Hubs;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.ConfigureService(builder.Configuration);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors("AllowAllOrigins"); // Apply CORS policy
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseRouting();
app.MapHub<ChatHub>("/chathub");

app.Run();
