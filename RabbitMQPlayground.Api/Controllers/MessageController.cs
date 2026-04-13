using Microsoft.AspNetCore.Mvc;
using RabbitMQPlayground.Producer;

namespace RabbitMQPlayground.Api.Controllers;

[ApiController]
[Route("messages")]
public class MessageController(IProducer producer, ILogger<MessageController> logger) : ControllerBase
{
    private readonly IProducer _producer = producer;
    private readonly ILogger<MessageController> _logger = logger;

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] string message)
    {
        await _producer.SendMessage("test", message);

        _logger.LogInformation($"Sent message {message} at {DateTime.Now}");

        return Ok();
    }
}
