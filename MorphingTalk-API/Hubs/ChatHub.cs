using Application.DTOs.Chatting;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace MorphingTalk_API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            _logger.LogInformation($"User {Context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value} joined conversation {conversationId}");
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
            _logger.LogInformation($"User {Context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value} left conversation {conversationId}");
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}, User: {Context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}, User: {Context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
