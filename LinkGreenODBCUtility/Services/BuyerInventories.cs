using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

namespace LinkGreenODBCUtility
{
    public class BuyerInventories : IOdbcTransfer
    {
        private readonly BuyerInventoryRepository _repository = new BuyerInventoryRepository(Settings.ConnectionString);
        private const string TableName = "BuyerInventories";
        private const string TableKey = "BuyerInventoryId";

        public bool Empty()
        {
            _repository.ClearAll();
            Logger.Instance.Info($"{TableName} LinkGreen transfer table emptied.");
            Logger.Instance.Debug($"{Settings.ConnectionString}.{TableName} emptied.");
            return true;
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            _repository.SaveTableMapping(dsnName, tableName, TableName);
            Logger.Instance.Debug($"{TableName} table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            _repository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"{TableName} field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public void UpdateTemporaryTables()
        {
            BatchTaskManager batchTaskManager = new BatchTaskManager("BuyerInventories");
            var commands = batchTaskManager.GetCommandsByTrigger();
            foreach (string cmd in commands)
            {
                Batch.Exec(cmd);
            }
        }

        public bool Publish(BackgroundWorker bw = null)
        {
            Empty();

            var mappedDsnName = new Mapping().GetDsnName("BuyerInventories");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("BuyerInventories"))
            {
                Logger.Instance.Debug($"Buyer Inventory migrated using DSN: {mappedDsnName}");
            }
            else
            {
                Logger.Instance.Warning("Failed to migrate Buyer Inventory.");
            }

            var apiKey = Settings.GetApiKey(); 

            if (string.IsNullOrEmpty(apiKey))
            {
                Logger.Instance.Warning("No Api Key set while executing products publish.");
                return false;
            }

            var products = _repository.GetAll().ToList();
            var existingCategories = WebServiceHelper.GetAllCategories();

            // TODO: There's no GetAll() for the BuyerInventory service
            var existingInventory = WebServiceHelper.GetAllInventory();
            var items = 0;

            foreach (var product in products)
            {
                var existingCategory = existingCategories.FirstOrDefault(c => c.Name == product.Category);
                if (existingCategory == null)
                {
                    existingCategory = WebServiceHelper.PushCategory(new PrivateCategory { Name = product.Category });
                    existingCategories.Add(existingCategory);
                }

                var existingProduct = existingInventory.FirstOrDefault(i => i.PrivateSKU == product.PrivateSku);

                dynamic request = new ExpandoObject();
                // NOTE: This is case-sensitive!?
                request.PrivateSKU = product.PrivateSku;
                request.CategoryId = (existingCategory?.Id).GetValueOrDefault();
                request.Description = product.Description ?? string.Empty;

                if (existingProduct != null)
                {
                    request.Id = existingProduct.Id;
                }
                if (product.Inactive.HasValue)
                {
                    request.Inactive = product.Inactive.Value;
                }

                //TODO: Get the location first
                if (product.LocationId.HasValue)
                {
                    request.LocationId = product.LocationId.Value;
                }
                if (!string.IsNullOrEmpty(product.UPC))
                {
                    request.UPC = product.UPC;
                }

                if (product.MinOrderSpring.HasValue)
                {
                    request.MinOrderSpring = product.MinOrderSpring.Value;
                }
                if (product.MinOrderSummer.HasValue)
                {
                    request.MinOrderSummer = product.MinOrderSummer.Value;
                }
                if (product.FreightFactor.HasValue)
                {
                    request.FreightFactor = product.FreightFactor.Value;
                }
                if (product.QuantityAvailable.HasValue)
                {
                    request.QuantityAvailable = product.QuantityAvailable >= 1 ? product.QuantityAvailable : 1;
                }
                if (!string.IsNullOrEmpty(product.Comments))
                {
                    request.Comments = product.Comments;
                }
                if (product.SuggestedRetailPrice.HasValue)
                {
                    request.SuggestedRetailPrice = product.SuggestedRetailPrice.Value;
                }
                if (!string.IsNullOrEmpty(product.OpenSizeDescription))
                {
                    request.OpenSizeDescription = product.OpenSizeDescription;
                }
                if (product.NetPrice.HasValue)
                {
                    request.NetPrice = product.NetPrice;
                }
                if (product.SlaveQuantityPerMaster.HasValue)
                {
                    request.SlaveQuantityPerMaster = product.SlaveQuantityPerMaster;
                }
                if (!string.IsNullOrEmpty(product.SlaveQuantityDescription))
                {
                    request.SlaveQuantityDescription = product.SlaveQuantityDescription;
                }
                if (!string.IsNullOrEmpty(product.MasterQuantityDescription))
                {
                    request.MasterQuantityDescription = product.MasterQuantityDescription;
                }
                if (product.RetailPrice.HasValue)
                {
                    request.RetailPrice = product.RetailPrice;
                }
                if (product.RetailOrderLevel.HasValue)
                {
                    request.RetailOrderLevel = product.RetailOrderLevel;
                }
                if (product.AmazonSell.HasValue)
                {
                    request.AmazonSell = product.AmazonSell;
                }
                if (product.OnlineSell.HasValue)
                {
                    request.OnlineSell = product.OnlineSell;
                }
                if (product.SupplierId.HasValue)
                {
                    request.SupplierId = product.SupplierId;
                }
                if (!string.IsNullOrEmpty(product.SupplierSku))
                {
                    request.SupplierSku = product.SupplierSku;
                }

                WebServiceHelper.PushBuyerInventory(request);

                Logger.Instance.Debug($"Finished importing inventory {++items} of {products.Count}. Id: {product.Id}");
            }

            return true;
        }
    }
}