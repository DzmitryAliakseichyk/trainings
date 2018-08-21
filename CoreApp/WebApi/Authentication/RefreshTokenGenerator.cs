using System;

namespace WebApi.Authentication
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}