using Business.Providers;
using Data;
using Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using WebApi.Authentication;

namespace WebApi
{
    internal class ConfigureIoc
    {
        internal static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddSingleton<IPasswordGenerator, PasswordGenerator>();

            services.AddSingleton<IMongoWrapper, MongoWrapper>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<IAccessTokenRepository, AccessTokenRepository>();
            services.AddTransient<ITokenProvider, TokenProvider>();
        }
    }
}
