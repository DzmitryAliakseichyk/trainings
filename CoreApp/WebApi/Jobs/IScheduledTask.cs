using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Jobs
{
    public interface IScheduledTask
    {
        string Cron { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
