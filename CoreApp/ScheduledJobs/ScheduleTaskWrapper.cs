using System;
using System.Threading;
using System.Threading.Tasks;
using NCrontab;

namespace ScheduledJobs
{
    internal class ScheduleTaskWrapper
    {
        private readonly CrontabSchedule _schedule;

        private DateTime _lastRunTime;

        private DateTime _nextRunTime;

        public Task CurrenTask { get; set; } = null;

        private IScheduledTask Task { get; }

        public ScheduleTaskWrapper(IScheduledTask task)
        {
            Task = task;
            _schedule = CrontabSchedule.Parse(Task.Cron);
            _nextRunTime = _schedule.GetNextOccurrence(DateTime.Now);
        }

        public void SetNextRunTime()
        {
            _lastRunTime = _nextRunTime;
            _nextRunTime = _schedule.GetNextOccurrence(_nextRunTime);
        }

        public Task RunTask(CancellationToken cancelationToken)
        {
            return Task.ExecuteAsync(cancelationToken);
        }
        
        public bool ShouldRun(DateTime currentTime)
        {
            return _nextRunTime < currentTime && _lastRunTime != _nextRunTime;
        }
    }
}
