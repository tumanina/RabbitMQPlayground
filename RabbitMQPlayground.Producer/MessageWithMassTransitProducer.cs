using MassTransit;
using Microsoft.Extensions.Logging;
using RabbitMQPlayground.Contracts;

namespace RabbitMQPlayground.Producer;

public class MessageWithMassTransitProducer(ILogger<MessageWithMassTransitProducer> logger, IPublishEndpoint publishEndpoint) : IProducer
{
    private ILogger<MessageWithMassTransitProducer> _logger = logger;
    private IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Publish(User user)
    {
        try
        {
            await _publishEndpoint.Publish(user);

        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
