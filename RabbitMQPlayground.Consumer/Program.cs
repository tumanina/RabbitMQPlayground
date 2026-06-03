using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQPlayground.Configuration;
using RabbitMQPlayground.Consumer;
using System.Text;

var configBuilder = new ConfigurationBuilder();

configBuilder.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = configBuilder.AddEnvironmentVariables()
    .Build();

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var telemetryConfiguration = configuration.GetSection(nameof(TelemetryConfiguration)).Get<TelemetryConfiguration>();

builder.Services.Configure<MongodbConfiguration>(configuration.GetSection(nameof(MongodbConfiguration)));
builder.Services.Configure<RabbitMQConfiguration>(configuration.GetSection(nameof(RabbitMQConfiguration)));
builder.Services.AddSingleton<IMessagesContext, MessagesContext>();
builder.Services.AddScoped<IMessageHandler, MessageHandler>();

var rabbitMQConfiguration = configuration.GetSection(nameof(RabbitMQConfiguration)).Get<RabbitMQConfiguration>();
if (rabbitMQConfiguration?.UseMassTransit ?? false)
{
    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<UserConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(rabbitMQConfiguration?.HostName ?? "localhost");

            cfg.ConfigureEndpoints(context);
        });
    });

}

var host = builder.Build();

if (!rabbitMQConfiguration.UseMassTransit)
{
    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    var factory = new ConnectionFactory
    {
        HostName = rabbitMQConfiguration?.HostName ?? "localhost",
        Port = rabbitMQConfiguration?.Port ?? 5672
    };

    var handler = host.Services.GetRequiredService<IMessageHandler>();
    using var connection = await factory.CreateConnectionAsync();
    using var channel = await connection.CreateChannelAsync();
    try
    {
        await channel.QueueDeclarePassiveAsync(queue: "test");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (ch, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                await handler.Handle(message);
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        };
        await channel.BasicConsumeAsync("test", false, consumer);
    }
    catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex)
    {
        logger.LogError(ex.Message);
    }
}

host.Run();