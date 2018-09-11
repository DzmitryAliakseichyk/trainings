using Business.Providers;
using Data.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using Data.Repositories.MsSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using WebApi.Authentication;
using WebApi.Authentication.Generators;
using WebApi.Authentication.Helpers;
using WebApi.Email;
using WebApi.Mappers;

namespace WebApi
{
    internal class ConfigureIoc
    {
        internal static void Configure(IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddTransient<IJwtTokenHelper, JwtTokenHelper>();
            services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddSingleton<IPasswordGenerator, PasswordGenerator>();

            services.AddTransient<DbContext, AppIdentityDbContext>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<IAccessTokenRepository, AccessTokenRepository>();

            services.AddTransient<ITokenProvider, TokenProvider>();

            services.AddSingleton<IUserMapper, UserMapper>();

            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddSingleton<IEmailTemplateProvider, EmailTemplateProvider>();
        }
    }
}
