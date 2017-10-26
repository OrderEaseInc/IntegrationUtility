using DataTransfer.AccessDatabase;

namespace LinkGreenODBCUtility
{
    public class SupplierInventories : IOdbcTransfer
    {
        private readonly SupplierInventoryRepository repository;

        public SupplierInventories()
        {
            repository = new SupplierInventoryRepository($"DSN={Settings.DsnName}");
        }

        public bool Empty()
        {
            throw new System.NotImplementedException();
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            repository.SaveTableMapping(dsnName, tableName, "SupplierInventory");
            Logger.Instance.Debug($"Supplier Inventory table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            repository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Supplier Inventory field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public bool Sync()
        {
            repository.DownloadSupplierInventory();
            return true;
        }
    }
}