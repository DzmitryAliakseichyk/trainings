using Microsoft.Extensions.DependencyInjection;
using WebApi.Authentication;

namespace WebApi
{
    internal class ConfigureIoc
    {
        internal static void Configure(IServiceCollection services)
        {
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
        }
    }
}
