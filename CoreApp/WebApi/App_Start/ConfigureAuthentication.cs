using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WebApi.Authentication;
using IdentityRole = Microsoft.AspNetCore.Identity.MongoDB.IdentityRole;

namespace WebApi
{
    internal class ConfigureAuthentication
    {
        internal static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString =
                $"{configuration.GetSection("MongoConnection:ConnectionString").Value}/{configuration.GetSection("MongoConnection:Database").Value}";
            services.AddIdentityWithMongoStoresUsingCustomTypes<AppUser, IdentityRole>(connectionString)
                .AddDefaultTokenProviders();
            
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
                    };
                });

        }
    }
}
