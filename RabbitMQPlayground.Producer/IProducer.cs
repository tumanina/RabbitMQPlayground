namespace RabbitMQPlayground.Producer;

public interface IProducer
{
    public Task Publish(User user);
}
