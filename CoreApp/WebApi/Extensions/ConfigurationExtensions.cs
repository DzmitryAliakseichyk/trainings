using Microsoft.Extensions.Configuration;

namespace WebApi.Extensions
{
    public static class ConfigurationExtensions
    {
        // https://crontab.guru
        private const string DefaultCron = "* * * * *";

        public static string GetCron(this IConfiguration configuration, string className)
        {
            var cron = configuration.GetSection($"Cron:{className}").Value;
            if (string.IsNullOrWhiteSpace(cron))
            {
                return DefaultCron;
            }

            return cron;
        }
    }
}