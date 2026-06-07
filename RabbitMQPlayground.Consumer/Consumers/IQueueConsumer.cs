using System;

namespace RabbitMQPlayground.Consumer.Consumers;

public interface IQueueConsumer
{
    public Task Run(string queueName);
}
