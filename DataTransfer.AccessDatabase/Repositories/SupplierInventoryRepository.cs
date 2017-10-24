using System.Collections.Generic;
using System.Data.Odbc;
using LinkGreen.Applications.Common;
using LinkGreen.Applications.Common.Model;

namespace DataTransfer.AccessDatabase
{
    public class SupplierInventoryRepository : AdoRepository<SupplierInventory>
    {
        private const string TableName = "SupplierInventory";
        private readonly string _connectionString;

        public SupplierInventoryRepository(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        public void DownloadSupplierInventory()
        {
            var buyers = WebServiceHelper.GetAllBuyers();
            
            var querier = GetQuerier();
            foreach (var buyer in buyers) {
                var inventory = WebServiceHelper.GetSupplierInventory(buyer.Id);
                foreach (var item in inventory) {
                    querier.Insert(item);
                }
            }
        }

        public override void SaveFieldMapping(string fieldName, string mappingName)
        {
            using (OdbcCommand command = new OdbcCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{TableName}'")) {
                ExecuteCommand(command);
            }
        }

        private SupplierInventoryQuerier GetQuerier()
        {
            string dsnName;
            string tableName;
            var fields = new Dictionary<string, string>();

            using (var connection = new OdbcConnection(_connectionString)) {
                connection.Open();
                using (var command = new OdbcCommand($"SELECT FieldName,MappingName FROM FieldMappings WHERE TableName = '{TableName}'", connection)) {
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            var fieldName = reader["FieldName"].ToString();
                            var mappingName = reader["MappingName"].ToString();
                            fields.Add(fieldName, mappingName);
                        }
                    }
                }
                using (var command = new OdbcCommand($"SELECT DsnName,MappingName FROM TableMappings WHERE TableName = '{TableName}'", connection)) {
                    using (var reader = command.ExecuteReader()) {
                        reader.Read();
                        dsnName = reader["DsnName"].ToString();
                        tableName = reader["MappingName"].ToString();
                    }
                }
            }

            var querier = new SupplierInventoryQuerier(dsnName, tableName, fields);
            return querier;
        }
    }

    public class SupplierInventoryQuerier
    {
        private readonly string _dsnName;
        private readonly string _tableName;
        private readonly Dictionary<string, string> _fields;

        public SupplierInventoryQuerier(string dsnName, string tableName, Dictionary<string, string> fields)
        {
            _dsnName = dsnName;
            _tableName = tableName;
            _fields = fields;
        }

        public void Clear()
        {
            var sql = $"DELETE * FROM {_tableName}";
            using (var connection = new OdbcConnection($"DSN={_dsnName}")) {
                connection.Open();
                using (var command = new OdbcCommand(sql, connection)) {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Insert(SupplierInventory inventory)
        {
            var sql = $"INSERT INTO {_tableName} ({_fields["BuyerLinkedSku"]}, {_fields["CatalogPrice"]}, " +
                      $"{_fields["Description"]}, {_fields["Inventory"]}, {_fields["ItemId"]}, " +
                      $"{_fields["SizeDescription"]}, {_fields["SupplierSku"]}) " +
                      $"VALUES ({NullableString(inventory.BuyerLinkedSku)}, {NullableDecimal(inventory.CatalogPrice)}, " +
                      $"{NullableString(inventory.Description)}, {NullableInt(inventory.Inventory)}, " +
                      $"{NullableString(inventory.SizeDescription)}, {NullableString(inventory.SupplierSku)}";
            using (var connection = new OdbcConnection($"DSN={_dsnName}")) {
                connection.Open();
                using (var command = new OdbcCommand(sql, connection)) {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static string NullableString(string value) => value == null ? "null" : $"'{value}'";
        private static string NullableInt(int? value) => value.HasValue ? value.ToString() : "null";
        private static string NullableDecimal(decimal? value) => value.HasValue ? value.ToString() : "null";
    }
}