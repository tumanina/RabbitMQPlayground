using MassTransit;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RabbitMQPlayground.Configuration;
using RabbitMQPlayground.Producer;

var builder = WebApplication.CreateBuilder(args);

var configBuilder = new ConfigurationBuilder();

configBuilder.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = configBuilder.AddEnvironmentVariables()
    .Build();

RegisterTelemetry(builder, configuration);

builder.Services.Configure<RabbitMQConfiguration>(configuration.GetSection(nameof(RabbitMQConfiguration)));
var rabbitMqConfiguration = configuration.GetSection(nameof(RabbitMQConfiguration)).Get<RabbitMQConfiguration>();
if (rabbitMqConfiguration.UseMassTransit)
{
    builder.Services.AddMassTransit(x =>
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(rabbitMqConfiguration.HostName);
        });
    });
    builder.Services.AddScoped<IProducer, MessageWithMassTransitProducer>();
}
else
{
    builder.Services.AddSingleton<IProducer, MessageProducer>();
}

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var endpointConventionBuilder = app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocumentTitle = "API Explorer";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void RegisterTelemetry(WebApplicationBuilder builder, IConfigurationRoot configuration)
{
    builder.Logging.ClearProviders();

    var telemetryConfiguration = configuration.GetSection(nameof(TelemetryConfiguration)).Get<TelemetryConfiguration>();
    builder.Services.AddOpenTelemetry()
        .WithTracing(tracing => tracing
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
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
}