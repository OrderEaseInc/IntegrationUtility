using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

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
                catch (Exception ex)
                {
                    Logger.Instance.Error($"An error occured while creating task {jobName}: {ex}");
                    MessageBox.Show($"An error occured while creating task {jobName}. Does the task already exist?", "Error");
                }
            }

            return false;
        }

        private static JobKey GetJobKey(string jobName, string groupName = "User")
        {
            var groupMatcher = GroupMatcher<JobKey>.GroupContains(groupName);
            JobKey jobKey = sched.GetJobKeys(groupMatcher).FirstOrDefault(s => s.Name == jobName);
            return jobKey;
        }

        private static TriggerKey GetTriggerKey(string triggerName, string groupName = "User")
        {
            var groupMatcher = GroupMatcher<TriggerKey>.GroupContains(groupName);
            TriggerKey key = sched.GetTriggerKeys(groupMatcher).FirstOrDefault(s => s.Name == triggerName);
            return key;
        }

        public static IJobDetail GetJob(string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
            {
                return null;
            }

            JobKey jobKey = GetJobKey(jobName);
            IJobDetail job = sched.GetJobDetail(jobKey);
            return job;
        }

        public static List<IJobDetail> GetJobs()
        {
            List<IJobDetail> jobs = new List<IJobDetail>();
            var groupMatcher = GroupMatcher<JobKey>.GroupContains("User");
            var jobKeys = sched.GetJobKeys(groupMatcher);
            foreach (var jobKey in jobKeys)
            {
                IJobDetail detail = sched.GetJobDetail(jobKey);

                jobs.Add(detail);
            }

            return jobs;
        }

        public static List<IJobExecutionContext> GetCurrentlyExecutingJobs()
        {
            List<IJobExecutionContext> jobs = new List<IJobExecutionContext>();
            var currentJobs = sched.GetCurrentlyExecutingJobs();
            foreach (var job in currentJobs)
            {
                jobs.Add(job);
            }

            return jobs;
        }

        public static bool DeleteJob(string jobName)
        {
            try
            {
                JobKey jobKey = GetJobKey(jobName);
                sched.DeleteJob(jobKey);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool PauseJob(string jobName)
        {
            try
            {
                JobKey jobKey = GetJobKey(jobName);
                sched.PauseJob(jobKey);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool ResumeJob(string jobName)
        {
            try
            {
                JobKey jobKey = GetJobKey(jobName);
                sched.ResumeJob(jobKey);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool IsTriggerPaused(string triggerName, string triggerGroup)
        {
            TriggerKey key = GetTriggerKey(triggerName, triggerGroup);
            return sched.GetTriggerState(key) == TriggerState.Paused;
        }

        public static void Dispose()
        {
            sched.Shutdown(true); // Wait for jobs to complete and then shutdown
        }
    }
}
