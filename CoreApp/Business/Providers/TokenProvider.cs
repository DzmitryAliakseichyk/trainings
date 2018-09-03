using System;
using System.Linq.Expressions;
using Common.Models;
using Data.Repositories;

namespace Business.Providers
{
    public class TokenProvider : ITokenProvider
    {
        private const int RefreshTokenLifeTime = 2;

        private readonly IRefreshTokenRepository _refreshTokenRepository;

        private readonly IAccessTokenRepository _accessTokenRepository;

        public TokenProvider(IRefreshTokenRepository refreshTokenRepository, 
            IAccessTokenRepository accessTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _accessTokenRepository = accessTokenRepository;
        }

        public void RegisterRefreshToken(string refreshToken, Guid userId)
        {
            _refreshTokenRepository.Create(new RefreshToken
            {
                Id = Guid.Parse(refreshToken),
                UserId = userId,
                ExpirationDate = DateTimeOffset.Now.AddDays(RefreshTokenLifeTime)
            });
        }

        public void RegisterAccessToken(string signature, DateTimeOffset expirationDate, Guid userId)
        {
            _accessTokenRepository.Create(new AccessToken
            {
                TokenSignature = signature,
                UserId = userId,
                ExpirationDate = expirationDate
            });
        }
        
        public RefreshToken GetRefreshToken(Guid refreshToken)
        {
            var token = _refreshTokenRepository.Get(refreshToken);
            return token;
        }

        public bool IsAccessTokenRegistered(string accessToken)
        {
            return _accessTokenRepository.CheckIfExist(x => x.TokenSignature.Equals(accessToken));
        }

        public void UpdateRefreshToken(Guid refreshToken)
        {
            var token = _refreshTokenRepository.Get(refreshToken);

            _refreshTokenRepository.Update(new RefreshToken
            {
                Id = token.Id,
                UserId = token.UserId,
                ExpirationDate = DateTimeOffset.Now.AddDays(RefreshTokenLifeTime)
            });
        }

        #region Delete

        public void DeleteRefreshToken(Expression<Func<RefreshToken, bool>> condition)
        {
            _refreshTokenRepository.Delete(condition);
        }

        public void DeleteRefreshTokenById(Guid refreshToken)
        {
            _refreshTokenRepository.Delete(refreshToken);
        }

        public void DeleteRefreshTokensByUserId(Guid userId)
        {
            _refreshTokenRepository.Delete(x => x.UserId == userId);
        }

        public void DeleteAccessToken(Expression<Func<AccessToken, bool>> condition)
        {
            _accessTokenRepository.Delete(condition);
        }

        public void DeleteAccessToken(string accessToken)
        {
        }

        public void DeleteAccessTokenByUserId(Guid userId)
        {
            _accessTokenRepository.Delete(x => x.UserId == userId);
        }

        #endregion
    }
}