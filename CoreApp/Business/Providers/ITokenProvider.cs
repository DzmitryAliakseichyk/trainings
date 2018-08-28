using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Models;

namespace Business.Providers
{
    public interface ITokenProvider
    {
        Task RegisterRefreshToken(string refreshToken, Guid userId);
        Task RegisterAccessToken(string signature, DateTimeOffset expirationDate, Guid userId);
        Task<RefreshToken> GetRefreshToken(Guid refreshToken);
        Task UpdateRefreshToken(Guid refreshToken);
        Task DeleteRefreshTokenById(Guid refreshToken);
        Task DeleteAccessToken(string accessToken);
        Task DeleteRefreshTokensByUserId(Guid userId);
        Task DeleteAccessTokenByUserId(Guid userId);
        Task<bool> IsAccessTokenRegistered(string accessToken);
        Task DeleteRefreshToken(Expression<Func<RefreshToken, bool>> condition);
        Task DeleteAccessToken(Expression<Func<AccessToken, bool>> condition);
    }
}