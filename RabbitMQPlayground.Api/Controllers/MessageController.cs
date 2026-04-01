using Microsoft.AspNetCore.Mvc;
using RabbitMQPlayground.Producer;

namespace RabbitMQPlayground.Api.Controllers;

[ApiController]
[Route("messages")]
public class MessageController : ControllerBase
{
    private readonly IProducer _producer;

    public MessageController(IProducer producer)
    {
        _producer = producer;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] string message)
    {
        await _producer.SendMessage("test", message);

        return Ok();
    }
}
