using System.ComponentModel;
using DataTransfer.AccessDatabase;

namespace LinkGreenODBCUtility
{
    public class Suppliers : IOdbcTransfer
    {
        private readonly SupplierRepository _supplierRepository;
        private const string TableName = "Suppliers";
        private const string TableKey = "SupplierId";
        public bool _validPushFields;

        public Suppliers()
        {
            _supplierRepository = new SupplierRepository(Settings.ConnectionString);
        }

        public bool Empty()
        {
            _supplierRepository.ClearAll();
            Logger.Instance.Info($"{TableName} LinkGreen transfer table emptied.");
            Logger.Instance.Debug($"{Settings.ConnectionString}.{TableName} emptied.");
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
            Logger.Instance.Debug(
                $"{TableName} field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public bool Download()
        {
            var result = _supplierRepository.DownloadAllSuppliers();
            Logger.Instance.Debug($"Downloaded {result} from LinkGreen to Transfer table {TableName}");
            return true;
        }

        public bool Publish(BackgroundWorker bw = null)
        {
            Empty();
            // Download from LinkGreen to Access
            Download();
            // Push any missing records to the Production database
            var mappedDsnName = new Mapping().GetDsnName(TableName);
            var newMapping = new Mapping(mappedDsnName);

            if (newMapping.PushData(TableName, TableKey))
            {
                // Update the Access database with the latest info from the production db
                newMapping.UpdateData(TableName, TableKey);

                // Send it up to LinkGreen
                var result = _supplierRepository.SyncAllSuppliers();
                Logger.Instance.Debug($"{TableName} {result} suppliers contact info updated in LinkGreen");

                return true;
            }

            if (!newMapping._validPushFields)
            {
                _validPushFields = false;
            }

            return false;
        }
    }
}