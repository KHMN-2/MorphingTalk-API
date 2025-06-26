using Application.DTOs.Chatting;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MorphingTalk_API.Hubs
{    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        
        // In-memory storage for user connections and typing states
        // In production, consider using Redis or another distributed cache
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();
        private static readonly ConcurrentDictionary<string, HashSet<string>> _typingUsers = new();
        private static readonly ConcurrentDictionary<string, DateTime> _userLastSeen = new();

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }        
        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            _logger.LogInformation($"User {userId} (Connection: {Context.ConnectionId}) joined conversation group {conversationId}");
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
        }        // Typing Indicator Methods
        
        /// <summary>
        /// User started typing in a conversation
        /// </summary>        [HubMethodName("StartTyping")]
        public async Task StartTyping(string conversationId)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            _logger.LogInformation($"StartTyping called by {userId} for conversation {conversationId}");
            
            // Track typing state
            var conversationTypingKey = $"{conversationId}";
            _typingUsers.AddOrUpdate(conversationTypingKey, 
                new HashSet<string> { userId }, 
                (key, existing) => { existing.Add(userId); return existing; });

            var typingIndicator = new TypingIndicatorDto
            {
                UserId = userId,
                ConversationId = conversationId,
                IsTyping = true,
                Timestamp = DateTime.UtcNow
            };
            
            _logger.LogInformation($"Broadcasting UserStartedTyping to group {conversationId} excluding connection {Context.ConnectionId}");
            
            // Notify other users in the conversation that this user is typing
            await Clients.GroupExcept(conversationId, Context.ConnectionId)
                .SendAsync("UserStartedTyping", typingIndicator);
                
            _logger.LogInformation($"UserStartedTyping broadcast completed for user {userId}");
        }/// <summary>
        /// User stopped typing in a conversation
        /// </summary>        [HubMethodName("StopTyping")]
        public async Task StopTyping(string conversationId)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            _logger.LogInformation($"StopTyping called by {userId} for conversation {conversationId}");
            
            // Remove from typing state
            var conversationTypingKey = $"{conversationId}";
            if (_typingUsers.TryGetValue(conversationTypingKey, out var typingUsers))
            {
                typingUsers.Remove(userId);
                if (typingUsers.Count == 0)
                {
                    _typingUsers.TryRemove(conversationTypingKey, out _);
                }
            }

            var typingIndicator = new TypingIndicatorDto
            {
                UserId = userId,
                ConversationId = conversationId,
                IsTyping = false,
                Timestamp = DateTime.UtcNow
            };
            
            _logger.LogInformation($"Broadcasting UserStoppedTyping to group {conversationId} excluding connection {Context.ConnectionId}");
            
            // Notify other users in the conversation that this user stopped typing
            await Clients.GroupExcept(conversationId, Context.ConnectionId)
                .SendAsync("UserStoppedTyping", typingIndicator);
                
            _logger.LogInformation($"UserStoppedTyping broadcast completed for user {userId}");
        }/// <summary>
        /// Get currently typing users in a conversation
        /// </summary>
        public async Task GetTypingUsers(string conversationId)
        {
            var conversationTypingKey = $"{conversationId}";
            var typingUsers = _typingUsers.GetValueOrDefault(conversationTypingKey, new HashSet<string>());
            
            await Clients.Caller.SendAsync("TypingUsersResponse", conversationId, typingUsers.ToArray(), DateTime.UtcNow);
        }        // Online Status Methods
        
        /// <summary>
        /// Set user as online/offline
        /// </summary>
        public async Task SetOnlineStatus(bool isOnline)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            _logger.LogInformation($"User {userId} set online status to {isOnline}");
            
            // Update last seen when going offline
            if (!isOnline)
            {
                _userLastSeen[userId] = DateTime.UtcNow;
            }

            var statusDto = new UserStatusDto
            {
                UserId = userId,
                IsOnline = isOnline,
                Timestamp = DateTime.UtcNow,
                LastSeen = isOnline ? null : DateTime.UtcNow
            };
            
            // Broadcast to all connected clients that this user's status changed
            await Clients.All.SendAsync("UserStatusChanged", statusDto);
        }

        /// <summary>
        /// Update user's last seen timestamp
        /// </summary>
        public async Task UpdateLastSeen()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            var timestamp = DateTime.UtcNow;
            
            _userLastSeen[userId] = timestamp;
            
            // Broadcast last seen update to all connected clients
            await Clients.All.SendAsync("UserLastSeenUpdated", userId, timestamp);
        }

        /// <summary>
        /// Get online status of specific users
        /// </summary>
        public async Task GetUsersOnlineStatus(string[] userIds)
        {
            var requestingUserId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            _logger.LogInformation($"User {requestingUserId} requested status for {userIds.Length} users");
            
            var responseDto = new OnlineStatusResponseDto
            {
                RequestTimestamp = DateTime.UtcNow
            };

            foreach (var userId in userIds)
            {
                var isOnline = IsUserOnline(userId);
                var lastSeen = _userLastSeen.GetValueOrDefault(userId);
                
                responseDto.UserStatuses[userId] = new UserStatusDto
                {
                    UserId = userId,
                    IsOnline = isOnline,
                    Timestamp = DateTime.UtcNow,
                    LastSeen = lastSeen == default ? null : lastSeen
                };
            }
            
            await Clients.Caller.SendAsync("OnlineStatusResponse", responseDto);
        }

        /// <summary>
        /// Check if a user is currently online (has active connections)
        /// </summary>
        private bool IsUserOnline(string userId)
        {
            return _userConnections.ContainsKey(userId) && _userConnections[userId].Count > 0;
        }

        /// <summary>
        /// Get list of all online users
        /// </summary>
        public async Task GetOnlineUsers()
        {
            var onlineUsers = _userConnections.Keys.ToArray();
            await Clients.Caller.SendAsync("OnlineUsersResponse", onlineUsers, DateTime.UtcNow);
        }        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            _logger.LogInformation($"Client connected: {Context.ConnectionId}, User: {userId}");
            
            // Track user connections
            _userConnections.AddOrUpdate(userId, 
                new HashSet<string> { Context.ConnectionId },
                (key, existing) => { existing.Add(Context.ConnectionId); return existing; });

            var statusDto = new UserStatusDto
            {
                UserId = userId,
                IsOnline = true,
                Timestamp = DateTime.UtcNow,
                LastSeen = null
            };
            
            // Set user as online when they connect
            await Clients.All.SendAsync("UserStatusChanged", statusDto);
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}, User: {userId}");
            
            // Remove connection tracking
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                connections.Remove(Context.ConnectionId);
                if (connections.Count == 0)
                {
                    _userConnections.TryRemove(userId, out _);
                    
                    // Update last seen
                    _userLastSeen[userId] = DateTime.UtcNow;

                    var statusDto = new UserStatusDto
                    {
                        UserId = userId,
                        IsOnline = false,
                        Timestamp = DateTime.UtcNow,
                        LastSeen = DateTime.UtcNow
                    };
                    
                    // Set user as offline when all connections are gone
                    await Clients.All.SendAsync("UserStatusChanged", statusDto);
                }
            }
            
            // Clean up typing indicators for this user
            foreach (var kvp in _typingUsers.ToList())
            {
                if (kvp.Value.Contains(userId))
                {
                    kvp.Value.Remove(userId);
                    if (kvp.Value.Count == 0)
                    {
                        _typingUsers.TryRemove(kvp.Key, out _);
                    }
                    
                    // Notify that user stopped typing
                    var typingIndicator = new TypingIndicatorDto
                    {
                        UserId = userId,
                        ConversationId = kvp.Key,
                        IsTyping = false,
                        Timestamp = DateTime.UtcNow
                    };
                    
                    await Clients.Group(kvp.Key).SendAsync("UserStoppedTyping", typingIndicator);
                }
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
