using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Microsoft.AspNetCore.SignalR;

namespace MorphingTalk.API.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinChat(string chatId, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            await Clients.Group(chatId).SendAsync("UserJoined", userId);
        }

        public async Task LeaveChat(string chatId, string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
            await Clients.Group(chatId).SendAsync("UserLeft", userId);
        }
        //private readonly IMessageService _messageService;
        //private readonly ILogger<ChatHub> _logger;

        //public ChatHub(IMessageService messageService, ILogger<ChatHub> logger)
        //{
        //    _messageService = messageService;
        //    _logger = logger;
        //}

        //public async Task SendTextMessage(string content)
        //{
        //    var message = new TextMessage
        //    {
        //        Id = Guid.NewGuid(),
        //        SenderId = Context.ConnectionId,
        //        Timestamp = DateTime.UtcNow,
        //        Content = content
        //    };

        //    await _messageService.ProcessMessageAsync(message);
        //    await Clients.All.SendAsync("ReceiveMessage", message);
        //}

        //public async Task SendVoiceMessage(string voiceUrl, int duration)
        //{
        //    var message = new VoiceMessage
        //    {
        //        Id = Guid.NewGuid(),
        //        SenderId = Context.ConnectionId,
        //        Timestamp = DateTime.UtcNow,
        //        VoiceUrl = voiceUrl,
        //        Duration = duration
        //    };

        //    await _messageService.ProcessMessageAsync(message);
        //    await Clients.All.SendAsync("ReceiveMessage", message);
        //}

        //public override async Task OnConnectedAsync()
        //{
        //    _logger.LogInformation($"Client connected: {Context.ConnectionId}");
        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
        //    await base.OnDisconnectedAsync(exception);
        //}
    }
}
