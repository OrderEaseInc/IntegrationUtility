using System.Data.Odbc;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

namespace DataTransfer.AccessDatabase
{
    public class SupplierInventoryRepository : AdoRepository<SupplierInventory>
    {
        private const string TableName = "SupplierInventories";

        public SupplierInventoryRepository(string connectionString) : base(connectionString) { }

        public void DownloadSupplierInventory()
        {
            var suppliers = WebServiceHelper.GetAllSuppliers();
            
            foreach (var supplier in suppliers) {
                var inventory = WebServiceHelper.GetSupplierInventory(supplier.Id);
                if (inventory != null) {
                    foreach (var item in inventory) {
                        Insert(item);
                    }
                }
            }
        }

        public override void SaveFieldMapping(string fieldName, string mappingName)
        {
            using (OdbcCommand command = new OdbcCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{TableName}'")) {
                ExecuteCommand(command);
            }
        }

        public void ClearAll()
        {
            using (OdbcCommand command = new OdbcCommand($"DELETE * FROM {TableName}")) {
                ExecuteCommand(command);
            }
        }

        private void Insert(SupplierInventory inventory)
        {
            var sql = $"INSERT INTO {TableName} (BuyerLinkedSku, CatalogPrice, Description, Inventory, ItemId, SizeDescription, SupplierSku) " +
                $"VALUES ({NullableString(inventory.BuyerLinkedSku)}, {NullableDecimal(inventory.CatalogPrice)}, " +
                $"{NullableString(inventory.Description)}, {NullableInt(inventory.Inventory)}, {inventory.ItemId}," +
                $"{NullableString(inventory.SizeDescription)}, {NullableString(inventory.SupplierSku)})";
            using (var command = new OdbcCommand(sql)) {
                ExecuteCommand(command);
            }
        }
    }
}