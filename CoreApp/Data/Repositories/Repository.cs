using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Data.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : BaseModel
    {
        protected readonly ILogger<Repository<T>> Logger;
        protected readonly IMongoDatabase Db;

        protected abstract string CollectionName { get; }

        protected IMongoCollection<T> MongoCollection => Db.GetCollection<T>(CollectionName);

        protected Repository(
            IMongoWrapper mongoWrapper,
            ILogger<Repository<T>> logger)
        {
            Logger = logger;
            Db = mongoWrapper.Database;
        }

        public virtual async Task<T> Create(T token)
        {
            try
            {
                await MongoCollection.InsertOneAsync(token);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }

            return token;
        }

        public virtual async Task<T> Get(Guid id)
        {
            var token = default(T);
            try
            {
                using (var cursor = await MongoCollection.FindAsync(x => x.Id == id))
                {
                    while (await cursor.MoveNextAsync())
                    {
                        var batch = cursor.Current;
                        foreach (var document in batch)
                        {
                            token = document;
                            break;
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }

            return token;
        }

        public virtual async Task<T> Update(T token)
        {
            try
            {
                var result = await MongoCollection.ReplaceOneAsync(
                    f => f.Id == token.Id,
                    token);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }

            return token;
        }
        
        public virtual async Task Delete(Guid id)
        {
            try
            {
                await MongoCollection.DeleteOneAsync(x => x.Id == id);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }
        }

        public virtual async Task Delete(Expression<Func<T, bool>> condition)
        {
            try
            {
                await MongoCollection.DeleteManyAsync<T>(condition);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }
        }
    }
}