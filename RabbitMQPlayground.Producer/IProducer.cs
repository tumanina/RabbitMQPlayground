using System;

namespace RabbitMQPlayground.Producer;

public interface IProducer
{
    public Task SendMessage(string message);
}
