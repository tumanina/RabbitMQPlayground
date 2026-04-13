using System;

namespace RabbitMQPlayground.Consumer;

public interface IMessageHandler
{
    public Task Handle(string message);
}
