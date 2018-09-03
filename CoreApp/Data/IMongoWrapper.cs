using MongoDB.Driver;

namespace Data
{
    public interface IMongoWrapper
    {
        IMongoDatabase Database { get; }
    }
}