using Application.DTOs.Chatting;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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
        }        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            _logger.LogInformation($"User {userId} joined conversation {conversationId}");
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            _logger.LogInformation($"User {userId} left conversation {conversationId}");
        }

        // WebRTC Call Signaling Methods
        
        /// <summary>
        /// Join a call room for WebRTC signaling
        /// </summary>
        public async Task JoinCall(string conversationId, string userId)
        {
            var callGroup = $"call_{conversationId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, callGroup);
            
            _logger.LogInformation($"User {userId} joined call in conversation {conversationId}");
            
            // Notify others in the conversation that someone joined the call
            await Clients.GroupExcept(callGroup, Context.ConnectionId)
                .SendAsync("UserJoinedCall", userId, conversationId);
        }

        /// <summary>
        /// Leave a call room
        /// </summary>
        public async Task LeaveCall(string conversationId, string userId)
        {
            var callGroup = $"call_{conversationId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, callGroup);
            
            _logger.LogInformation($"User {userId} left call in conversation {conversationId}");
            
            // Notify others that user left the call
            await Clients.Group(callGroup)
                .SendAsync("UserLeftCall", userId, conversationId);
        }

        /// <summary>
        /// Send WebRTC offer to initiate a call
        /// </summary>
        public async Task SendOffer(string conversationId, string targetUserId, object offer)
        {
            _logger.LogInformation($"Sending WebRTC offer from {Context.UserIdentifier} to {targetUserId} in conversation {conversationId}");
            
            await Clients.User(targetUserId)
                .SendAsync("ReceiveOffer", Context.UserIdentifier, offer, conversationId);
        }

        /// <summary>
        /// Send WebRTC answer in response to an offer
        /// </summary>
        public async Task SendAnswer(string conversationId, string targetUserId, object answer)
        {
            _logger.LogInformation($"Sending WebRTC answer from {Context.UserIdentifier} to {targetUserId} in conversation {conversationId}");
            
            await Clients.User(targetUserId)
                .SendAsync("ReceiveAnswer", Context.UserIdentifier, answer, conversationId);
        }

        /// <summary>
        /// Send ICE candidates for WebRTC connection establishment
        /// </summary>
        public async Task SendIceCandidate(string conversationId, string targetUserId, object candidate)
        {
            _logger.LogInformation($"Sending ICE candidate from {Context.UserIdentifier} to {targetUserId} in conversation {conversationId}");
            
            await Clients.User(targetUserId)
                .SendAsync("ReceiveIceCandidate", Context.UserIdentifier, candidate, conversationId);
        }

        /// <summary>
        /// Invite user to a call
        /// </summary>
        public async Task InviteToCall(string conversationId, string targetUserId, string callType)
        {
            _logger.LogInformation($"Call invitation from {Context.UserIdentifier} to {targetUserId} in conversation {conversationId}, type: {callType}");
            
            await Clients.User(targetUserId)
                .SendAsync("CallInvitation", Context.UserIdentifier, conversationId, callType);
        }

        /// <summary>
        /// Respond to a call invitation
        /// </summary>
        public async Task RespondToCall(string conversationId, string callerId, bool accepted)
        {
            _logger.LogInformation($"Call response from {Context.UserIdentifier} to {callerId} in conversation {conversationId}: {(accepted ? "accepted" : "declined")}");
            
            await Clients.User(callerId)
                .SendAsync("CallResponse", Context.UserIdentifier, conversationId, accepted);
        }

        /// <summary>
        /// End an active call
        /// </summary>
        public async Task EndCall(string conversationId)
        {
            var callGroup = $"call_{conversationId}";
            _logger.LogInformation($"Call ended by {Context.UserIdentifier} in conversation {conversationId}");
            
            // Notify all participants that the call has ended
            await Clients.Group(callGroup)
                .SendAsync("CallEnded", Context.UserIdentifier, conversationId);
        }        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            _logger.LogInformation($"Client connected: {Context.ConnectionId}, User: {userId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}, User: {userId}");
            
            // Clean up any call groups the user was in
            // Note: SignalR automatically removes from groups on disconnect
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
