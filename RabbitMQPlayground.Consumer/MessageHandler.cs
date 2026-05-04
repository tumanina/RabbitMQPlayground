using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace RabbitMQPlayground.Consumer
{
    public class MessageHandler(ILogger<MessageHandler> logger, IMessagesContext database) : IMessageHandler
    {
        private readonly ILogger _logger = logger;
        private readonly IMessagesContext _database = database;

        public async Task Handle(string message)
        {
            _logger.LogInformation("Processing message: " + message);

            var document = new BsonDocument { { "Time", DateTime.Now }, { "Content", message } };
            await _database.Messages.InsertOneAsync(document);
        }
    }
}
