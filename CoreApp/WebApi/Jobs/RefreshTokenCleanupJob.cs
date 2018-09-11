using Business.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NCrontab;
using ScheduledJobs;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Extensions;

namespace WebApi.Jobs
{
    public class RefreshTokenCleanupJob : BaseScheduledHostedService
    {
        private readonly IServiceProvider _provider;

        public RefreshTokenCleanupJob(
            IConfiguration config,
            ILogger<RefreshTokenCleanupJob> logger,
            IServiceProvider provider) 
        {
            _provider = provider;

            Schedule = CrontabSchedule.Parse(config.GetCron(nameof(RefreshTokenCleanupJob)));
            NextRun = Schedule.GetNextOccurrence(DateTime.Now);

            Logger = logger;
        }

        protected override ILogger Logger { get; }

        protected override Task Process()
        {
            using (var scope = _provider.CreateScope())
            {
                var tokenProvider = scope.ServiceProvider.GetRequiredService<ITokenProvider>();
                Logger.LogInformation($"{nameof(RefreshTokenCleanupJob)} start process at {LastRun}");
                tokenProvider.DeleteRefreshToken(x => DateTimeOffset.Now > x.ExpirationDate);
            }
            
            return Task.CompletedTask;
        }
    }
}