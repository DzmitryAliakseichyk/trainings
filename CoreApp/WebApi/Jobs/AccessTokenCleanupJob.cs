using System;
using System.Threading.Tasks;
using Business.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NCrontab;
using ScheduledJobs;
using WebApi.Extensions;

namespace WebApi.Jobs
{
    public class AccessTokenCleanupJob : BaseScheduledHostedService
    {
        private readonly IServiceProvider _provider;

        public AccessTokenCleanupJob(
            IConfiguration config,
            ILogger<AccessTokenCleanupJob> logger,
            IServiceProvider provider)
        {
            _provider = provider;

            Schedule = CrontabSchedule.Parse(config.GetCron(nameof(AccessTokenCleanupJob)));
            NextRun = Schedule.GetNextOccurrence(DateTime.Now);

            Logger = logger;
        }

        protected override ILogger Logger { get; }

        protected override Task Process()
        {
            using (var scope = _provider.CreateScope())
            {
                var tokenProvider = scope.ServiceProvider.GetRequiredService<ITokenProvider>();
                Logger.LogInformation($"{nameof(AccessTokenCleanupJob)} start process at {LastRun}");
                tokenProvider.DeleteAccessToken(x => DateTimeOffset.Now > x.ExpirationDate);
            }

            return Task.CompletedTask;
        }
    }
}