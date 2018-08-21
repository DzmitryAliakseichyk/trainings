using System;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Data.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ILogger<RefreshTokenRepository> _logger;
        private const int RefreshTokenLifeTime = 2;
        private readonly IMongoDatabase _db;

        private IMongoCollection<RefreshToken> MongoCollection => _db.GetCollection<RefreshToken>(Collections.RefreshTokens);

        public RefreshTokenRepository(
            IMongoWrapper mongoWrapper,
            ILogger<RefreshTokenRepository> logger)
        {
            _logger = logger;
            _db = mongoWrapper.Database;
        }

        public async Task<RefreshToken> Create(string refreshToken, string username)
        {
            var refreshTokenObject = new RefreshToken
            {
                Id = Guid.Parse(refreshToken),
                Username = username,
                ExpirationDate = DateTimeOffset.Now.AddDays(RefreshTokenLifeTime)
            };
            
            try
            {
                await MongoCollection.InsertOneAsync(refreshTokenObject);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }

            return refreshTokenObject;
        }

        public async Task Remove(string refreshToken)
        {
            try
            {
                await MongoCollection.DeleteOneAsync(x => x.Id == Guid.Parse(refreshToken));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }
        }
    }
}