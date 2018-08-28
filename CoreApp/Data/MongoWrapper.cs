using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Data
{
    public class MongoWrapper : IMongoWrapper
    {
        public MongoWrapper(IOptions<MongoConnectionModel> settings)
        {
            Client = new MongoClient(settings.Value.ConnectionString);
            Database = Client.GetDatabase(settings.Value.Database);
        }

        public IMongoClient Client { get; }

        public IMongoDatabase Database { get; }

    }
}