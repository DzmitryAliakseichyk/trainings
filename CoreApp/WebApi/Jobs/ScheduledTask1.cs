using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Jobs
{
    public class ScheduledTask1 : IScheduledTask
    {
        public string Cron => "*/5 * * * * *";

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("SheduleTask1");
            return Task.FromResult(0);
        }
    }
}
