using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScheduledJobs;
using ScheduledJobs.Examples;
using WebApi.Jobs;

namespace WebApi
{
    internal class ConfigeScheduledTasks
    {
        internal static void Configure(IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            //uncomment us to see scheduler in action
            //services.AddSingleton<IScheduledTask, ScheduledTask1>();
            //services.AddSingleton<IScheduledTask, ScheduledTask2>();

            //services.AddHostedService<ScheduledJob1>();
            //services.AddHostedService<ScheduledJob2>();

            services.AddHostedService<RefreshTokenCleanupJob>();
            services.AddHostedService<AccessTokenCleanupJob>();

            var logger = loggerFactory.CreateLogger("ScheduledTasks");
            services.AddTransient<IHostedService, SchedulerHostedService>(serviceProvider =>
            {
                var instance = new SchedulerHostedService(serviceProvider.GetServices<IScheduledTask>());
                instance.UnobservedTaskException += (sender, args) =>
                {
                    logger.LogError(args.Exception, args.Exception.Message);
                    args.SetObserved();
                };
                return instance;
            });
        }
    }
}
