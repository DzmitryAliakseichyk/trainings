using Common.Models;
using Microsoft.Extensions.Logging;

namespace Data.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    { 
        public RefreshTokenRepository(
            IMongoWrapper mongoWrapper,
            ILogger<RefreshTokenRepository> logger) : base(mongoWrapper, logger)
        {
        }

        protected override string CollectionName => Collections.RefreshTokens;
    }
}