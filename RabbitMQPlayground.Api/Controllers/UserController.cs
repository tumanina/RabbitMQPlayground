using Microsoft.AspNetCore.Mvc;
using RabbitMQPlayground.Contracts;
using RabbitMQPlayground.Producer;

namespace RabbitMQPlayground.Api.Controllers;

[ApiController]
[Route("users")]
public class UserController(IProducer producer, ILogger<UserController> logger) : ControllerBase
{
    private readonly IProducer _producer = producer;
    private readonly ILogger<UserController> _logger = logger;

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] User user)
    {
        await _producer.Publish(user);
        _logger.LogInformation("Sent message");
        return Ok();
    }
}
