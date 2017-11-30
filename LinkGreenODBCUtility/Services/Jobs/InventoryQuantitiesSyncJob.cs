using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LinkGreenODBCUtility.Services.Jobs
{
    public class InventoryQuantitiesSyncJob: IJob
    {
        private static string jobName = "InventoryQuantities";

        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.Info($"Job started: {GetType().Name}");
            var Tasks = new Tasks();
            Tasks.StartTask(jobName);

            var inventoryQuantities = new InventoryQuantity();
            if (inventoryQuantities.Publish())
            {
                Logger.Instance.Info("Inventory Quantities Published");
                Tasks.SetStatus(jobName, "Success");
            }
            else
            {
                Logger.Instance.Error("Inventory Quantities failed to publish. No API Key was found");
                Tasks.SetStatus(jobName, "Failed");
            }

            Tasks.EndTask(jobName);
            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
