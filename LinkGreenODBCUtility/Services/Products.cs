using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace LinkGreenODBCUtility
{
    class Products : IOdbcTransfer
    {
        // public string ConnectionString = $"DSN={Settings.DsnName}";
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
            var productInventoryRepository = new ProductInventoryRepository(Settings.ConnectionString);
            productInventoryRepository.ClearAll();
            Logger.Instance.Info("Products LinkGreen transfer table emptied.");
            Logger.Instance.Debug($"{Settings.ConnectionString}.Products emptied.");
            return true;
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            var productRepository = new ProductInventoryRepository(Settings.ConnectionString);
            productRepository.SaveTableMapping(dsnName, tableName, "Products");
            Logger.Instance.Debug($"Products table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            var productRepository = new ProductInventoryRepository(Settings.ConnectionString);
            productRepository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Products field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public void UpdateTemporaryTables()
        {
            BatchTaskManager batchTaskManager = new BatchTaskManager("Products");
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

            if (string.IsNullOrEmpty(apiKey))
            {
                publishDetails.Insert(0, "No Api Key set while executing products publish.");
                Logger.Instance.Warning("No Api Key set while executing products publish.");

                return false;
            }

            var products = new ProductInventoryRepository(Settings.ConnectionString).GetAll().ToList();
            var existingCategories = WebServiceHelper.GetAllCategories();

            var existingInventory = WebServiceHelper.GetAllInventory();
            var items = 0;

            var productsToImport = new List<SupplierInventoryItemImport>();

            foreach (var product in products)
            {
                try
                {
                    var request = MapProducts(product, existingInventory, ref existingCategories);
                    if (request != null)
                        productsToImport.Add(request);

                    if (productsToImport.Count == 400)
                    {
                        ProcessProductImport(productsToImport);
                        productsToImport.Clear();
                    }

                    bw?.ReportProgress(0, $"Processing product sync (Pushing {++items}/{products.Count})\n\rPlease wait");
                    //Logger.Instance.Debug($"Finished importing product {items} of {products.Count}. Id: {product.Id}");
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error("Import " + JsonConvert.SerializeObject(product) + Environment.NewLine + ex.Message +
                                          Environment.NewLine + ex.StackTrace);
                }
            }

            if (productsToImport.Count < 400)
                ProcessProductImport(productsToImport);

            publishDetails.Insert(0, $"{items} products published to OrderEase");
            return true;
        }

        private static int? ProcessProductImport(List<SupplierInventoryItemImport> productsToImport)
        {
            var importRequest = new ProductImportRequest()
            {
                Products = productsToImport,
                ImportConfig = new ProductImportConfig()
            };

            importRequest.ImportConfig.ProductImportOptions.Add(new ProductOptions()
            {
                Name = ProductImportOptions.CreateCategoryIfNotExists,
                Selected = true
            });

            importRequest.ImportConfig.IntegrationKey = Guid.Parse("7201CCB7-AF24-4B16-9459-4DFCF8F9CB8A");

            importRequest.ImportConfig.MappedColumns = new List<string>()
            {
                nameof(SupplierInventoryItemImport.PrivateSKU),
                nameof(SupplierInventoryItemImport.Description),
                nameof(SupplierInventoryItemImport.Comments),
                nameof(SupplierInventoryItemImport.NetPrice),
                nameof(SupplierInventoryItemImport.Inactive),
                nameof(SupplierInventoryItemImport.MasterQuantityDescription),
                nameof(SupplierInventoryItemImport.SlaveQuantityDescription),
                nameof(SupplierInventoryItemImport.SlaveQuantityPerMaster),
                nameof(SupplierInventoryItemImport.UPC),
                nameof(SupplierInventoryItemImport.ProdWeight),
                nameof(SupplierInventoryItemImport.UnitOfMeasureWeight),
                nameof(SupplierInventoryItemImport.ExternalReference),
                nameof(SupplierInventoryItemImport.ExternalSource),
                nameof(SupplierInventoryItemImport.CategoryName),
                nameof(SupplierInventoryItemImport.CatalogIntegrationReferences),
                nameof(SupplierInventoryItemImport.ProductFeatureItems)
            };

            long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            importRequest.ImportConfig.FileName = $"ODBC_Products_{timestamp}_{productsToImport.Count}.json";

            return WebServiceHelper.ImportProducts(importRequest);
        }

        private static SupplierInventoryItemImport MapProducts(ProductInventory product, List<InventoryItemResponse> existingInventory, ref List<PrivateCategory> existingCategories)
        {
            Trace.TraceInformation(product.Description);

            var importItem = new SupplierInventoryItemImport()
            {
                Description = product.Description ?? "",
                PrivateSKU = product.Id,
                Inactive = product.Inactive,
                QuantityAvailable = product.QuantityAvailable >= 0 ? product.QuantityAvailable : 0,
                RetailSell = product.RetailSell,
                DropShipSell = product.DropShipSell
            };


            bool updateCategories = Settings.GetUpdateCategories();
            //lets check if this item already exists, if so just update qty, else
            var existing = existingInventory.FirstOrDefault(s => s.PrivateSKU == product.Id);
            var existingCategory = existingCategories.FirstOrDefault(s => s?.Name == product.Category);

            var updateExistingProducts = Settings.GetUpdateExistingProducts();
            if (existing != null && !updateExistingProducts)
                return null;

            // Add
            if (existing == null)
            {
                if (existingCategory == null)
                {
                    try
                    {
                        existingCategory = WebServiceHelper.PushCategory(new CreateCategoryRequest { data = product.Category });
                        existingCategories.Add(existingCategory);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Error("Product push - adding category" + Environment.NewLine +
                                              JsonConvert.SerializeObject(product.Category) + Environment.NewLine +
                                              ex.Message + Environment.NewLine + ex.StackTrace);
                        throw;
                    }
                }
            }
            // Update
            else
            {
                if (existingCategory == null && updateCategories)
                {
                    try
                    {
                        existingCategory = WebServiceHelper.PushCategory(new CreateCategoryRequest { data = product.Category });
                        existingCategories.Add(existingCategory);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Error("Product push - updating category" + Environment.NewLine +
                                              JsonConvert.SerializeObject(product.Category) + Environment.NewLine +
                                              ex.Message + Environment.NewLine + ex.StackTrace);
                        throw;
                    }
                }
            }


            if (existingCategory != null && (updateCategories || existing == null))
            {
                importItem.CategoryId = existingCategory.Id;
            }
            else if (existing != null)
            {
                importItem.CategoryId = existing.CategoryId;
            }

            importItem.NetPrice = product.NetPrice;
            importItem.OpenSizeDescription = product.OpenSizeDescription ?? "";
            importItem.MasterQuantityDescription = product.MasterQuantityDescription ?? "";
            importItem.Comments = product.Comments ?? "";
            importItem.DirectDeliveryCode = product.DirectDeliveryCode ?? "";
            importItem.DirectDeliveryMinQuantity = product.DirectDeliveryMinQuantity;
            importItem.FreightFactor = product.FreightFactor;
            importItem.IsDirectDelivery = product.IsDirectDelivery;
            importItem.MasterQuantityDescription = product.MasterQuantityDescription ?? "";
            importItem.MinOrderSpring = product.MinOrderSpring;
            importItem.MinOrderSummer = product.MinOrderSummer;
            importItem.SlaveQuantityDescription = product.SlaveQuantityDescription ?? "";
            importItem.SlaveQuantityPerMaster = product.SlaveQuantityPerMaster;
            importItem.SuggestedRetailPrice = product.SuggestedRetailPrice;
            importItem.UPC = product.UPC;


            if (product.ProductFeatures != null && product.ProductFeatures.Any())
            {
                importItem.ProductFeatureItems = product.ProductFeatures
                    .Where(p => p.Key != null && p.Value != null)
                    .Select(p => new ProductFeatureItem
                    {
                        FeatureGroupName = p.Key,
                        Value_EN = p.Value == null ? "" : p.Value.ToString(),
                        FeatureId = p.Key + "_" + product.Id
                    }).ToList();
            }

            return importItem;
        }

    }
}
