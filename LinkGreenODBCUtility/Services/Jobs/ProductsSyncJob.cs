using LinkGreen.Email;
using Quartz;

// ReSharper disable once CheckNamespace
namespace LinkGreenODBCUtility
{
    public class ProductsSyncJob : IJob
    {
        private const string JobName = "Products";
        private static readonly Mapping Mapping = new Mapping();

        public void Execute(IJobExecutionContext context)
        {
            var notificationEmail = Settings.GetNotificationEmail();
            Logger.Instance.Info($"Job started: {GetType().Name}");
            var tasks = new Tasks();
            tasks.StartTask(JobName);

            var products = new Products();
            products.UpdateTemporaryTables();
            products.Empty();

            var mappedDsnName = Mapping.GetDsnName("Products");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Products") && products.Publish(out var publishDetails))
            {
                Logger.Instance.Info("Products synced.");
                tasks.SetStatus(JobName, "Success");
                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    Mail.SendProcessCompleteEmail(notificationEmail, publishDetails, $"{JobName} Publish",
                        response => Logger.Instance.Info(response));

            }
            else
            {
                Logger.Instance.Error("Products failed to sync.");
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
