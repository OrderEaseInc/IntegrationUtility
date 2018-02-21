using System;
using System.Diagnostics;
using Quartz;

namespace LinkGreenODBCUtility.Services.Jobs
{
    public class ExternalExecuteJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Instance.Info($"Job started: External Task {GetType().Name}");
                Logger.Instance.Info(
                    $"Job started: Starting {context.JobDetail.JobDataMap.GetString("ExternalExecutable")}");
                Process.Start(context.JobDetail.JobDataMap.GetString("ExternalExecutable"), context.JobDetail.JobDataMap.GetString("JobParameters"));
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Exception: External Task {GetType().Name}: {ex.Message}");
            }
            finally
            {
                Logger.Instance.Info($"Job Finished: External Task {GetType().Name}");
            }
        }
    }
}
