using System;
using System.IdentityModel.Tokens.Jwt;

namespace WebApi.Authentication.Helpers
{
    public class JwtTokenHelper : IJwtTokenHelper
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public JwtTokenHelper()
        {
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public string GetSignature(string jwtToken)
        {
            return _jwtSecurityTokenHandler.ReadJwtToken(jwtToken).RawSignature;
        }

        public DateTimeOffset GetExpirationDate(string jwtToken)
        {
            var expirationDate = _jwtSecurityTokenHandler.ReadJwtToken(jwtToken).ValidTo;
            return (DateTimeOffset) expirationDate;
        }
    }
}