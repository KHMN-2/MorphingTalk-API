using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Microsoft.Extensions.Logging;

namespace Application.Services.Chatting
{
    //public class VoiceMessageHandler : IMessageHandler
    //{
    //    private readonly ILogger<VoiceMessageHandler> _logger;

    //    public VoiceMessageHandler(ILogger<VoiceMessageHandler> logger)
    //    {
    //        _logger = logger;
    //    }

    //    public bool CanHandle(MessageType messageType) => messageType == MessageType.Voice;

    //    public async Task HandleMessageAsync(Message message)
    //    {
    //        var voiceMessage = message as VoiceMessage;
    //        if (voiceMessage == null) throw new InvalidOperationException("Invalid message type");

    //        // Add voice-specific processing logic here
    //        _logger.LogInformation($"Processing voice message: {voiceMessage.VoiceUrl}");
    //        await Task.CompletedTask;
    //    }
    //}
}
