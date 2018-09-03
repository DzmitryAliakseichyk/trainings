using System;
using System.Linq.Expressions;
using Common.Models;

namespace Business.Providers
{
    public interface ITokenProvider
    {
        void RegisterRefreshToken(string refreshToken, Guid userId);
        void RegisterAccessToken(string signature, DateTimeOffset expirationDate, Guid userId);
        RefreshToken GetRefreshToken(Guid refreshToken);
        void UpdateRefreshToken(Guid refreshToken);
        void DeleteRefreshTokenById(Guid refreshToken);
        void DeleteAccessToken(string accessToken);
        void DeleteRefreshTokensByUserId(Guid userId);
        void DeleteAccessTokenByUserId(Guid userId);
        bool IsAccessTokenRegistered(string accessToken);
        void DeleteRefreshToken(Expression<Func<RefreshToken, bool>> condition);
        void DeleteAccessToken(Expression<Func<AccessToken, bool>> condition);
    }
}