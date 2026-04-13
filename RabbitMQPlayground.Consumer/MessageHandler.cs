using Microsoft.Extensions.Logging;

namespace RabbitMQPlayground.Consumer
{
    public class MessageHandler(ILogger<MessageHandler> logger) : IMessageHandler
    {
        private readonly ILogger _logger = logger;

        public async Task Handle(string message)
        {
            await Task.Delay(10);
            _logger.LogInformation("Processing message: " + message);
        }
    }
}
