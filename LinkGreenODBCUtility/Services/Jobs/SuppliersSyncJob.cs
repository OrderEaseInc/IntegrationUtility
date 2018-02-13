using LinkGreen.Email;
using Quartz;

namespace LinkGreenODBCUtility.Services.Jobs
{
    public class SuppliersSyncJob : IJob
    {
        private const string JobName = "Suppliers";

        public void Execute(IJobExecutionContext context)
        {
            var notificationEmail = Settings.GetNotificationEmail();
            Logger.Instance.Info($"Job started: {GetType().Name}");
            var Tasks = new Tasks();
            Tasks.StartTask(JobName);

            var suppliers = new Suppliers();
            var result = suppliers.Publish(out var publishDetails);
            if (result)
            {
                Logger.Instance.Info("Suppliers Synced");
                Tasks.SetStatus(JobName, "Success");

                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    Mail.SendProcessCompleteEmail(notificationEmail, publishDetails, $"{JobName} Publish",
                        response => Logger.Instance.Info(response));


            }
            else
            {
                Logger.Instance.Error("Suppliers failed to sync. No API Key was found");
                Tasks.SetStatus(JobName, "Failed");

                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    Mail.SendProcessCompleteEmail(notificationEmail, $"{JobName} Publish failed, please check logs or contact support", $"{JobName} Publish",
                        response => Logger.Instance.Info(response));

            }

            Tasks.EndTask(JobName);
            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
