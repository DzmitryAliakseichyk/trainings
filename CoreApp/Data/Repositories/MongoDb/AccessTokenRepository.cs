using Common.Models;
using Microsoft.Extensions.Logging;

namespace Data.Repositories.MongoDb
{
    public class AccessTokenRepository : Repository<AccessToken>, IAccessTokenRepository
    {
        public AccessTokenRepository(
            IMongoWrapper mongoWrapper,
            ILogger<AccessTokenRepository> logger) : base(mongoWrapper, logger)
        {
        }

        protected override string CollectionName => Collections.AccessTokens;
    }
}