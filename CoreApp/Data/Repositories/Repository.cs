using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Data.Repositories
{
    public abstract class Repository<T> : BaseRepository, IRepository<T> where T : BaseModel
    {
        protected abstract string CollectionName { get; }

        protected readonly ILogger<Repository<T>> Logger;

        private readonly object _collectionLock = new object();

        private IMongoCollection<T> _mongoCollection;

        protected IMongoCollection<T> MongoCollection
        {
            get
            {
                if (_mongoCollection == null)
                {
                    lock (_collectionLock)
                    {
                        if (_mongoCollection == null)
                        {
                            _mongoCollection = Db.GetCollection<T>(CollectionName);
                        }
                    }
                }

                return _mongoCollection;
            }
        }

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

        public virtual async Task<T> Get(Expression<Func<T, bool>> condition)
        {
            var token = default(T);
            try
            {
                using (var cursor = await MongoCollection.FindAsync(condition))
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

        public virtual async Task<T> Get(Guid id)
        {
            return await Get(x => x.Id == id);
        }

        public virtual async Task<bool> CheckIfExist(Expression<Func<T, bool>> condition)
        {
            try
            {
                return await MongoCollection.Find(condition).CountDocumentsAsync() > 0;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }
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

        public virtual async Task Delete(Guid id)
        {
            await Delete(x => x.Id == id);
        }
    }
}