using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Jobs
{
    public class ScheduledTask2 : IScheduledTask
    {
        public string Cron => "*/1 * * * *";

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("ScheduledTask2");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"SheduleTask2 is in progress {i} from {10} is done");
                Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).GetAwaiter().GetResult();
            }
            return Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
        }
    }
}
