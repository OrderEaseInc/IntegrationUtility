using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Threading;
using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

namespace LinkGreenODBCUtility
{
    public class InventoryQuantity: IOdbcTransfer
    {
        private readonly InventoryQuantityRepository repository;
        private const string TableName = "InventoryQuantities";
        public bool _validFields;

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

        public bool Publish()
        {
            // clear out transfer table
            Empty();

            var mappedDsnName = new Mapping().GetDsnName("InventoryQuantities");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("InventoryQuantities"))
            {
                Logger.Instance.Debug($"Inventory Quantities migrated using DSN: {mappedDsnName}");

                string apiKey = ConfigurationManager.AppSettings["ApiKey"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    Logger.Instance.Warning("No Api Key set while executing inventory quantities publish.");
                    return false;
                }

                Thread.Sleep(500); // TODO: Code Smell - Figure out the real problem here
                var inventoryQuantityItems = repository.GetAll().ToList();
                var existingInventory = WebServiceHelper.GetAllInventory();
                var items = 0;

                foreach (var inventoryQuantityItem in inventoryQuantityItems)
                {
                    var existingProduct = existingInventory.FirstOrDefault(i => i.PrivateSKU == inventoryQuantityItem.Sku);

                    var request = new InventoryItemRequest();
                    request.PrivateSKU = inventoryQuantityItem.Sku;

                    if (existingProduct != null)
                    {
                        items++;
                        request.AccountingReference = existingProduct.AccountingReference;
                        request.CategoryId = existingProduct.CategoryId;
                        request.Comments = existingProduct.Comments;
                        request.Description = existingProduct.Description;
                        request.DirectDeliveryCode = existingProduct.DirectDeliveryCode;
                        request.FreightFactor = existingProduct.FreightFactor;
                        request.DirectDeliveryMinQuantity = existingProduct.DirectDeliveryMinQuantity;
                        request.Id = existingProduct.Id;
                        request.Inactive = existingProduct.Inactive;
                        request.IsDirectDelivery = existingProduct.IsDirectDelivery;
                        request.LocationId = existingProduct.LocationId;
                        request.MasterQuantityDescription = existingProduct.MasterQuantityDescription;
                        request.MinOrderSpring = existingProduct.MinOrderSpring;
                        request.MinOrderSummer = existingProduct.MinOrderSummer;
                        request.NetPrice = existingProduct.NetPrice;
                        request.OpenSizeDescription = existingProduct.OpenSizeDescription;
                        request.PrivateSKU = existingProduct.PrivateSKU;
                        request.SlaveQuantityPerMaster = existingProduct.SlaveQuantityPerMaster;
                        request.SuggestedRetailPrice = existingProduct.SuggestedRetailPrice;
                        request.UPC = existingProduct.UPC;
                        // set the quantity
                        request.QuantityAvailable = inventoryQuantityItem.Quantity >= 1 ? inventoryQuantityItem.Quantity : 1;

                        WebServiceHelper.PushInventoryItem(request);

                        Logger.Instance.Debug($"Set available quantity to {inventoryQuantityItem.Quantity} for Sku: {inventoryQuantityItem.Sku}");
                    }
                }

                if (items < 1)
                {
                    Logger.Instance.Warning($"No inventory quantity items were published. Double check your skus.");
                }

                return true;
            }

            Logger.Instance.Warning("Failed to migrate Inventory Quantities.");
            return false;
        }
    }
}