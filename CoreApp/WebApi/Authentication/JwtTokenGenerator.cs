using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Models;
using WebApi.Models.Settings;

namespace WebApi.Authentication
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettingsModel _jwtSettings;

        public JwtTokenGenerator(IOptions<JwtSettingsModel> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }
       
        public string Generate(AppUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Sid, user.Id),

                new Claim(ClaimTypes.Name, user.UserName)
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                signingCredentials: credentials,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpitarionsOffsetInMinutes)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
