using Business.Providers;
using Data;
using Data.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using MongoDB.Driver;
using WebApi.Authentication;
using WebApi.Authentication.Email;
using WebApi.Authentication.Generators;
using WebApi.Authentication.Helpers;
using WebApi.Mappers;

namespace WebApi
{
    internal class ConfigureIoc
    {
        internal static void Configure(
            IServiceCollection services, 
            IConfiguration configuration,
            IHostingEnvironment env)
        {
            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddTransient<IJwtTokenHelper, JwtTokenHelper>();
            services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddSingleton<IPasswordGenerator, PasswordGenerator>();

            services.AddSingleton<IMongoWrapper, MongoWrapper>();
            services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddSingleton<IAccessTokenRepository, AccessTokenRepository>();
            services.AddTransient<ITokenProvider, TokenProvider>();

            services.AddSingleton<IUserMapper, UserMapper>();

            if (env.IsDevelopment())
            {
                services.AddSingleton<IEmailSender, LocalEmailSender>();
            }
        }
    }
}
