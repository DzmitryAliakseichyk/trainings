using Common.Models;
using Microsoft.Extensions.Logging;

namespace Data.Repositories
{
    public class AccessTokenRepository : Repository<Token>, IAccessTokenRepository
    {
        public AccessTokenRepository(
            IMongoWrapper mongoWrapper,
            ILogger<AccessTokenRepository> logger) : base(mongoWrapper, logger)
        {
        }

        protected override string CollectionName => Collections.AccessTokens;
    }
}