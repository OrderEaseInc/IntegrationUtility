using LinkGreen.Email;
using Quartz;

namespace LinkGreenODBCUtility
{
    public class CustomersSyncJob : IJob
    {
        private const string JobName = "Customers";
        private static readonly Mapping Mapping = new Mapping();

        public void Execute(IJobExecutionContext context)
        {
            var notificationEmail = Settings.GetNotificationEmail();
            Logger.Instance.Info($"Job started: {GetType().Name}");
            var Tasks = new Tasks();
            Tasks.StartTask(JobName);

            var customers = new Customers();
            customers.UpdateTemporaryTables();
            customers.Empty();

            string mappedDsnName = Mapping.GetDsnName("Customers");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Customers") && customers.Publish(out var publishDetails))
            {
                Logger.Instance.Info("Customers synced.");
                Tasks.SetStatus(JobName, "Success");

                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    Mail.SendProcessCompleteEmail(notificationEmail, publishDetails, $"{JobName} Publish",
                        response => Logger.Instance.Info(response));

            }
            else
            {
                Logger.Instance.Error("Customers failed to sync.");
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
