using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;
using LinkGreenODBCUtility.Utility;
using Mapper = AutoMapper.Mapper;

// ReSharper disable once CheckNamespace
namespace LinkGreenODBCUtility
{
    public class InventoryQuantity : IOdbcTransfer
    {
        private readonly InventoryQuantityRepository repository;
        private const string TableName = "InventoryQuantities";

        public InventoryQuantity()
        {
            repository = new InventoryQuantityRepository(Settings.ConnectionString);
        }

        public bool Empty()
        {
            repository.ClearAll();
            Logger.Instance.Info($"{TableName} LinkGreen transfer table emptied.");
            Logger.Instance.Debug($"{Settings.ConnectionString}.{TableName} emptied.");
            return true;
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            repository.SaveTableMapping(dsnName, tableName, "InventoryQuantities");
            Logger.Instance.Debug($"Inventory Quantities table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            repository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Inventory Quantities field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public bool Publish(out List<string> publishDetails, BackgroundWorker bw = null)
        {
            publishDetails = new List<string>();

            try
            {
                // clear out transfer table
                Empty();

                var mappedDsnName = new Mapping().GetDsnName("InventoryQuantities");
                var newMapping = new Mapping(mappedDsnName);
                if (newMapping.MigrateData("InventoryQuantities"))
                {
                    Logger.Instance.Debug($"Inventory Quantities migrated using DSN: {mappedDsnName}");

                    var apiKey = Settings.GetApiKey();

                    if (string.IsNullOrEmpty(apiKey))
                    {
                        Logger.Instance.Warning("No Api Key set while executing inventory quantities publish.");
                        publishDetails.Insert(0, "No Api Key set while executing inventory quantities publish");
                        return false;
                    }

                    Thread.Sleep(500); // TODO: Code Smell - Figure out the real problem here
                    var inventoryQuantityItems = repository.GetAll().ToList();
                    var existingInventory = WebServiceHelper.GetAllInventory();
                    var items = 0;

                    var request = new List<IdSkuQuantity>();

                    foreach (var inventoryQuantityItem in inventoryQuantityItems)
                    {
                        var existingProduct = existingInventory.FirstOrDefault(i => i.PrivateSKU == inventoryQuantityItem.Sku);

                        //var request = new InventoryItemRequest { PrivateSKU = inventoryQuantityItem.Sku };

                        // ReSharper disable once InvertIf
                        if (existingProduct != null)
                        {
                            items++;

                            request.Add(Mapper.Map<IdSkuQuantity>(inventoryQuantityItem));
                        }
                    }


                    if (items > 0 && request.All(i => string.IsNullOrEmpty(i.CatalogName)))
                    {
                        var chunks = request.ChunkBy(500);
                        foreach (var chunk in chunks)
                            WebServiceHelper.PushInventoryQuantity(chunk);
                    }
                    else if (items > 0)
                    {
                        foreach (var item in request)
                        {
                            WebServiceHelper.UpdateInventoryItemQuantity(item.SKU, item.Quantity, item.CatalogName);
                        }
                    }

                    if (items < 1)
                    {
                        Logger.Instance.Warning("No inventory quantity items were published. Double check your skus.");
                        publishDetails.Insert(0, "No inventory quantity items were published. Double check your skus");
                        return false;
                    }

                    publishDetails.Insert(0, $"{items} inventory quantity items were published.");

                    return true;
                }
            }
            catch (System.Exception ex)
            {
                Logger.Instance.Warning("Failed to migrate Inventory Quantities.");
                publishDetails.Insert(0, "Failed to migrate Inventory Quantities");
                Logger.Instance.Error("Inventory Quanity Error" + ex.Message);
                Logger.Instance.Error(ex.StackTrace);
                return false;
            }

            Logger.Instance.Warning("Failed to migrate Inventory Quantities Non-Error.");
            publishDetails.Insert(0, "Failed to migrate Inventory Quantities Non-Error");
            return false;
        }
    }
}