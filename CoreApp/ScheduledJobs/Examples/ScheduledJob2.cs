using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace ScheduledJobs.Examples
{
    public class ScheduledJob2 : BaseScheduledHostedService
    {
        private const string Cron = "*/1 * * * *";

        public ScheduledJob2(ILogger<ScheduledJob2> logger) 
        {
            Logger = logger;
            Schedule = CrontabSchedule.Parse(Cron);
            NextRun = Schedule.GetNextOccurrence(DateTime.Now);
        }

        protected override ILogger Logger { get; }

        protected override Task Process()
        {
            Logger.LogInformation(nameof(ScheduledJob2));
            for (int i = 0; i < 20; i++)
            {
                Logger.LogInformation($"{nameof(ScheduledJob2)} In progress {i} from {1000} is done");
                Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();
            }
            return Task.Delay(TimeSpan.FromMinutes(5));
        }
    }
}
