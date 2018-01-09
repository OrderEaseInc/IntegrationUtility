using System.Collections.Generic;
using DataTransfer.AccessDatabase;

namespace LinkGreenODBCUtility
{
    public class SupplierInventories : IOdbcTransfer
    {
        private readonly SupplierInventoryRepository repository;
        private const string TableName = "SupplierInventories";
        private const string TableKey = "SupplierSKU";
        public bool _validFields;

        public SupplierInventories()
        {
            repository = new SupplierInventoryRepository(Settings.ConnectionString);
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
            repository.SaveTableMapping(dsnName, tableName, "SupplierInventories");
            Logger.Instance.Debug($"Supplier Inventory table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            repository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Supplier Inventory field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public bool Download()
        {
            repository.DownloadSupplierInventory();
            Logger.Instance.Debug($"Downloaded from LinkGreen to Transfer table {TableName}");
            return true;
        }

        public bool PushMatchedSkus()
        {
            // clear out transfer table
            Empty();

            var mappedDsnName = new Mapping().GetDsnName(TableName);
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData(TableName))
            {
                Logger.Instance.Debug($"Suppliers migrated using DSN: {mappedDsnName}");
            }
            else
            {
                if (!newMapping._validFields)
                {
                    _validFields = false;
                }
                else
                {
                    Logger.Instance.Warning("Failed to migrate suppliers.");
                }

                return false;
            }

            // Push any matched BuyerSKUs back up to LinkGreen
            repository.SyncAllSupplierInventories();

            return true;
        }

        public bool Publish()
        {
            // clear out transfer table
            Empty();
            // populate transfer table with LinkGreen data
            Download();

            // Clear out the supplier inventory data & re-populate it
            var mappedDsnName = new Mapping().GetDsnName(TableName);
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.PushData(TableName, TableKey, true))
            {
                Logger.Instance.Debug("Supplier Inventory migrated from utility to mapped production database.");
                return true;
            }

            if (!newMapping._validPushFields)
            {
                _validFields = false;
            }

            Logger.Instance.Error("Failed to migrate Supplier Inventory from utility to mapped production database.");

            return false;
        }
    }
}