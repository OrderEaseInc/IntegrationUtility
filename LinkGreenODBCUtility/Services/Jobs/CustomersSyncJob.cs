using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LinkGreenODBCUtility
{
    public class CustomersSyncJob : IJob
    {
        private static Mapping Mapping = new Mapping();

        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.Info($"Job started: {GetType().Name}");

            var customers = new Customers();
            customers.UpdateTemporaryTables();
            customers.Empty();

            string mappedDsnName = Mapping.GetDsnName("Customers");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Customers") && customers.Publish())
            {
                Logger.Instance.Info("Customers synced.");
            }
            else
            {
                Logger.Instance.Error("Customers failed to sync.");
            }

            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
