using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace ScheduledJobs
{
    public abstract class BaseScheduledHostedService : BaseHostedService
    {
        private Task _currentTask = null;

        private const int RecurrenceInSecs = 60;

        protected CrontabSchedule Schedule { get; set; }

        protected DateTime LastRun { get; set; }

        protected DateTime NextRun { get; set; }
        
        protected abstract ILogger Logger { get; }

        protected abstract Task Process();

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var taskFactory = new TaskFactory(TaskScheduler.Current);
                var currentTime = DateTime.Now;

                if (ShouldRun(currentTime))
                {
                    if (_currentTask != null && !TaskStatuses.FinishedStatuses.Contains(_currentTask.Status))
                    {
                        continue;
                    }

                    if (_currentTask?.IsCompleted == true)
                    {
                        _currentTask.Dispose();
                    }

                    SetNextRun(currentTime);

                    _currentTask = taskFactory
                        .StartNew(
                            async () =>
                            {
                                try
                                {
                                    await Process();
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError(ex, ex.Message);
                                    throw;
                                }
                            },
                            cancellationToken)
                        .Unwrap();
                }

                await Task.Delay(TimeSpan.FromSeconds(RecurrenceInSecs), cancellationToken);
            }
        }

        protected bool ShouldRun(DateTime currentTime)
        {
            return NextRun < currentTime && LastRun != NextRun;
        }

        protected void SetNextRun(DateTime currentTime)
        {
            LastRun = NextRun;
            NextRun = Schedule.GetNextOccurrence(currentTime);
        }
    }
}