using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Jobs
{
    public class ScheduledTask2 : IScheduledTask
    {
        public string Cron => "*/10 * * * * *";

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("ScheduledTask2");
            return Task.FromResult(0);
        }
    }
}
