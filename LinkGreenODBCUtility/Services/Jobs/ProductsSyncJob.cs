using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkGreen.Email;
using Quartz;

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
            var Tasks = new Tasks();
            Tasks.StartTask(JobName);

            var products = new Products();
            products.UpdateTemporaryTables();
            products.Empty();

            string mappedDsnName = Mapping.GetDsnName("Products");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Products") && products.Publish(out var publishDetails))
            {
                Logger.Instance.Info("Products synced.");
                Tasks.SetStatus(JobName, "Success");
                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    Mail.SendProcessCompleteEmail(notificationEmail, publishDetails, $"{JobName} Publish",
                        response => Logger.Instance.Info(response));

            }
            else
            {
                Logger.Instance.Error("Products failed to sync.");
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
