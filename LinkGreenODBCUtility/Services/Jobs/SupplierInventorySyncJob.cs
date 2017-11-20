using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LinkGreenODBCUtility.Services.Jobs
{
    public class SupplierInventorySyncJob: IJob
    {
        private static string jobName = "SupplierInventory";

        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.Info($"Job started: {GetType().Name}");
            var Tasks = new Tasks();
            Tasks.StartTask(jobName);

            var supplierInventories = new SupplierInventories();
            var result = supplierInventories.PushMatchedSkus();
            if (result)
            {
                Logger.Instance.Info("Matched Supplier Inventory Synced.");
                Tasks.SetStatus(jobName, "Success");
            }
            else
            {
                Logger.Instance.Error("Supplier Inventory failed to Publish. No API Key was found");
                Tasks.SetStatus(jobName, "Failed");
            }

            Tasks.EndTask(jobName);
            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
