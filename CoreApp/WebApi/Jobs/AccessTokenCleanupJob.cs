using System;
using System.Threading.Tasks;
using Business.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NCrontab;
using ScheduledJobs;
using WebApi.Extensions;

namespace WebApi.Jobs
{
    public class AccessTokenCleanupJob : BaseScheduledHostedService
    {
        private readonly ITokenProvider _tokenProvider;

        public AccessTokenCleanupJob(
            IConfiguration config,
            ILogger<AccessTokenCleanupJob> logger,
            ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;

            Schedule = CrontabSchedule.Parse(config.GetCron(nameof(AccessTokenCleanupJob)));
            NextRun = Schedule.GetNextOccurrence(DateTime.Now);

            Logger = logger;
        }

        protected override ILogger Logger { get; }

        protected override Task Process()
        {
            Logger.LogInformation($"{nameof(AccessTokenCleanupJob)} start process at {LastRun}");
            _tokenProvider.DeleteAccessToken(x => DateTimeOffset.Now > x.ExpirationDate);

            return Task.CompletedTask;
        }
    }
}