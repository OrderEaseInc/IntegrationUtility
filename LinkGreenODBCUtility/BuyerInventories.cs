using System.Configuration;
using System.Linq;
using DataTransfer.AccessDatabase;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

namespace LinkGreenODBCUtility
{
    public class BuyerInventories : IOdbcTransfer
    {
        private readonly BuyerInventoryRepository _repository = new BuyerInventoryRepository($"DSN={Settings.DsnName}");
        private const string TableName = "BuyerInventories";
        private const string TableKey = "BuyerInventoryId";

        public bool Empty()
        {
            _repository.ClearAll();
            Logger.Instance.Info($"{TableName} LinkGreen transfer table emptied.");
            Logger.Instance.Debug($"{Settings.DsnName}.{TableName} emptied.");
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
            TaskManager taskManager = new TaskManager("BuyerInventories");
            var commands = taskManager.GetCommandsByTrigger();
            foreach (string cmd in commands) {
                Batch.Exec(cmd);
            }
        }

        public bool Publish()
        {
            string apiKey = ConfigurationManager.AppSettings["ApiKey"];

            if (string.IsNullOrEmpty(apiKey)) {
                Logger.Instance.Warning("No Api Key set while executing products publish.");
                return false;
            }

            var products = _repository.GetAll().ToList();
            var existingCategories = WebServiceHelper.GetAllCategories();

            // TODO: There's no GetAll() for the BuyerInventory service
            var existingInventory = WebServiceHelper.GetAllInventory();
            var items = 0;

            foreach (var product in products) {
                var existingCategory = existingCategories.FirstOrDefault(c => c.Name == product.Category);
                if (existingCategory == null) {
                    existingCategory = WebServiceHelper.PushCategory(new PrivateCategory { Name = product.Category });
                    existingCategories.Add(existingCategory);
                }

                var request = new BuyerInventory {
                    PrivateSku = product.PrivateSku,
                    Description = product.Description ?? string.Empty,
                    Inactive =  product.Inactive,
                    CategoryId = (existingCategory?.Id).GetValueOrDefault(),
                    LocationId = product.LocationId,
                    UPC = product.UPC ?? string.Empty,
                    MinOrderSpring = product.MinOrderSpring,
                    MinOrderSummer = product.MinOrderSummer,
                    FreightFactor = product.FreightFactor,
                    QuantityAvailable = product.QuantityAvailable >= 1 ? product.QuantityAvailable : 1,
                    Comments = product.Comments ?? string.Empty,
                    SuggestedRetailPrice = product.SuggestedRetailPrice,
                    OpenSizeDescription = product.OpenSizeDescription ?? string.Empty,
                    NetPrice = product.NetPrice,
                    SlaveQuantityPerMaster = product.SlaveQuantityPerMaster,
                    SlaveQuantityDescription = product.SlaveQuantityDescription ?? string.Empty,
                    MasterQuantityDescription = product.MasterQuantityDescription ?? string.Empty,
                    RetailPrice = product.RetailPrice,
                    RetailOrderLevel = product.RetailOrderLevel,
                    AmazonSell = product.AmazonSell,
                    OnlineSell = product.OnlineSell,
                    SupplierId = product.SupplierId,
                    SupplierSku = product.SupplierSku ?? string.Empty
                };

                var existingProduct = existingInventory.FirstOrDefault(i => i.PrivateSKU == product.PrivateSku);
                if (existingProduct != null) {
                    request.Id = existingProduct.Id;
                }

                WebServiceHelper.PushBuyerInventory(request);

                Logger.Instance.Debug($"Finished importing buyer inventory {++items} of {products.Count}. Id: {product.Id}");
            }

            return true;
        }
    }
}