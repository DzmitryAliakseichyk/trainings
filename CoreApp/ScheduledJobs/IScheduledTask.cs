using System.Threading;
using System.Threading.Tasks;

namespace ScheduledJobs
{
    public interface IScheduledTask
    {
        string Cron { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
