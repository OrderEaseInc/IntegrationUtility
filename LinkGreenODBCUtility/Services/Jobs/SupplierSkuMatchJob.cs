using Quartz;

namespace LinkGreenODBCUtility.Services.Jobs
{
    public class SupplierSkuMatchJob : IJob
    {
        public const string JobName = "PublishMatchedSupplierSkus";

        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.Info($"Job started: ${GetType().Name}");
            var tasks = new Tasks();
            tasks.StartTask(JobName);

            var supplierInventories = new SupplierInventories();
            var result = supplierInventories.PushMatchedSkus();
            if (result) {
                Logger.Instance.Info("Matched Supplier Inventory Synced.");
                tasks.SetStatus(JobName, "Success");
            } else {
                Logger.Instance.Error("Supplier Inventory failed to Publish. No API Key was found");
                tasks.SetStatus(JobName, "Failed");
            }

            tasks.EndTask(JobName);
            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}