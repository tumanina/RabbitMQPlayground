using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQPlayground.Configuration;
using System.Text;

namespace RabbitMQPlayground.Producer;

public class MessageProducer: IProducer, IDisposable
{
    private readonly RabbitMQConfiguration _configuration;
    private IConnection? _connection;

    public MessageProducer(IOptions<RabbitMQConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    public async Task SendMessage(string queueName, string message)
    {
        if (_connection == null)
        {
            var factory = new ConnectionFactory 
            { 
                HostName = _configuration.HostName,
                Port = _configuration.Port
            };
            _connection = await factory.CreateConnectionAsync();
        }

        using var channel = await _connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" } });
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: Encoding.UTF8.GetBytes(message));
    }

    public void Dispose()
    {
        if (_connection != null)
        {
            _connection.CloseAsync().GetAwaiter().GetResult();
            _connection.Dispose();
        }
    }
}
