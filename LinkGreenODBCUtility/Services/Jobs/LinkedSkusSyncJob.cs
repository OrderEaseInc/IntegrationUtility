using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LinkGreenODBCUtility.Services.Jobs
{
    public class LinkedSKusSyncJob: IJob
    {
        private static string jobName = "LinkedSkus";

        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.Info($"Job started: {GetType().Name}");
            var Tasks = new Tasks();
            Tasks.StartTask(jobName);

            var linkedSkus = new LinkedSkus();
            var result = linkedSkus.Publish();
            if (result)
            {
                linkedSkus.Empty();
                Logger.Instance.Info("Linked Skus Published");
                Tasks.SetStatus(jobName, "Success");
            }
            else
            {
                Logger.Instance.Error("Linked Skus failed to Publish. Is your API key set?");
                Tasks.SetStatus(jobName, "Failed");
            }

            Tasks.EndTask(jobName);
            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
