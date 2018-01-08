using DataTransfer.AccessDatabase;

namespace LinkGreenODBCUtility
{
    public class LinkedSkus: IOdbcTransfer
    {
        private readonly LinkedSkusRepository repository;
        private const string TableName = "LinkedSkus";
        public bool _validFields;

        public LinkedSkus()
        {
            repository = new LinkedSkusRepository(Settings.ConnectionString);
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
            repository.SaveTableMapping(dsnName, tableName, "LinkedSkus");
            Logger.Instance.Debug($"Linked Skus table mapping saved: (DSN: {dsnName}, Table: {tableName})");
        }

        public void SaveFieldMapping(string fieldName, string mappingName)
        {
            repository.SaveFieldMapping(fieldName, mappingName);
            Logger.Instance.Debug($"Linked Skus field mapping saved: (Field: {fieldName}, MappingField: {mappingName})");
        }

        public bool Publish()
        {
            // clear out transfer table
            Empty();

            var mappedDsnName = new Mapping().GetDsnName(TableName);
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData(TableName))
            {
                Logger.Instance.Debug($"Linked Skus migrated using DSN: {mappedDsnName}");
            }
            else
            {
                if (!newMapping._validFields)
                {
                    _validFields = false;
                }
                else
                {
                    Logger.Instance.Warning("Failed to migrate Linked Skus.");
                }

                return false;
            }

            // Push any matched BuyerSKUs up to LinkGreen
            repository.SyncAllLinkedSkus();

            return true;
        }
    }
}