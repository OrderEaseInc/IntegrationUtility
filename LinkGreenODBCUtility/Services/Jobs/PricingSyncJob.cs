using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace LinkGreenODBCUtility
{
    public class PricingSyncJob : IJob
    {
        private static Mapping Mapping = new Mapping();

        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.Info($"Job started: {GetType().Name}");

            var priceLevelPrices = new PriceLevelPrices();
            priceLevelPrices.UpdateTemporaryTables();
            priceLevelPrices.Empty();

            string mappedDsnName = Mapping.GetDsnName("PriceLevelPrices");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("PriceLevelPrices") && priceLevelPrices.Publish())
            {
                Logger.Instance.Info("Pricing synced.");
            }
            else
            {
                Logger.Instance.Error("Pricing failed to sync.");
            }

            Logger.Instance.Info($"Job finished: {GetType().Name}");
        }
    }
}
