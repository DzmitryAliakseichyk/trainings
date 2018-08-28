using System.Threading.Tasks;

namespace ScheduledJobs
{
    public static class TaskStatuses
    {
        public static readonly TaskStatus[] FinishedStatuses =
        {
            TaskStatus.RanToCompletion,
            TaskStatus.Canceled,
            TaskStatus.Faulted
        };
    }
}