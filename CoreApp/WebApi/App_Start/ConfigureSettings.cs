using Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Models;
using WebApi.Models.Settings;

namespace WebApi
{
    internal class ConfigureSettings
    {
        internal static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoConnectionModel>(options =>
            {
                options.ConnectionString = configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.Database = configuration.GetSection("MongoConnection:Database").Value;
            });

            services.Configure<JwtSettingsModel>(options =>
            {
                options.SecretKey = configuration.GetSection("Jwt:SecretKey").Value;
                options.Issuer = configuration.GetSection("Jwt:Issuer").Value;
                options.Audience = configuration.GetSection("Jwt:Audience").Value;
                options.ExpitarionsOffsetInMinutes = int.TryParse(configuration.GetSection("Jwt:ExpitarionsOffsetInMinutes").Value, out var expiration) ? expiration : 0;
            });


        }
    }
}
