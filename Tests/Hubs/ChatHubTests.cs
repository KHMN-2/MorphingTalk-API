using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;
using MorphingTalk_API.Hubs;
using Application.DTOs.Chatting;

namespace Tests.Hubs
{
    public class ChatHubTests
    {
        private readonly Mock<ILogger<ChatHub>> _mockLogger;
        private readonly Mock<IHubCallerClients> _mockClients;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly Mock<IGroupManager> _mockGroups;
        private readonly Mock<HubCallerContext> _mockContext;
        private readonly Mock<ClaimsPrincipal> _mockUser;
        private readonly ChatHub _chatHub;

        public ChatHubTests()
        {
            _mockLogger = new Mock<ILogger<ChatHub>>();
            _mockClients = new Mock<IHubCallerClients>();
            _mockClientProxy = new Mock<IClientProxy>();
            _mockGroups = new Mock<IGroupManager>();
            _mockContext = new Mock<HubCallerContext>();
            _mockUser = new Mock<ClaimsPrincipal>();

            _chatHub = new ChatHub(_mockLogger.Object);

            // Setup mock context
            _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");
            _mockContext.Setup(c => c.User).Returns(_mockUser.Object);
            _mockUser.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier))
                .Returns(new Claim(ClaimTypes.NameIdentifier, "test-user-id"));

            // Setup mock clients
            _mockClients.Setup(c => c.GroupExcept(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_mockClientProxy.Object);
            _mockClients.Setup(c => c.All).Returns(_mockClientProxy.Object);
            _mockClients.Setup(c => c.Caller).Returns(_mockClientProxy.Object);

            // Assign mocks to hub
            _chatHub.Context = _mockContext.Object;
            _chatHub.Clients = _mockClients.Object;
            _chatHub.Groups = _mockGroups.Object;
        }

        [Fact]
        public async Task StartTyping_ShouldNotifyOtherUsers()
        {
            // Arrange
            var conversationId = "conversation-123";
            
            // Act
            await _chatHub.StartTyping(conversationId);

            // Assert
            _mockClients.Verify(c => c.GroupExcept(conversationId, "test-connection-id"), Times.Once);
            _mockClientProxy.Verify(c => c.SendAsync("UserStartedTyping", 
                It.Is<TypingIndicatorDto>(dto => 
                    dto.UserId == "test-user-id" && 
                    dto.ConversationId == conversationId && 
                    dto.IsTyping == true),
                default), Times.Once);
        }

        [Fact]
        public async Task StopTyping_ShouldNotifyOtherUsers()
        {
            // Arrange
            var conversationId = "conversation-123";
            
            // First start typing
            await _chatHub.StartTyping(conversationId);
            
            // Act
            await _chatHub.StopTyping(conversationId);

            // Assert
            _mockClientProxy.Verify(c => c.SendAsync("UserStoppedTyping", 
                It.Is<TypingIndicatorDto>(dto => 
                    dto.UserId == "test-user-id" && 
                    dto.ConversationId == conversationId && 
                    dto.IsTyping == false),
                default), Times.Once);
        }

        [Fact]
        public async Task SetOnlineStatus_ShouldBroadcastStatusChange()
        {
            // Arrange
            var isOnline = true;

            // Act
            await _chatHub.SetOnlineStatus(isOnline);

            // Assert
            _mockClients.Verify(c => c.All, Times.Once);
            _mockClientProxy.Verify(c => c.SendAsync("UserStatusChanged", 
                It.Is<UserStatusDto>(dto => 
                    dto.UserId == "test-user-id" && 
                    dto.IsOnline == isOnline),
                default), Times.Once);
        }

        [Fact]
        public async Task GetUsersOnlineStatus_ShouldReturnStatusResponse()
        {
            // Arrange
            var userIds = new[] { "user1", "user2", "user3" };

            // Act
            await _chatHub.GetUsersOnlineStatus(userIds);

            // Assert
            _mockClients.Verify(c => c.Caller, Times.Once);
            _mockClientProxy.Verify(c => c.SendAsync("OnlineStatusResponse", 
                It.Is<OnlineStatusResponseDto>(dto => 
                    dto.UserStatuses.Count == userIds.Length),
                default), Times.Once);
        }

        [Fact]
        public async Task OnConnectedAsync_ShouldSetUserOnline()
        {
            // Act
            await _chatHub.OnConnectedAsync();

            // Assert
            _mockClients.Verify(c => c.All, Times.Once);
            _mockClientProxy.Verify(c => c.SendAsync("UserStatusChanged", 
                It.Is<UserStatusDto>(dto => 
                    dto.UserId == "test-user-id" && 
                    dto.IsOnline == true),
                default), Times.Once);
        }

        [Fact]
        public async Task OnDisconnectedAsync_ShouldSetUserOffline()
        {
            // Arrange - First connect to establish user connection
            await _chatHub.OnConnectedAsync();

            // Act
            await _chatHub.OnDisconnectedAsync(null);

            // Assert - Should have been called twice (connect + disconnect)
            _mockClients.Verify(c => c.All, Times.AtLeast(2));
            _mockClientProxy.Verify(c => c.SendAsync("UserStatusChanged", 
                It.Is<UserStatusDto>(dto => 
                    dto.UserId == "test-user-id" && 
                    dto.IsOnline == false),
                default), Times.Once);
        }

        [Fact]
        public async Task GetTypingUsers_ShouldReturnCurrentlyTypingUsers()
        {
            // Arrange
            var conversationId = "conversation-123";
            await _chatHub.StartTyping(conversationId);

            // Act
            await _chatHub.GetTypingUsers(conversationId);

            // Assert
            _mockClients.Verify(c => c.Caller, Times.Once);
            _mockClientProxy.Verify(c => c.SendAsync("TypingUsersResponse", 
                conversationId, 
                It.Is<string[]>(users => users.Contains("test-user-id")),
                It.IsAny<DateTime>(),
                default), Times.Once);
        }
    }
}
