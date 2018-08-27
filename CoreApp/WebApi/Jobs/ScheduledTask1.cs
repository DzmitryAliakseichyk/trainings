using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Jobs
{
    public class ScheduledTask1 : IScheduledTask
    {
        public string Cron => "*/1 * * * *";

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("SheduleTask1");
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"SheduleTask1 In progress {i} from {1000} is done");
                Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).GetAwaiter().GetResult();
            }
            throw new NullReferenceException();
        }
    }
}
