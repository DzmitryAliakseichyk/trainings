using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace ScheduledJobs.Examples
{
    public class ScheduledJob1 : BaseScheduledHostedService
    {
        private const string Cron = "*/1 * * * *";

        public ScheduledJob1(ILogger<ScheduledJob1> logger)
        {
            Logger = logger;
            Schedule = CrontabSchedule.Parse(Cron);
            NextRun = Schedule.GetNextOccurrence(DateTime.Now);
        }

        protected override ILogger Logger { get; }

        protected override Task Process()
        {
            Logger.LogInformation(nameof(ScheduledJob1));
            for (int i = 0; i < 20; i++)
            {
                Logger.LogInformation($"{nameof(ScheduledJob1)} In progress {i} from {1000} is done");
                Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult();
            }
            throw new NullReferenceException();
        }
    }
}
