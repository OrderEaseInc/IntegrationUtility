using System;
using System.ComponentModel;
using System.Linq;
using LinkGreen.Email;

namespace LinkGreenODBCUtility.RunnableActivities
{
    internal class DownloadOrderRunner : IRunnableActivity
    {
        public RunnableResult Run(BackgroundWorker bw)
        {
            RunnableResult result;
            bw?.ReportProgress(0, "Processing order download\n\rPlease wait");
            var notificationEmail = Settings.GetNotificationEmail();

            try
            {

                var orders = new OrdersFromLinkGreen();
                orders.Empty();
                var published = orders.Publish(out var orderPublishDetails);
                if (published)
                {

                    if (!string.IsNullOrEmpty(notificationEmail))
                    {
                        Mail.SendProcessCompleteEmail(notificationEmail, orderPublishDetails, "Order Download",
                            response => Logger.Instance.Info(response));
                    }

                    result = new RunnableResult
                    {
                        Message = "Orders Downloaded",
                        Title = "Success",
                        Error = string.Empty,
                        InfoMessage = string.Empty
                    };

                }
                else
                {

                    result = new RunnableResult
                    {
                        Message = $"Orders were not downloaded. Do you have your API Key set?\n\n{orderPublishDetails.FirstOrDefault()}",
                        Title = "Download Failure",
                        Error = "Orders were not downloaded.",
                        InfoMessage = string.Empty
                    };

                    if (!string.IsNullOrWhiteSpace(notificationEmail))
                        Mail.SendProcessCompleteEmail(notificationEmail, "Order Download failed, please check logs or contact support", "Order Download",
                            response => Logger.Instance.Error(response));
                }

            }
            catch (Exception ex)
            {

                result = new RunnableResult
                {
                    Message = "Orders were not downloaded. Do you have your API Key set?",
                    Title = "Download Failure",
                    Error = "Orders were not downloaded.",
                    InfoMessage = ex.GetBaseException().Message
                };

                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    Mail.SendProcessCompleteEmail(notificationEmail, "Order Download failed, please check logs or contact support", "Order Download",
                        response => Logger.Instance.Error(response));
            }

            return result;
        }
    }
}
