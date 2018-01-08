using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using LinkGreen.Applications.Common.Model;
using Microsoft.CSharp.RuntimeBinder;

namespace DataTransfer.AccessDatabase
{
    public class InventoryQuantityRepository : OleDbRepository<InventoryQuantity>
    {
        private const string TableName = "InventoryQuantities";

        public InventoryQuantityRepository(string connectionString) : base(connectionString) { }

        public override void SaveFieldMapping(string fieldName, string mappingName)
        {
            using (var command = new OleDbCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{TableName}'")) {
                ExecuteCommand(command);
            }
        }

        public void ClearAll()
        {
            using (var command = new OleDbCommand($"DELETE * FROM {TableName}")) {
                ExecuteCommand(command);
            }
        }

        public IEnumerable<InventoryQuantity> GetAll()
        {
            using (var command = new OleDbCommand($"SELECT * FROM {TableName}"))
            {
                return GetRecords(command);
            }
        }

        protected override InventoryQuantity PopulateRecord(dynamic reader)
        {
            try {

                return new InventoryQuantity {
                    Sku = reader.Sku,
                    Quantity = reader.QuantityAvailable
                };

            } catch (RuntimeBinderException exception) {
                Console.WriteLine(exception);
                throw new InvalidDataException("One of the fields in the source ODBC database has an invalid column type or value", exception);
            }
        }
    }
}