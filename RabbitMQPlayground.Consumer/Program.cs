using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQPlayground.Configuration;
using RabbitMQPlayground.Consumer;
using System.Diagnostics;
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

string activitySourceName = RegisterTelemetry(builder, telemetryConfiguration);

builder.Services.Configure<MongodbConfiguration>(configuration.GetSection(nameof(MongodbConfiguration)));
builder.Services.Configure<RabbitMQConfiguration>(configuration.GetSection(nameof(RabbitMQConfiguration)));
builder.Services.AddSingleton<IMessagesContext, MessagesContext>();
builder.Services.AddScoped<IMessageHandler, MessageHandler>();

var host = builder.Build();

var rabbitMQConfiguration = configuration.GetSection(nameof(RabbitMQConfiguration)).Get<RabbitMQConfiguration>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();


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
else
{
    var factory = new ConnectionFactory
    {
        HostName = rabbitMQConfiguration?.HostName ?? "localhost",
        Port = rabbitMQConfiguration?.Port ?? 5672
    };

    var handler = host.Services.GetRequiredService<IMessageHandler>();
    using var connection = await factory.CreateConnectionAsync();
    using var source = new ActivitySource(activitySourceName);
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
                using (Activity? activity = source.StartActivity("ConsumeMessage"))
                {
                    activity?.AddEvent(new ActivityEvent("Handled message " + message));
                }
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                using (Activity? activity = source.StartActivity("ConsumeMessage"))
                {
                    activity?.AddEvent(new ActivityEvent("Exception " + ex.Message));
                }
                logger.LogError(ex.Message);
            }
        };
        await channel.BasicConsumeAsync("test", false, consumer);
    }
    catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex)
    {
        using (Activity? activity = source.StartActivity("RabbitMQException"))
        {
            activity?.AddEvent(new ActivityEvent(ex.Message));
        }
    }
}

host.Run();

static string RegisterTelemetry(HostApplicationBuilder builder, TelemetryConfiguration? telemetryConfiguration)
{
    var resource = builder.Environment.ApplicationName;
    const string activitySourceName = "ConsumerLogs";
    builder.Services.AddOpenTelemetry()
         .ConfigureResource(res => res
            .AddService(resource))
        .WithTracing(tracing => tracing
            .AddSource(activitySourceName)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(opts => { opts.Endpoint = new Uri(telemetryConfiguration.Endpoint); }));

    builder.Logging.AddOpenTelemetry(logging =>
    {
        logging.IncludeFormattedMessage = true;
        logging.IncludeScopes = true;
        logging.AttachLogsToActivityEvent();
        logging.AddOtlpExporter(opts => { opts.Endpoint = new Uri(telemetryConfiguration.Endpoint); });
    });
    return activitySourceName;
}