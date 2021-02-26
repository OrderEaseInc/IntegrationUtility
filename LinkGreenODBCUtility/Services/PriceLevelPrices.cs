using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;
using LinkGreenODBCUtility.Utility;

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

            var updateCounter = 0;

            if (!string.IsNullOrEmpty(apiKey))
            {
                var pricesToImport = new ProductPriceRepository(Settings.ConnectionString).GetAll().ToList();
                var existingInventory = WebServiceHelper.GetAllInventory();

                var prices = pricesToImport;

                var allPrices = new Dictionary<string, List<PricingLevelItemRequest>>();

                foreach (var price in prices)
                {
                    var publishedProduct = existingInventory.FirstOrDefault(s => s.PrivateSKU == price.Id);

                    if (publishedProduct == null) // this price's product doesn't exist on linkgreen so let's ignore it
                    {
                        publishDetails.Add($"SKU {price.Id} did not exist in LinkGreen to be updated with price");
                        continue;
                    }

                    if (price.MinimumPurchase <= 0)
                    {
                        price.MinimumPurchase = 1;
                    }

                    if (price.Price > 0) 
                    {
                        var item = new PricingLevelItemRequest
                        {
                            SupplierInventoryItemId = publishedProduct.Id,
                            Price = price.Price.Value,
                            MinimumPurchase = price.MinimumPurchase
                        };
                        updateCounter++;
                        if (allPrices.ContainsKey(price.PriceLevel))
                            allPrices[price.PriceLevel].Add(item);
                        else
                            allPrices.Add(price.PriceLevel, new List<PricingLevelItemRequest> { item });
                    }
                }

                foreach (var kvp in allPrices)
                {
                    bw?.ReportProgress(0, $"Preparing to push {kvp.Key}");
                    var chunks = kvp.Value.ChunkBy(25);
                    var iCountChunks = 0;
                    Parallel.ForEach(chunks, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (chunk) =>
                       {
                           bw?.ReportProgress(0, $"Pushing {kvp.Key} Part {++iCountChunks} of {chunks.Count}");
                           WebServiceHelper.PushPricingLevel(kvp.Key, chunk.ToArray(),
                               new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                       });
                }

                publishDetails.Insert(0, $"{updateCounter} products have had their prices updated.");

                return true;
            }

            Logger.Instance.Warning("No Api Key set while executing pricing publish.");
            return false;
        }
    }
}
