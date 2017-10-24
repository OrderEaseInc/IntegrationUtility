﻿using DataTransfer.AccessDatabase;

namespace LinkGreenODBCUtility
{
    public class Suppliers : IOdbcTransfer
    {
        private readonly SupplierRepository _supplierRepository;
        private const string TableName = "Suppliers";

        public Suppliers()
        {
            _supplierRepository = new SupplierRepository($"DSN={Settings.DsnName}");
        }

        public bool Empty()
        {
            _supplierRepository.ClearAll();
            Logger.Instance.Info($"{TableName} LinkGreen transfer table emptied.");
            Logger.Instance.Debug($"{Settings.DsnName}.{TableName} emptied.");
            return true;
        }

        public void SaveTableMapping(string dsnName, string tableName)
        {
            _supplierRepository.SaveTableMapping(dsnName, tableName, TableName);
            Logger.Instance.Debug($"{TableName} table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            _supplierRepository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"{TableName} field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public bool Download()
        {
            var result = _supplierRepository.DownloadAllSuppliers();
            Logger.Instance.Debug($"Downloaded {result} from LinkGreen to Transfer table {TableName}");
            return true;
        }

        public bool Publish()
        {
            var result = _supplierRepository.SyncAllSuppliers();
            foreach (var pair in result) {
                Logger.Instance.Debug($"{TableName} Sync: {pair.Value} was {pair.Key}");
            }
            return true;
        }
    }
}