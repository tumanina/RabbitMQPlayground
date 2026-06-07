using System;

namespace RabbitMQPlayground.Listener;

public interface IMessageHandler
{
    public Task Handle(string message);
}
