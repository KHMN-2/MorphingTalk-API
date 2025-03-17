using Microsoft.AspNetCore.SignalR;
using MorphingTalk.API.Interfaces.Services;
using MorphingTalk.Core.Domain.Entities;

namespace MorphingTalk.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IMessageService messageService, ILogger<ChatHub> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        public async Task SendTextMessage(string content)
        {
            var message = new TextMessage
            {
                Id = Guid.NewGuid(),
                SenderId = Context.ConnectionId,
                Timestamp = DateTime.UtcNow,
                Content = content
            };

            await _messageService.ProcessMessageAsync(message);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendVoiceMessage(string voiceUrl, int duration)
        {
            var message = new VoiceMessage
            {
                Id = Guid.NewGuid(),
                SenderId = Context.ConnectionId,
                Timestamp = DateTime.UtcNow,
                VoiceUrl = voiceUrl,
                Duration = duration
            };

            await _messageService.ProcessMessageAsync(message);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
