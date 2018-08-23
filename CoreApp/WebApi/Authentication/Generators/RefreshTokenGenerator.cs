using System;

namespace WebApi.Authentication.Generators
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}