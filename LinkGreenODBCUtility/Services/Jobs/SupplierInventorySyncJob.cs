using LinkGreen.Email;
using Quartz;

namespace LinkGreenODBCUtility.Services.Jobs
{
    public class SupplierInventorySyncJob : IJob
    {
        private const string JobName = "SupplierInventory";

        public void Execute(IJobExecutionContext context)
        {
            var notificationEmail = Settings.GetNotificationEmail();
            Logger.Instance.Info($"Job started: {GetType().Name}");
            var tasks = new Tasks();
            tasks.StartTask(JobName);

            var supplierInventories = new SupplierInventories();
            var result = supplierInventories.Publish(out var publishDetails);
            if (result)
            {
                Logger.Instance.Info("Matched Supplier Inventory Synced.");
                tasks.SetStatus(JobName, "Success");

                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    Mail.SendProcessCompleteEmail(notificationEmail, publishDetails, $"{JobName} Publish",
                        response => Logger.Instance.Info(response));

            }
            else
            {
                Logger.Instance.Error("Supplier Inventory failed to Publish. No API Key was found");
                tasks.SetStatus(JobName, "Failed");

                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    Mail.SendProcessCompleteEmail(notificationEmail, $"{JobName} Publish failed, please check logs or contact support", $"{JobName} Publish",
                        response => Logger.Instance.Info(response));

            }

            tasks.EndTask(JobName);
            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
