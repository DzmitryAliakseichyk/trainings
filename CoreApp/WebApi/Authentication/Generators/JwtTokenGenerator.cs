using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Authentication.Models;
using WebApi.Models.Settings;

namespace WebApi.Authentication.Generators
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettingsModel _jwtSettings;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public JwtTokenGenerator(IOptions<JwtSettingsModel> jwtSettings, RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _jwtSettings = jwtSettings.Value;
            _roleManager = roleManager;
            _userManager = userManager;
        }
       
        public string Generate(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var userRoles = _userManager.GetRolesAsync(user).Result;
            
            var roles = _roleManager.Roles.Where(r => userRoles.Contains(r.Name)).ToList();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.RoleEnumValue.ToString()));
            }

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
