using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LinkGreenODBCUtility.Services.Jobs
{
    public class SuppliersSyncJob: IJob
    {
        private static string jobName = "Suppliers";

        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.Info($"Job started: {GetType().Name}");

            var Tasks = new Tasks();

            var suppliers = new Suppliers();
            var result = suppliers.Publish();
            if (result)
            {
                Logger.Instance.Info("Suppliers Synced");
                Tasks.SetStatus(jobName, "Success");
            }
            else
            {
                Logger.Instance.Error("Suppliers failed to sync. No API Key was found");
                Tasks.SetStatus(jobName, "Failed");
            }

            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
