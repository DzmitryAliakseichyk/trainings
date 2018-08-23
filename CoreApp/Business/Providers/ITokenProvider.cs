using System;
using System.Threading.Tasks;
using Common.Models;

namespace Business.Providers
{
    public interface ITokenProvider
    {
        Task CreateRefreshToken(string refreshToken, Guid userId);
        Task CreateAccessToken(string signature, DateTimeOffset expirationDate, Guid userId);
        Task<RefreshToken> GetRefreshToken(Guid refreshToken);
        Task UpdateRefreshToken(Guid refreshToken);
        Task DeleteRefreshTokens(Guid refreshToken);
        Task DeleteAccessToken(string accessToken);
        Task DeleteRefreshTokensByUserId(Guid userId);
        Task DeleteAccessTokenByUserId(Guid userId);
    }
}