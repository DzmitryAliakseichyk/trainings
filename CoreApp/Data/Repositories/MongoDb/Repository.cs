using System;
using System.Linq.Expressions;
using Common.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Data.Repositories.MongoDb
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

        public virtual T Create(T entity)
        {
            try
            {
                MongoCollection.InsertOne(entity);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }

            return entity;
        }

        public virtual T Get(Guid id)
        {
            return Get(x => x.Id == id);
        }

        public T Get(Expression<Func<T, bool>> filter = null, string includeProperties = default(string))
        {
            T token;
            try
            {
                token = MongoCollection.Find(filter).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }

            return token;
        }
        
        public virtual bool CheckIfExist(Expression<Func<T, bool>> filter)
        {
            try
            {
                return MongoCollection.Find(filter).CountDocuments() > 0;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }
        }

        public virtual T Update(T entityToUpdate)
        {
            try
            {
                MongoCollection.ReplaceOne(
                    f => f.Id == entityToUpdate.Id,
                    entityToUpdate);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }

            return entityToUpdate;
        }

        public virtual void Delete(Expression<Func<T, bool>> filter)
        {
            try
            {
                MongoCollection.DeleteMany<T>(filter);
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