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
        private static Mapping Mapping = new Mapping();

        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.Info($"Job started: {GetType().Name}");

            var products = new Products();
            products.UpdateTemporaryTables();
            products.Empty();

            string mappedDsnName = Mapping.GetDsnName("Products");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Products") && products.Publish())
            {
                Logger.Instance.Info("Products synced.");
            }
            else
            {
                Logger.Instance.Error("Products failed to sync.");
            }

            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
