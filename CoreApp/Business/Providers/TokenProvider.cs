using System;
using System.Linq.Expressions;
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

        public async Task RegisterRefreshToken(string refreshToken, Guid userId)
        {
            await _refreshTokenRepository.Create(new RefreshToken
            {
                Id = Guid.Parse(refreshToken),
                UserId = userId,
                ExpirationDate = DateTimeOffset.Now.AddDays(RefreshTokenLifeTime)
            });
        }

        public async Task RegisterAccessToken(string signature, DateTimeOffset expirationDate, Guid userId)
        {
            await _accessTokenRepository.Create(new AccessToken
            {
                TokenSignature = signature,
                UserId = userId,
                ExpirationDate = expirationDate
            });
        }
        
        public async Task<RefreshToken> GetRefreshToken(Guid refreshToken)
        {
            var token = await _refreshTokenRepository.Get(refreshToken);
            return token;
        }

        public Task<bool> IsAccessTokenRegistered(string accessToken)
        {
            return _accessTokenRepository.CheckIfExist(x => x.TokenSignature.Equals(accessToken));
        }

        public async Task UpdateRefreshToken(Guid refreshToken)
        {
            var token = await _refreshTokenRepository.Get(refreshToken);

            await _refreshTokenRepository.Update(new RefreshToken
            {
                Id = token.Id,
                UserId = token.UserId,
                ExpirationDate = DateTimeOffset.Now.AddDays(RefreshTokenLifeTime)
            });
        }

        #region Delete

        public async Task DeleteRefreshToken(Expression<Func<RefreshToken, bool>> condition)
        {
            await _refreshTokenRepository.Delete(condition);
        }

        public async Task DeleteRefreshTokenById(Guid refreshToken)
        {
            await _refreshTokenRepository.Delete(refreshToken);
        }

        public async Task DeleteRefreshTokensByUserId(Guid userId)
        {
            await _refreshTokenRepository.Delete(x => x.UserId == userId);
        }

        public async Task DeleteAccessToken(Expression<Func<AccessToken, bool>> condition)
        {
            await _accessTokenRepository.Delete(condition);
        }

        public async Task DeleteAccessToken(string accessToken)
        {
            await _accessTokenRepository.Delete(x => x.TokenSignature.Equals(accessToken));
        }

        public async Task DeleteAccessTokenByUserId(Guid userId)
        {
            await _accessTokenRepository.Delete(x => x.UserId == userId);
        }

        #endregion
    }
}