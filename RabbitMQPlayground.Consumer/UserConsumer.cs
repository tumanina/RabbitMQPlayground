using MassTransit;
using RabbitMQPlayground.Contracts;
using System.Text.Json;

namespace RabbitMQPlayground.Consumer
{
    public class UserConsumer(IMessageHandler messageHandler) : IConsumer<User>
    {
        private readonly IMessageHandler _messageHandler = messageHandler;

        public Task Consume(ConsumeContext<User> context)
        {
            return _messageHandler.Handle(JsonSerializer.Serialize(context.Message));
        }
    }
}
