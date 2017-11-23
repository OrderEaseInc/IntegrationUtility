using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LinkGreenODBCUtility
{
    public class CategoriesSyncJob : IJob
    {
        private static string jobName = "Categories";
        private static Mapping Mapping = new Mapping();

        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.Info($"Job started: {GetType().Name}");
            var Tasks = new Tasks();
            Tasks.StartTask(jobName);

            var categories = new Categories();
            categories.UpdateTemporaryTables();
            categories.Empty();

            string mappedDsnName = Mapping.GetDsnName("Categories");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Categories") && categories.Publish())
            {
                Logger.Instance.Info("Categories synced.");
                Tasks.SetStatus(jobName, "Success");
            }
            else
            {
                Logger.Instance.Error("Categories failed to sync.");
                Tasks.SetStatus(jobName, "Failed");
            }

            Tasks.EndTask(jobName);
            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
