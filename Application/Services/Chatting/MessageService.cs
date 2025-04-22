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
    public class MessageService : IMessageService
    {
        private readonly IEnumerable<IMessageHandler> _handlers;
        private readonly ILogger<MessageService> _logger;

        public MessageService(IEnumerable<IMessageHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task ProcessMessageAsync(Message message)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(message.Type));

            if (handler == null)
            {
                _logger.LogError($"No handler found for message type: {message.Type}");
                throw new NotSupportedException($"Message type {message.Type} is not supported");
            }

            await handler.HandleMessageAsync(message);
        }
    }
}
