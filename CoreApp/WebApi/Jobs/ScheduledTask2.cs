using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Jobs
{
    public class ScheduledTask2 : IScheduledTask
    {
        public string Cron => "*/2 * * * *";

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("ScheduledTask2");
            return Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
        }
    }
}
