using LinkGreen.Email;
using Quartz;

namespace LinkGreenODBCUtility.Services.Jobs
{
    public class LinkedSKusSyncJob : IJob
    {
        private const string JobName = "LinkedSkus";

        public void Execute(IJobExecutionContext context)
        {
            var notificationEmail = Settings.GetNotificationEmail();
            Logger.Instance.Info($"Job started: {GetType().Name}");
            var Tasks = new Tasks();
            Tasks.StartTask(JobName);

            var linkedSkus = new LinkedSkus();
            var result = linkedSkus.Publish(out var publishDetails);
            if (result)
            {
                linkedSkus.Empty();
                Logger.Instance.Info("Linked Skus Published");
                Tasks.SetStatus(JobName, "Success");
                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    Mail.SendProcessCompleteEmail(notificationEmail, publishDetails, $"{JobName} Publish",
                        response => Logger.Instance.Info(response));
            }
            else
            {
                Logger.Instance.Error("Linked Skus failed to Publish. Is your API key set?");
                Tasks.SetStatus(JobName, "Failed");

                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    Mail.SendProcessCompleteEmail(notificationEmail, "Supplier Publish failed, please check logs or contact support", $"{JobName} Publish",
                        response => Logger.Instance.Info(response));
            }

            Tasks.EndTask(JobName);
            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
