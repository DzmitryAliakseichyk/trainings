﻿using System;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
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

        public virtual async Task Delete(Func<T, bool> condition)
        {
            try
            {
                await MongoCollection.DeleteManyAsync(x => condition.Invoke(x));
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
                throw;
            }
        }
    }
}