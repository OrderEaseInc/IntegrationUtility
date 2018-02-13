using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

namespace LinkGreenODBCUtility
{
    class PriceLevelPrices : IOdbcTransfer
    {
        //public string ConnectionString = $"DSN={Settings.DsnName}";
        public string ClientConnectionString;

        public PriceLevelPrices()
        {

        }

        public PriceLevelPrices(string clientDsnName)
        {
            ClientConnectionString = "DSN=" + clientDsnName;
        }

        public bool Empty()
        {
            var productPriceRepository = new ProductPriceRepository(Settings.ConnectionString);
            productPriceRepository.ClearAll();
            return true;
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            var productPriceRepository = new ProductPriceRepository(Settings.ConnectionString);
            productPriceRepository.SaveTableMapping(dsnName, tableName, "PriceLevelPrices");
            Logger.Instance.Debug($"Product Pricing table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            var productPriceRepository = new ProductPriceRepository(Settings.ConnectionString);
            productPriceRepository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Product Pricing field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public void UpdateTemporaryTables()
        {
            BatchTaskManager batchTaskManager = new BatchTaskManager("PriceLevelPrices");
            List<string> commands = batchTaskManager.GetCommandsByTrigger();
            foreach (string cmd in commands)
            {
                Batch.Exec(cmd);
            }
        }

        public bool Publish(out List<string> publishDetails, BackgroundWorker bw = null)
        {
            publishDetails = new List<string>();
            string apiKey = Settings.GetApiKey();

            if (!string.IsNullOrEmpty(apiKey))
            {
                var pricesToImport = new ProductPriceRepository(Settings.ConnectionString).GetAll().ToList();
                var existingInventory = WebServiceHelper.GetAllInventory();

                var prices = pricesToImport;

                foreach (var price in prices)
                {
                    var publishedProduct = existingInventory.FirstOrDefault(s => s.PrivateSKU == price.Id);

                    if (publishedProduct == null) //this price's product doesn't exist on linkgreen so let's ignore it
                    {
                        continue;
                    }

                    if (price.MinimumPurchase <= 0)
                    {
                        price.MinimumPurchase = 1;
                    }

                    if (price.Price > 0 && price.Price < publishedProduct.NetPrice) //no point in creating a price level price if it will cost more than net 
                    {
                        PricingLevelItemRequest item = new PricingLevelItemRequest
                        {
                            PriceLevelName = price.PriceLevel,
                            SupplierInventoryItemId = publishedProduct.Id,
                            Price = price.Price.Value,
                            MinimumPurchase = price.MinimumPurchase
                        };

                        WebServiceHelper.PushPricingLevelPrice(item);
                    }
                }

                return true;
            }

            Logger.Instance.Warning("No Api Key set while executing pricing publish.");
            return false;
        }
    }
}
