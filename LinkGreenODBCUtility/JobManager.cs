using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace LinkGreenODBCUtility
{
    public static class JobManager
    {
        // construct a scheduler factory
        public static ISchedulerFactory schedFact = new StdSchedulerFactory();
        // get a scheduler
        public static IScheduler sched = schedFact.GetScheduler();

        public static bool ScheduleJob(string jobName, DateTime startDateTime, int repeatInterval)
        {
            if (!string.IsNullOrEmpty(jobName) && repeatInterval > 0)
            {
                try
                {
                    sched.Start();

                    IJobDetail job = JobBuilder.Create<CategoriesSyncJob>()
                        .WithIdentity(jobName, "User")
                        .Build();

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithSimpleSchedule(x => x.WithIntervalInMinutes(repeatInterval).RepeatForever())
                        .StartAt(startDateTime)
                        .Build();

                    sched.ScheduleJob(job, trigger);

                    return true;
                }
                catch (ArgumentException ex)
                {
                    Logger.Instance.Error($"An error occured while creating task {jobName}: {ex}");
                }
            }

            return false;
        }

        public static void Dispose()
        {
            // Wait for jobs to complete and then shutdown
            sched.Shutdown(true);
        }
    }
}
