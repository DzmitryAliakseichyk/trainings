using MongoDB.Driver;

namespace Data
{
    public interface IMongoWrapper
    {
        IMongoClient Client { get; }
        IMongoDatabase Database { get; }
    }
}