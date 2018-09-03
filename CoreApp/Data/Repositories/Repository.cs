using System;
using System.Linq.Expressions;
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

        public virtual T Create(T token)
        {
            try
            {
                MongoCollection.InsertOne(token);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }

            return token;
        }

        public virtual T Get(Guid id)
        {
            return Get(x => x.Id == id);
        }

        public virtual T Get(Expression<Func<T, bool>> condition)
        {
            T token;
            try
            {
                token = MongoCollection.Find(condition).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }

            return token;
        }


        public virtual bool CheckIfExist(Expression<Func<T, bool>> condition)
        {
            try
            {
                return MongoCollection.Find(condition).CountDocuments() > 0;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }
        }

        public virtual T Update(T token)
        {
            try
            {
                MongoCollection.ReplaceOne(
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

        public virtual void Delete(Expression<Func<T, bool>> condition)
        {
            try
            {
                MongoCollection.DeleteMany<T>(condition);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }
        }

        public virtual void Delete(Guid id)
        {
            Delete(x => x.Id == id);
        }
    }
}