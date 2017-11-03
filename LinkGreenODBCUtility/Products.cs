using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

namespace LinkGreenODBCUtility
{
    class Products : IOdbcTransfer
    {
        public string ConnectionString = $"DSN={Settings.DsnName}";
        public string ClientConnectionString;

        public Products()
        {

        }

        public Products(string clientDsnName)
        {
            ClientConnectionString = "DSN=" + clientDsnName;
        }

        public bool Empty()
        {
            var productInventoryRepository = new ProductInventoryRepository(ConnectionString);
            productInventoryRepository.ClearAll();
            Logger.Instance.Info("Products LinkGreen transfer table emptied.");
            Logger.Instance.Debug($"{Settings.DsnName}.Products emptied.");
            return true;
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            var productRepository = new ProductInventoryRepository(ConnectionString);
            productRepository.SaveTableMapping(dsnName, tableName, "Products");
            Logger.Instance.Debug($"Products table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            var productRepository = new ProductInventoryRepository(ConnectionString);
            productRepository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Products field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public void UpdateTemporaryTables()
        {
            TaskManager taskManager = new TaskManager("Products");
            List<string> commands = taskManager.GetCommandsByTrigger();
            foreach (string cmd in commands)
            {
                Batch.Exec(cmd);
            }
        }

        public bool Publish()
        {
            string apiKey = ConfigurationManager.AppSettings["ApiKey"];

            if (!string.IsNullOrEmpty(apiKey))
            {
                var products = new ProductInventoryRepository(ConnectionString).GetAll().ToList();
                var existingCategories = WebServiceHelper.GetAllCategories();

                var existingInventory = WebServiceHelper.GetAllInventory();
                var items = 0;

                foreach (var product in products)
                {
                    Trace.TraceInformation(product.Description);

                    var request = new InventoryItemRequest
                    {
                        Description = product.Description ?? "",
                        PrivateSKU = product.Id,
                        Inactive = product.Inactive,
                        QuantityAvailable = product.QuantityAvailable >= 1 ? product.QuantityAvailable : 1
                    };
                    
                    bool updateCategories = Settings.GetUpdateCategories();
                    //lets check if this item already exists, if so just update qty, else
                    var existing = existingInventory.FirstOrDefault(s => s.PrivateSKU == product.Id);
                    var existingCategory = existingCategories.FirstOrDefault(s => s.Name == product.Category);

                    // Add
                    if (existing == null)
                    {
                        if (existingCategory == null)
                        {
                            existingCategory = WebServiceHelper.PushCategory(new PrivateCategory {Name = product.Category});
                            existingCategories.Add(existingCategory);
                        }
                    }
                    // Update
                    else
                    {
                        if (existingCategory == null && updateCategories)
                        {
                            existingCategory = WebServiceHelper.PushCategory(new PrivateCategory { Name = product.Category });
                            existingCategories.Add(existingCategory);
                        }
                    }

                    
                    if (existingCategory != null && (updateCategories || existing == null))
                    {
                         request.CategoryId = existingCategory.Id;
                    }
                    else if (existing != null)
                    {
                        request.CategoryId = existing.CategoryId;
                    }
                    
                    request.NetPrice = product.NetPrice;
                    request.OpenSizeDescription = product.OpenSizeDescription ?? "";
                    request.MasterQuantityDescription = product.MasterQuantityDescription ?? "";
                    request.Comments = product.Comments ?? "";
                    request.DirectDeliveryCode = product.DirectDeliveryCode ?? "";
                    request.DirectDeliveryMinQuantity = product.DirectDeliveryMinQuantity;
                    request.FreightFactor = product.FreightFactor;
                    request.IsDirectDelivery = product.IsDirectDelivery;
                    request.MasterQuantityDescription = product.MasterQuantityDescription ?? "";
                    request.MinOrderSpring = product.MinOrderSpring;
                    request.MinOrderSummer = product.MinOrderSummer;
                    request.SlaveQuantityDescription = product.SlaveQuantityDescription ?? "";
                    request.SlaveQuantityPerMaster = product.SlaveQuantityPerMaster;
                    request.SuggestedRetailPrice = product.SuggestedRetailPrice;

                    if (existing != null)
                    {
                        request.Id = existing.Id;
                    }

                    WebServiceHelper.PushInventoryItem(request);

                    Logger.Instance.Debug($"Finished importing product {++items} of {products.Count}. Id: {product.Id}");
                }

                WebServiceHelper.PostInventoryImport();

                return true;
            }

            Logger.Instance.Warning("No Api Key set while executing products publish.");

            return false;
        }
    }
}
