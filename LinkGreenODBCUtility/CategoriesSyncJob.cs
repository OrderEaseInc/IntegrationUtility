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
        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.Info($"Job started: {GetType().Name}");

            var categories = new Categories();
            categories.UpdateTemporaryTables();
            categories.Empty();

            Mapping Mapping = new Mapping();
            string mappedDsnName = Mapping.GetDsnName("Categories");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Categories") && categories.Publish())
            {
                Logger.Instance.Info("Categories synced.");
            }
            else
            {
                Logger.Instance.Error("Categories failed to sync.");
            }

            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
