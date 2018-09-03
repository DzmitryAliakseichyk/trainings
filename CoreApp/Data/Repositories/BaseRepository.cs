using MongoDB.Driver;

namespace Data.Repositories
{
    public abstract class BaseRepository
    {
        protected static IMongoDatabase Db;
    }
}