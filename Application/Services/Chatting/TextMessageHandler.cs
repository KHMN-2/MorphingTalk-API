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
    public class TextMessageHandler : IMessageHandler
    {
        ILogger<TextMessageHandler> _logger;

        public TextMessageHandler(ILogger<TextMessageHandler> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(MessageType messageType) => messageType == MessageType.Text;

        public async Task HandleMessageAsync(IMessage message)
        {
            var textMessage = message as TextMessage;
            if (textMessage == null) throw new InvalidOperationException("Invalid message type");

            textMessage.Content += "dsa";

            // Add text-specific processing logic here
            _logger.LogInformation($"Processing text message: {textMessage.Content}");
            await Task.CompletedTask;
        }
    }
}
