using System.Collections.Generic;
using System.Data.Odbc;
using LinkGreen.Applications.Common.Model;

namespace DataTransfer.AccessDatabase
{
    public class BuyerInventoryRepository : AdoRepository<BuyerInventory>
    {
        private const string TableName = "SupplierInventory";

        public BuyerInventoryRepository(string connectionString) : base(connectionString) { }

        public void ClearAll()
        {
            using (OdbcCommand command = new OdbcCommand($"DELETE * FROM {TableName}")) {
                ExecuteCommand(command);
            }
        }

        public override void SaveFieldMapping(string fieldName, string mappingName)
        {
            using (OdbcCommand command = new OdbcCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{TableName}'")) {
                ExecuteCommand(command);
            }
        }

        public IEnumerable<BuyerInventory> GetAll()
        {
            using (var command = new OdbcCommand($"SELECT * FROM {TableName}")) {
                return GetRecords(command);
            }
        }
    }
}