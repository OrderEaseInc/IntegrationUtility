using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LinkGreenODBCUtility.Services.Jobs
{
    public class BuyerInventorySyncJob: IJob
    {
        private static string jobName = "BuyerInventory";

        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.Info($"Job started: {GetType().Name}");

            var Tasks = new Tasks();

            var buyerInventories = new BuyerInventories();
            if (buyerInventories.Publish())
            {
                Logger.Instance.Info("Buyer Inventory Published");
                Tasks.SetStatus(jobName, "Success");
            }
            else
            {
                Logger.Instance.Error("Buyer Inventory failed to publish. No API Key was found");
                Tasks.SetStatus(jobName, "Failed");
            }

            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
