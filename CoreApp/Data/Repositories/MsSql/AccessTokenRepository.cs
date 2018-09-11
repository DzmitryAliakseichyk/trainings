using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories.MsSql
{
    public class AccessTokenRepository : Repository<AccessToken>, IAccessTokenRepository
    { 
        public AccessTokenRepository(
            DbContext context,
            ILogger<AccessTokenRepository> logger) : base(context, logger)
        {
        }
    }
}