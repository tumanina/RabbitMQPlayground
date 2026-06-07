using System;

namespace RabbitMQPlayground.Consumer.Handlers;

public interface IMessageHandler
{
    public Task Handle(string message);
}
