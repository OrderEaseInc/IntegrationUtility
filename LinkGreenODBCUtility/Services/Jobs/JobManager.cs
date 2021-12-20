using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using LinkGreenODBCUtility.Services.Jobs;

using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

// ReSharper disable once CheckNamespace
namespace LinkGreenODBCUtility
{
    public static class JobManager
    {
        // construct a scheduler factory
        public static ISchedulerFactory SchedFact = new StdSchedulerFactory();
        // get a scheduler
        public static IScheduler Sched = SchedFact.GetScheduler();

        public static string DefaultGroup = "User";

        public static bool ScheduleJob(string jobName, DateTime startDateTime, int repeatInterval, string externalExecutable, string jobParameters)
        {
            if (!string.IsNullOrEmpty(jobName) && repeatInterval > 0)
            {
                try
                {
                    Sched.Start();

                    IJobDetail job;

                    if (Guid.TryParse(jobName, out var _))
                    {
                        job = JobBuilder.Create<ExternalExecuteJob>()
                            .WithIdentity(jobName, DefaultGroup)
                            .Build();
                        job.JobDataMap["ExternalExecutable"] = externalExecutable;
                        job.JobDataMap["JobParameters"] = jobParameters;
                    }
                    else
                    {
                        switch (jobName)
                        {
                            case "Categories":
                                job = JobBuilder.Create<CategoriesSyncJob>()
                                    .WithIdentity(jobName, DefaultGroup)
                                    .Build();
                                break;
                            case "Customers":
                                job = JobBuilder.Create<CustomersSyncJob>()
                                    .WithIdentity(jobName, DefaultGroup)
                                    .Build();
                                break;
                            case "Products":
                                job = JobBuilder.Create<ProductsSyncJob>()
                                    .WithIdentity(jobName, DefaultGroup)
                                    .Build();
                                break;
                            case "InventoryQuantities":
                                job = JobBuilder.Create<InventoryQuantitiesSyncJob>()
                                    .WithIdentity(jobName, DefaultGroup)
                                    .Build();
                                break;
                            case "Price Levels":
                                job = JobBuilder.Create<PriceLevelsSyncJob>()
                                    .WithIdentity(jobName, DefaultGroup)
                                    .Build();
                                break;
                            case "Pricing":
                                job = JobBuilder.Create<PricingSyncJob>()
                                    .WithIdentity(jobName, DefaultGroup)
                                    .Build();
                                break;
                            case "Suppliers":
                                job = JobBuilder.Create<SuppliersSyncJob>()
                                    .WithIdentity(jobName, DefaultGroup)
                                    .Build();
                                break;
                            case "SupplierInventory":
                                job = JobBuilder.Create<SupplierInventorySyncJob>()
                                    .WithIdentity(jobName, DefaultGroup)
                                    .Build();
                                break;
                            case "LinkedSkus":
                                job = JobBuilder.Create<LinkedSKusSyncJob>()
                                    .WithIdentity(jobName, DefaultGroup)
                                    .Build();
                                break;
                            case "BuyerInventory":
                                job = JobBuilder.Create<BuyerInventorySyncJob>()
                                    .WithIdentity(jobName, DefaultGroup)
                                    .Build();
                                break;
                            case "DownloadOrders":
                                job = JobBuilder.Create<OrderDownloadJob>()
                                    .WithIdentity(jobName, DefaultGroup)
                                    .Build();
                                break;
                            default:
                                job = JobBuilder.Create<CategoriesSyncJob>()
                                    .WithIdentity(jobName, DefaultGroup)
                                    .Build();
                                break;
                        }
                    }

                    var trigger = TriggerBuilder.Create()
                        .WithSimpleSchedule(x => x.WithIntervalInMinutes(repeatInterval).RepeatForever())
                        .StartAt(startDateTime)
                        .Build();

                    Sched.ScheduleJob(job, trigger);

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error($"An error occurred while creating task {jobName}: {ex}");
                    MessageBox.Show($@"An error occurred while creating task {jobName}. Does the task already exist?", @"Emptied Successfully");
                }
            }

            return false;
        }

        private static JobKey GetJobKey(string jobName, string groupName = null)
        {
            if (string.IsNullOrEmpty(groupName))
                groupName = DefaultGroup;

            var groupMatcher = GroupMatcher<JobKey>.GroupContains(groupName);
            JobKey jobKey = Sched.GetJobKeys(groupMatcher).FirstOrDefault(s => s.Name == jobName);
            return jobKey;
        }

        private static TriggerKey GetTriggerKey(string triggerName, string groupName = null)
        {
            if (string.IsNullOrEmpty(groupName))
                groupName = DefaultGroup;

            var groupMatcher = GroupMatcher<TriggerKey>.GroupContains(groupName);
            TriggerKey key = Sched.GetTriggerKeys(groupMatcher).FirstOrDefault(s => s.Name == triggerName);
            return key;
        }

        public static IJobDetail GetJob(string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
            {
                return null;
            }

            JobKey jobKey = GetJobKey(jobName);
            if (jobKey == null)
            {
                return null;
            }

            IJobDetail job = Sched.GetJobDetail(jobKey);
            return job;
        }

        public static List<IJobDetail> GetJobs()
        {
            List<IJobDetail> jobs = new List<IJobDetail>();
            var groupMatcher = GroupMatcher<JobKey>.GroupContains(DefaultGroup);
            var jobKeys = Sched.GetJobKeys(groupMatcher);
            foreach (var jobKey in jobKeys)
            {
                IJobDetail detail = Sched.GetJobDetail(jobKey);

                jobs.Add(detail);
            }

            return jobs;
        }

        public static List<IJobExecutionContext> GetCurrentlyExecutingJobs()
        {
            List<IJobExecutionContext> jobs = new List<IJobExecutionContext>();
            var currentJobs = Sched.GetCurrentlyExecutingJobs();
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
                Sched.DeleteJob(jobKey);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool PauseJob(string jobName)
        {
            try
            {
                JobKey jobKey = GetJobKey(jobName);
                Sched.PauseJob(jobKey);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ResumeJob(string jobName)
        {
            try
            {
                JobKey jobKey = GetJobKey(jobName);
                Sched.ResumeJob(jobKey);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsTriggerPaused(string triggerName, string triggerGroup)
        {
            TriggerKey key = GetTriggerKey(triggerName, triggerGroup);
            return Sched.GetTriggerState(key) == TriggerState.Paused;
        }

        public static void Dispose()
        {
            Sched.Shutdown(true); // Wait for jobs to complete and then shutdown
        }
    }
}
