using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using LinkGreen.Applications.Common.Model;
using Microsoft.CSharp.RuntimeBinder;

namespace DataTransfer.AccessDatabase
{
    public class BuyerInventoryRepository : OleDbRepository<BuyerInventory>
    {
        private const string TableName = "BuyerInventories";

        public BuyerInventoryRepository(string connectionString) : base(connectionString) { }

        public void ClearAll()
        {
            using (var command = new OleDbCommand($"DELETE * FROM {TableName}"))
            {
                ExecuteCommand(command);
            }
        }

        public override void SaveFieldMapping(string fieldName, string mappingName)
        {
            using (var command = new OleDbCommand($"UPDATE `FieldMappings` SET `MappingName` = '{mappingName}' WHERE `FieldName` = '{fieldName}' AND `TableName` = '{TableName}'"))
            {
                ExecuteCommand(command);
            }
        }

        public IEnumerable<BuyerInventory> GetAll()
        {
            using (var command = new OleDbCommand($"SELECT * FROM {TableName}"))
            {
                return GetRecords(command);
            }
        }

        protected override BuyerInventory PopulateRecord(dynamic reader)
        {
            try
            {
                return new BuyerInventory
                {
                    Id = reader.Id,
                    Category = reader.Category,
                    Description = reader.Description,
                    PrivateSku = reader.PrivateSku,
                    Location = reader.Location,
                    UPC = reader.UPC,
                    MinOrderSpring = reader.MinOrderSpring,
                    MinOrderSummer = reader.MinOrderSummer,
                    FreightFactor = reader.FreightFactor,
                    QuantityAvailable = reader.QuantityAvailable,
                    Comments = reader.Comments,
                    SuggestedRetailPrice = reader.SuggestedRetailPrice,
                    OpenSizeDescription = reader.OpenSizeDescription,
                    NetPrice = reader.NetPrice,
                    SlaveQuantityPerMaster = reader.SlaveQuantityPerMaster,
                    SlaveQuantityDescription = reader.SlaveQuantityDescription,
                    MasterQuantityDescription = reader.MasterQuantityDescription,
                    Inactive = reader.Inactive,
                    RetailPrice = reader.RetailPrice,
                    RetailOrderLevel = reader.RetailOrderLevel,
                    AmazonSell = reader.AmazonSell,
                    OnlineSell = reader.OnlineSell,
                    SupplierId = reader.SupplierId,
                    SupplierSku = reader.SupplierSku
                };
            }
            catch (RuntimeBinderException exception)
            {
                Console.WriteLine(exception);
                throw new InvalidDataException("One of the fields in the source ODBC database has an invalid column type or value", exception);
            }
        }
    }
}