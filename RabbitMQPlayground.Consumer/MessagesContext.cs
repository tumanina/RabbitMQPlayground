using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQPlayground.Configuration;

namespace RabbitMQPlayground.Consumer
{
    public class MessagesContext: IMessagesContext
    {
        public MessagesContext(IOptions<MongodbConfiguration> configuration)
        {
            var client = new MongoClient(configuration.Value.ConnectionString);
            var database = client.GetDatabase(configuration.Value.DatabaseName);

            Messages = database.GetCollection<BsonDocument>(configuration.Value.CollectionName);
        }

        public IMongoCollection<BsonDocument> Messages { get; }
    }

    public interface IMessagesContext
    {
        IMongoCollection<BsonDocument> Messages { get; }
    }
}
