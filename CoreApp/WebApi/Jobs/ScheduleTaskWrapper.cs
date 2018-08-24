using System;
using System.Threading;
using System.Threading.Tasks;
using NCrontab;

namespace WebApi.Jobs
{
    internal class ScheduleTaskWrapper
    {
        private readonly CrontabSchedule _schedule;

        private DateTime _lastRunTime;

        private DateTime _nextRunTime;

        public Task CurrenTask { get; set; } = null;

        private IScheduledTask _task { get; }

        public ScheduleTaskWrapper(IScheduledTask task)
        {
            _task = task;
            _schedule = CrontabSchedule.Parse(_task.Cron);
            _nextRunTime = _schedule.GetNextOccurrence(DateTime.Now);
        }

        public void SetNextRunTime()
        {
            _lastRunTime = _nextRunTime;
            _nextRunTime = _schedule.GetNextOccurrence(_nextRunTime);
        }

        public Task RunTask(CancellationToken cancelationToken)
        {
            return _task.ExecuteAsync(cancelationToken);
        }


        public bool ShouldRun(DateTime currentTime)
        {
            return _nextRunTime < currentTime && _lastRunTime != _nextRunTime;
        }
    }
}
