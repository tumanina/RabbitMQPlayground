using DnsClient.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQPlayground.Configuration;
using RabbitMQPlayground.Consumer.Handlers;
using System.Text;

namespace RabbitMQPlayground.Consumer.Consumers
{
    public class UsersQueueConsumer(ILogger<UsersQueueConsumer> logger, IMessageHandler handler, IOptions<RabbitMQConfiguration> configuration) : BackgroundService
    {
        private ILogger<UsersQueueConsumer> _logger = logger;
        private readonly RabbitMQConfiguration _configuration = configuration.Value;
        private IMessageHandler _handler = handler;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration.HostName,
                Port = _configuration.Port
            };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            try
            {
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (ch, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    await _handler.Handle(message);
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                await channel.BasicConsumeAsync("users", false, consumer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
