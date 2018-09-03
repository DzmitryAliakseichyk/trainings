using Business.Providers;
using Data;
using Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using WebApi.Authentication;
using WebApi.Authentication.Generators;
using WebApi.Authentication.Helpers;

namespace WebApi
{
    internal class ConfigureIoc
    {
        internal static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddTransient<IJwtTokenHelper, JwtTokenHelper>();
            services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddSingleton<IPasswordGenerator, PasswordGenerator>();

            services.AddSingleton<IMongoWrapper, MongoWrapper>();
            services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddSingleton<IAccessTokenRepository, AccessTokenRepository>();
            services.AddTransient<ITokenProvider, TokenProvider>();
        }
    }
}
