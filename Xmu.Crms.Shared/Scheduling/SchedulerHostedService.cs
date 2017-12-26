using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xmu.Crms.Shared.Scheduling.Cron;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Shared.Scheduling
{
    //Stolen from https://blog.maartenballiauw.be/post/2017/08/01/building-a-scheduled-cache-updater-in-aspnet-core-2.html
    public class SchedulerHostedService : HostedService
    {
        private readonly List<SchedulerTaskWrapper> _scheduledTasks = new List<SchedulerTaskWrapper>();

        public SchedulerHostedService(IEnumerable<ITimerService> scheduledTasks)
        {
            var referenceTime = DateTime.Now;
            foreach (var scheduledTask in scheduledTasks)
            {
                AddTask(scheduledTask, referenceTime);
            }
        }

        public void AddTask(ITimerService scheduledTask, DateTime? nextRunTime = null)
        {
            foreach ((var container, var method, var cron) in scheduledTask.GetType().GetMethods().SelectMany(m => m.GetCustomAttributes(typeof(CronAttribute), true).OfType<CronAttribute>().Select(c => (scheduledTask, m, c))))
            {
                if (!(cron.Schedule.StartsWith("* ") || cron.Schedule.StartsWith("0 ")))
                {
                    throw new NotImplementedException();
                }
                _scheduledTasks.Add(new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(cron.Schedule.Substring(2).Replace('?', '*')),
                    Container = container,
                    Task = method,
                    NextRunTime = nextRunTime ?? DateTime.Now
                });
            }

        }

        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.Now;

            var tasksThatShouldRun = _scheduledTasks.Where(t => t.ShouldRun(referenceTime)).ToList();

            foreach (var taskThatShouldRun in tasksThatShouldRun)
            {
                taskThatShouldRun.Increment();

                await taskFactory.StartNew(
                    () =>
                    {
                        try
                        {
                            taskThatShouldRun.Task.Invoke(taskThatShouldRun.Container, new object[]{});
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
        }

        private class SchedulerTaskWrapper
        {
            public CrontabSchedule Schedule { private get; set; }
            public ITimerService Container { get; set; }
            public MethodInfo Task { get; set; }
            private DateTime LastRunTime { get; set; }
            public DateTime NextRunTime { private get; set; }

            public void Increment()
            {
                LastRunTime = NextRunTime;
                NextRunTime = Schedule.GetNextOccurrence(NextRunTime);
            }

            public bool ShouldRun(DateTime currentTime) => NextRunTime < currentTime && LastRunTime != NextRunTime;
        }
    }
}