using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LinkGreenODBCUtility
{
    public class ProductsSyncJob : IJob
    {
        private static string jobName = "Products";
        private static Mapping Mapping = new Mapping();

        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.Info($"Job started: {GetType().Name}");
            var Tasks = new Tasks();
            Tasks.StartTask(jobName);

            var products = new Products();
            products.UpdateTemporaryTables();
            products.Empty();

            string mappedDsnName = Mapping.GetDsnName("Products");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Products") && products.Publish())
            {
                Logger.Instance.Info("Products synced.");
                Tasks.SetStatus(jobName, "Success");
            }
            else
            {
                Logger.Instance.Error("Products failed to sync.");
                Tasks.SetStatus(jobName, "Failed");
            }

            Tasks.EndTask(jobName);
            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
