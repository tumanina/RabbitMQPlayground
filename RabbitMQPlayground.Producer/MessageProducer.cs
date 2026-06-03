using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQPlayground.Configuration;
using RabbitMQPlayground.Contracts;
using System.Text.Json;

namespace RabbitMQPlayground.Producer;

public class MessageProducer(IOptions<RabbitMQConfiguration> configuration) : IProducer, IDisposable
{
    private readonly RabbitMQConfiguration _configuration = configuration.Value;
    private IConnection? _connection;

    public async Task Publish(User user)
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
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "users", body: JsonSerializer.SerializeToUtf8Bytes(user));
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
