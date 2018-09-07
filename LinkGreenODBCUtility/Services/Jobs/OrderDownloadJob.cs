using LinkGreen.Email;
using Quartz;

namespace LinkGreenODBCUtility.Services.Jobs
{
    public class OrderDownloadJob : IJob
    {
        private const string JobName = "DownloadOrders";

        public void Execute(IJobExecutionContext context)
        {
            var notificationEmail = Settings.GetNotificationEmail();
            Logger.Instance.Info($"Job started: {GetType().Name}");
            var tasks = new Tasks();
            tasks.StartTask(JobName);

            var orders = new OrdersFromLinkGreen();
            var result = orders.Publish(out var publishDetails);
            if (result) {
                Logger.Instance.Info("Orders downloaded.");
                tasks.SetStatus(JobName, "Success");

                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    Mail.SendProcessCompleteEmail(notificationEmail, publishDetails, "Order Download",
                        response => Logger.Instance.Info(response));

            } else {
                Logger.Instance.Error("Orders failed to Download. No API Key was found");
                tasks.SetStatus(JobName, "Failed");

                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    Mail.SendProcessCompleteEmail(notificationEmail, "Order download failed, please check logs or contact support", "Order Download",
                        response => Logger.Instance.Info(response));

            }

            tasks.EndTask(JobName);
            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}