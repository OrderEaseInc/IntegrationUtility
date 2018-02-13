using LinkGreen.Email;
using Quartz;

namespace LinkGreenODBCUtility.Services.Jobs
{
    public class BuyerInventorySyncJob : IJob
    {
        private const string JobName = "BuyerInventory";

        public void Execute(IJobExecutionContext context)
        {
            var notificationEmail = Settings.GetNotificationEmail();
            Logger.Instance.Info($"Job started: {GetType().Name}");
            var tasks = new Tasks();
            tasks.StartTask(JobName);

            var buyerInventories = new BuyerInventories();
            if (buyerInventories.Publish(out var publishDetails))
            {
                Logger.Instance.Info("Buyer Inventory Published");
                tasks.SetStatus(JobName, "Success");

                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    Mail.SendProcessCompleteEmail(notificationEmail, publishDetails, $"{JobName} Publish",
                        response => Logger.Instance.Info(response));
            }
            else
            {
                Logger.Instance.Error("Buyer Inventory failed to publish. No API Key was found");
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
