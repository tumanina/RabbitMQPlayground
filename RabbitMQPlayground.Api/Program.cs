using RabbitMQPlayground.Configuration;
using RabbitMQPlayground.Producer;

var builder = WebApplication.CreateBuilder(args);

var configBuilder = new ConfigurationBuilder();

configBuilder.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = configBuilder.AddEnvironmentVariables()
    .Build();

builder.Services.Configure<RabbitMQConfiguration>(configuration.GetSection(nameof(RabbitMQConfiguration)));
builder.Services.AddSingleton<IProducer, MessageProducer>();
builder.Services.AddScoped<MessageProducer>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

IEndpointConventionBuilder endpointConventionBuilder = app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocumentTitle = "My API Explorer";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
