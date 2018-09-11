using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories.MsSql
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    { 
        public RefreshTokenRepository(
            DbContext context,
            ILogger<RefreshTokenRepository> logger) : base(context, logger)
        {
        }
    }
}