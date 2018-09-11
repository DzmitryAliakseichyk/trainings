using MongoDB.Driver;

namespace Data.Repositories.MongoDb
{
    public abstract class BaseRepository
    {
        protected static IMongoDatabase Db;
    }
}