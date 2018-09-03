using Business.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NCrontab;
using ScheduledJobs;
using System;
using System.Threading.Tasks;
using WebApi.Extensions;

namespace WebApi.Jobs
{
    public class RefreshTokenCleanupJob : BaseScheduledHostedService
    {
        private readonly ITokenProvider _tokenProvider;

        public RefreshTokenCleanupJob(
            IConfiguration config,
            ILogger<RefreshTokenCleanupJob> logger,
            ITokenProvider tokenProvider) 
        {
            _tokenProvider = tokenProvider;

            Schedule = CrontabSchedule.Parse(config.GetCron(nameof(RefreshTokenCleanupJob)));
            NextRun = Schedule.GetNextOccurrence(DateTime.Now);

            Logger = logger;
        }

        protected override ILogger Logger { get; }

        protected override Task Process()
        {
            Logger.LogInformation($"{nameof(RefreshTokenCleanupJob)} start process at {LastRun}");
            _tokenProvider.DeleteRefreshToken(x => DateTimeOffset.Now > x.ExpirationDate);

            return Task.CompletedTask;
        }
    }
}