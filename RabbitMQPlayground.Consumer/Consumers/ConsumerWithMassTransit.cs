using MassTransit;
using RabbitMQPlayground.Consumer.Handlers;
using RabbitMQPlayground.Contracts;
using System.Text.Json;

namespace RabbitMQPlayground.Consumer.Consumers
{
    public class ConsumerWithMassTransit(IMessageHandler messageHandler) : IConsumer<User>
    {
        private readonly IMessageHandler _messageHandler = messageHandler;

        public Task Consume(ConsumeContext<User> context)
        {
            return _messageHandler.Handle(JsonSerializer.Serialize(context.Message));
        }
    }
}
