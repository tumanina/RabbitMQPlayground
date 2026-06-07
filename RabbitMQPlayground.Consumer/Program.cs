using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQPlayground.Configuration;
using RabbitMQPlayground.Consumer.Consumers;
using RabbitMQPlayground.Consumer.Database;
using RabbitMQPlayground.Consumer.Handlers;

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
        x.AddConsumer<ConsumerWithMassTransit>();

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(rabbitMQConfiguration?.HostName ?? "localhost");
            cfg.ConfigureEndpoints(context);
        });
    });
}
else
{
    builder.Services.AddHostedService<UsersQueueConsumer>();
}

var host = builder.Build();

await host.RunAsync();