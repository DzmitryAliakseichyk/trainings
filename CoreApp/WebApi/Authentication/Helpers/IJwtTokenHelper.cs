using System;

namespace WebApi.Authentication.Helpers
{
    public interface IJwtTokenHelper
    {
        string GetSignature(string jwtToken);
        DateTimeOffset GetExpirationDate(string jwtToken);
    }
}