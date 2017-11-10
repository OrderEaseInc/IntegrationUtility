using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LinkGreenODBCUtility
{
    public class PriceLevelsSyncJob : IJob
    {
        private static Mapping Mapping = new Mapping();

        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.Info($"Job started: {GetType().Name}");

            var priceLevels = new PriceLevels();
            priceLevels.UpdateTemporaryTables();
            priceLevels.Empty();

            string mappedDsnName = Mapping.GetDsnName("PriceLevels");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("PriceLevels") && priceLevels.Publish())
            {
                Logger.Instance.Info("Price Levels synced.");
            }
            else
            {
                Logger.Instance.Error("Price Levels failed to sync.");
            }

            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
