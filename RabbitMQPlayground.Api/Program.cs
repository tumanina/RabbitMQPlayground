using RabbitMQPlayground.Producer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IProducer, MessageProducer>();

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
