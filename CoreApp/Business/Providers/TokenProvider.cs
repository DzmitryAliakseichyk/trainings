using System;
using System.Threading.Tasks;
using Common.Models;
using Data.Repositories;

namespace Business.Providers
{
    public class TokenProvider : ITokenProvider
    {
        private const int RefreshTokenLifeTime = 2;
        private const int AccessTokenLifeTime = 15;

        private readonly IRefreshTokenRepository _refreshTokenRepository;

        private readonly IAccessTokenRepository _accessTokenRepository;

        public TokenProvider(IRefreshTokenRepository refreshTokenRepository, 
            IAccessTokenRepository accessTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _accessTokenRepository = accessTokenRepository;
        }

        public async Task Save(string refreshToken, string accessTokenSignature, string userName)
        {
            await _refreshTokenRepository.Upsert(new Token
            {
                Id = Guid.Parse(refreshToken),
                Username = userName,
                ExpirationDate = DateTimeOffset.Now.AddDays(RefreshTokenLifeTime)
            });

            await _accessTokenRepository.Upsert(new Token
            {
                Id = accessTokenSignature,
                Username = userName,
                ExpirationDate = DateTimeOffset.Now.AddMinutes(AccessTokenLifeTime)
            });
        }

        public async Task Delete(string refreshToken, string accessToken)
        {
            await _refreshTokenRepository.Delete(Guid.Parse(refreshToken));
            await _accessTokenRepository.Delete(accessToken);
        }

        public async Task Revoke(string userName)
        {
            await _refreshTokenRepository.Delete(x => x.Username.Equals(userName, StringComparison.OrdinalIgnoreCase));
            await _accessTokenRepository.Delete(x => x.Username.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }
    }
}