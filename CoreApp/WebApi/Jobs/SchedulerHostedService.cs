using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Jobs
{
    public class SchedulerHostedService : BaseHostedService
    {
        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        private readonly List<ScheduleTaskWrapper> _scheduledTasks = new List<ScheduleTaskWrapper>();

        public SchedulerHostedService(IEnumerable<IScheduledTask> scheduledTasks)
        {
            foreach (var scheduledTask in scheduledTasks)
            {
                _scheduledTasks.Add(new ScheduleTaskWrapper(scheduledTask));
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var taskFactory = new TaskFactory(TaskScheduler.Current);
                var referenceTime = DateTime.Now;

                var tasksThatShouldRun = _scheduledTasks.Where(t => t.ShouldRun(referenceTime)).ToList();

                foreach (var taskThatShouldRun in tasksThatShouldRun)
                {
                    var currentTask = taskThatShouldRun.CurrenTask;

                    if (currentTask != null && currentTask.Status == TaskStatus.Running)
                    {
                        continue;
                    }

                    taskThatShouldRun.CurrenTask = await taskFactory.StartNew(
                        async () =>
                        {
                            try
                            {
                                await taskThatShouldRun.RunTask(cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                var args = new UnobservedTaskExceptionEventArgs(
                                    ex as AggregateException ?? new AggregateException(ex));

                                UnobservedTaskException?.Invoke(this, args);

                                if (!args.Observed)
                                {
                                    throw;
                                }
                            }
                        },
                        cancellationToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(12), cancellationToken);
            }
        }
    }
}
