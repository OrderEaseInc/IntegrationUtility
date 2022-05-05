using System;
using Quartz;

namespace LinkGreenODBCUtility.Services.Jobs
{
    public class PurgeLogJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Instance.Debug("Starting Log Purge");
                Log.PurgeLog();
                Logger.Instance.Debug("Finished Log Purge");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                Logger.Instance.Error(ex.StackTrace);
            }
        }
    }
}